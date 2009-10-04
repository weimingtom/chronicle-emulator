using Chronicle.Game;
using Chronicle.Utility;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chronicle.Network
{
    public sealed class Client
    {
        private static Dictionary<EOpcode, PacketHandlerAttribute> sHandlers = new Dictionary<EOpcode, PacketHandlerAttribute>();

        [Initializer(10)]
        public static bool InitializeHandlers()
        {
            List<Doublet<PacketHandlerAttribute, PacketProcessor>> handlers = Reflector.FindAllMethods<PacketHandlerAttribute, PacketProcessor>();
            handlers.ForEach(d => { d.First.Processor = d.Second; sHandlers.Add(d.First.Opcode, d.First); });
            Log.WriteLine(ELogLevel.Info, "[Client] Initialized {0} Packet Handlers", sHandlers.Count);
            return true;
        }


        private const int MAX_RECEIVE_BUFFER = 16384;

        private Socket mSocket = null;
        private string mHost = null;
        private int mDisconnected = 0;

        private byte[] mReceiveBuffer = null;
        private int mReceiveStart = 0;
        private int mReceiveLength = 0;
        private DateTime mReceiveLast = DateTime.Now;
        private LockFreeQueue<ByteArraySegment> mSendSegments = new LockFreeQueue<ByteArraySegment>();
        private int mSending = 0;

        private Crypto mReceiveCrypto = null;
        private Crypto mSendCrypto = null;
        private ushort mReceivingPacketLength = 0;

        private Account mAccount = null;
        private Player mPlayer = null;

        public Client(Socket pSocket)
        {
            mSocket = pSocket;
            mReceiveBuffer = new byte[MAX_RECEIVE_BUFFER];
            mHost = ((IPEndPoint)mSocket.RemoteEndPoint).Address.ToString();
            Log.WriteLine(ELogLevel.Debug, "[{0}] Connected", Host);
            BeginReceive();
        }

        public string Host { get { return mHost; } }
        public Account Account { get { return mAccount; } internal set { mAccount = value; } }
        public Player Player { get { return mPlayer; } internal set { mPlayer = value; } }

        public void Disconnect()
        {
            if (Interlocked.CompareExchange(ref mDisconnected, 1, 0) == 0)
            {
                mSocket.Shutdown(SocketShutdown.Both);
                mSocket.Close();
                Log.WriteLine(ELogLevel.Debug, "[{0}] Disconnected", Host);
                if (mAccount != null && mPlayer != null)
                {
                    // Save Player
                    mPlayer.Map.RemovePlayer(mPlayer);
                }
                Server.ClientDisconnected(this);
            }
        }

        private void BeginReceive()
        {
            if (mDisconnected != 0) return;
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += (s, a) => EndReceive(a);
            args.SetBuffer(mReceiveBuffer, mReceiveStart, mReceiveBuffer.Length - (mReceiveStart + mReceiveLength));
            try { if (!mSocket.ReceiveAsync(args)) EndReceive(args); }
            catch (ObjectDisposedException) { }
        }

        private void EndReceive(SocketAsyncEventArgs pArguments)
        {
            if (mDisconnected != 0) return;
            if (pArguments.BytesTransferred <= 0)
            {
                if (pArguments.SocketError != SocketError.Success && pArguments.SocketError != SocketError.ConnectionReset) Log.WriteLine(ELogLevel.Error, "[{0}] Receive Error: {1}", Host, pArguments.SocketError);
                Disconnect();
                return;
            }
            mReceiveLength += pArguments.BytesTransferred;

            while (mReceiveLength > 4)
            {
                if (mReceivingPacketLength == 0)
                {
                    if (!mReceiveCrypto.ConfirmHeader(mReceiveBuffer, mReceiveStart))
                    {
                        Log.WriteLine(ELogLevel.Error, "[{0}] Invalid Packet Header", Host);
                        Disconnect();
                        return;
                    }
                    mReceivingPacketLength = mReceiveCrypto.GetHeaderLength(mReceiveBuffer, mReceiveStart);
                }
                if (mReceivingPacketLength > 0 && mReceiveLength >= mReceivingPacketLength + 4)
                {
                    mReceiveCrypto.Decrypt(mReceiveBuffer, mReceiveStart + 4, mReceivingPacketLength);

                    Packet packet = new Packet(mReceiveBuffer, mReceiveStart + 4, mReceivingPacketLength);
                    PacketHandlerAttribute handler = sHandlers.GetOrDefault(packet.Opcode, null);
                    if (handler != null) Server.AddCallback(() => handler.Processor(this, packet));
                    else
                    {
                        Log.WriteLine(ELogLevel.Debug, "[{0}] Receiving 0x{1}, {2} Bytes", Host, ((ushort)packet.Opcode).ToString("X4"), packet.Length);
                        packet.Dump();
                    }

                    mReceiveStart += mReceivingPacketLength + 4;
                    mReceiveLength -= mReceivingPacketLength + 4;
                    mReceivingPacketLength = 0;
                    mReceiveLast = DateTime.Now;
                }
            }

            if (mReceiveLength == 0) mReceiveStart = 0;
            else if (mReceiveStart > 0 && (mReceiveStart + mReceiveLength) >= mReceiveBuffer.Length)
            {
                Buffer.BlockCopy(mReceiveBuffer, mReceiveStart, mReceiveBuffer, 0, mReceiveLength);
                mReceiveStart = 0;
            }
            if (mReceiveLength == mReceiveBuffer.Length)
            {
                Log.WriteLine(ELogLevel.Error, "[{0}] Receive Overflow", Host);
                Disconnect();
            }
            else BeginReceive();
        }

        internal void SendHandshake(ushort pBuild, byte[] pReceiveIV, byte[] pSendIV)
        {
            mReceiveCrypto = new Crypto(pBuild, pReceiveIV);
            mSendCrypto = new Crypto((ushort)(0xFFFF - pBuild), pSendIV);

            byte[] buffer = new byte[15];
            buffer[0] = 0x0D;
            buffer[2] = (byte)pBuild;
            buffer[3] = (byte)(pBuild >> 8);
            Buffer.BlockCopy(pReceiveIV, 0, buffer, 6, 4);
            Buffer.BlockCopy(pSendIV, 0, buffer, 10, 4);
            buffer[14] = 0x08;
            Send(buffer);
        }
        private void Send(byte[] pBuffer)
        {
            if (mDisconnected != 0) return;
            mSendSegments.Enqueue(new ByteArraySegment(pBuffer));
            if (Interlocked.CompareExchange(ref mSending, 1, 0) == 0) BeginSend();
        }
        public void SendPacket(Packet pPacket)
        {
            if (mDisconnected != 0) return;
            byte[] buffer = new byte[pPacket.Length + 4];
            mSendCrypto.GenerateHeader(buffer);
            Buffer.BlockCopy(pPacket.InnerBuffer, 0, buffer, 4, pPacket.Length);
            mSendCrypto.Encrypt(buffer, 4, pPacket.Length);
            Send(buffer);
        }

        private void BeginSend()
        {
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += (s, a) => EndSend(a);
            ByteArraySegment segment = mSendSegments.Next;
            args.SetBuffer(segment.Buffer, segment.Start, segment.Length);
            try { if (!mSocket.SendAsync(args)) EndSend(args); }
            catch (ObjectDisposedException) { }
        }
        private void EndSend(SocketAsyncEventArgs pArguments)
        {
            if (mDisconnected != 0) return;
            if (pArguments.BytesTransferred <= 0)
            {
                if (pArguments.SocketError != SocketError.Success) Log.WriteLine(ELogLevel.Error, "[{0}] Send Error: {1}", Host, pArguments.SocketError);
                Disconnect();
                return;
            }
            if (mSendSegments.Next.Advance(pArguments.BytesTransferred)) mSendSegments.Dequeue();
            if (mSendSegments.Next != null) BeginSend();
            else mSending = 0;
        }
    }
}
