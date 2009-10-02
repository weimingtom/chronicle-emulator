using Chronicle.Data;
using Chronicle.Network;
using System;

namespace Chronicle.Game
{
    public sealed class Mob : IMoveable
    {
        private static int sUniqueCounter = 0;

        private MapData.MapMobData mData = null;

        private int mUniqueIdentifier = 0;
        private Player mController = null;
        private byte mStance = 0;
        private ushort mFoothold = 0;
        private Coordinates mPosition = new Coordinates(0, 0);
        private int mHealth = 0;
        private int mMaxHealth = 0;
        private int mMana = 0;
        private int mMaxMana = 0;

        internal Mob(MapData.MapMobData pData)
        {
            mData = pData;
            mUniqueIdentifier = ++sUniqueCounter;
            mStance = (byte)((mData.Flags & MapData.MapMobData.EMapMobFlags.FacesLeft) != MapData.MapMobData.EMapMobFlags.None ? 0 : 1);
            mFoothold = mData.Foothold;
            mPosition = new Coordinates(mData.X, mData.Y);

            MobData mobData = Server.GetMobData(mData.MobIdentifier);
            mHealth = mMaxHealth = mobData.HP;
            mMana = mMaxMana = mobData.MP;
        }

        public MapData.MapMobData Data { get { return mData; } }
        public int UniqueIdentifier { get { return mUniqueIdentifier; } }
        public Player Controller { get { return mController; } }
        public bool FacingLeft { get { return mStance % 2 == 0; } }
        public byte Stance { get { return mStance; } set { mStance = value; } }
        public ushort Foothold { get { return mFoothold; } set { mFoothold = value; } }
        public Coordinates Position { get { return mPosition; } set { mPosition = value; } }
        public int Health { get { return mHealth; } set { mHealth = value; } }
        public int MaxHealth { get { return mMaxHealth; } set { mMaxHealth = value; } }
        public int Mana { get { return mMana; } set { mMana = value; } }
        public int MaxMana { get { return mMaxMana; } set { mMaxMana = value; } }

        internal void AssignController(Player pPlayer)
        {
            if (mController != null) SendControl(false);
            mController = pPlayer;
            SendControl(true);
        }

        internal void SendControl(bool pTakeControl)
        {
            if (mController != null)
            {
                Packet packet = new Packet(EOpcode.SMSG_MOB_CONTROL);
                packet.WriteBool(pTakeControl);
                packet.WriteInt(UniqueIdentifier);
                if (pTakeControl)
                {
                    packet.WriteByte(0x05);
                    packet.WriteInt(mData.MobIdentifier);
                    WriteStatus(packet);
                    packet.WriteCoordinates(mPosition);
                    packet.WriteByte((byte)(0x02 | (FacingLeft ? 0x01 : 0x00)));
                    packet.WriteUShort(mFoothold);
                    packet.WriteUShort(mData.Foothold);
                    packet.WriteBool(false);
                    packet.WriteByte(0xFF);
                    packet.WriteInt(0);
                }
                mController.SendPacket(packet);
            }
        }

        internal void WriteStatus(Packet pPacket)
        {
            pPacket.WriteInt(0);
        }
    }
}
