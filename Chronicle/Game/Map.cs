using Chronicle.Data;
using Chronicle.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chronicle.Game
{
    public sealed class Map
    {
        private MapData mData = null;
        private List<Player> mPlayers = new List<Player>();
        private DateTime mLastPulse = DateTime.MinValue;
        private DateTime mLastMobControllersUpdate = DateTime.MinValue;

        private List<Portal> mPortals = new List<Portal>();
        private List<NPC> mNPCs = new List<NPC>();
        private List<Reactor> mReactors = new List<Reactor>();
        private List<Mob> mMobs = new List<Mob>();

        internal Map(MapData pData)
        {
            mData = pData;
            byte index = 0;
            mData.Portals.ForEach(p => mPortals.Add(new Portal(p, index++, Server.GetPortalScript(p.Name))));
            mData.NPCs.ForEach(n => mNPCs.Add(new NPC(n)));
            mData.Reactors.ForEach(r => mReactors.Add(new Reactor(r)));
            mData.Mobs.ForEach(m => mMobs.Add(new Mob(m)));
        }

        public MapData Data { get { return mData; } }

        public void AddPlayer(Player pPlayer) { mPlayers.Add(pPlayer); }
        public void RemovePlayer(Player pPlayer)
        {
            mPlayers.Remove(pPlayer);
            Player controller = null;
            if (mPlayers.Count > 0) controller = mPlayers[0];
            mMobs.ForEach(m => { if (m.Controller == pPlayer) m.AssignController(controller); });
            SendPlayerLeave(pPlayer);
        }

        internal void SendPacketToAllExcept(Packet pPacket, Player pExcept) { mPlayers.ForEach(p => { if (p != pExcept) p.SendPacket(pPacket); }); }
        internal void SendPacketToAll(Packet pPacket) { mPlayers.ForEach(p => p.SendPacket(pPacket)); }

        internal void SendPlayerDetails(Player pPlayer)
        {
            SendPacketToAllExcept(pPlayer.GetPlayerDetails(), pPlayer);
            mPlayers.ForEach(p => { if (p != pPlayer) pPlayer.SendPacket(p.GetPlayerDetails()); });
        }

        internal void SendPlayerLeave(Player pPlayer)
        {
            Packet packet = new Packet(EOpcode.SMSG_PLAYER_LEAVE);
            packet.WriteInt(pPlayer.Identifier);
            SendPacketToAll(packet);
        }

        internal Portal GetPortal(string pName) { return mPortals.Find(p => p.Data.Name == pName); }
        internal Mob GetMob(int pUniqueIdentifier) { return mMobs.Find(m => m.UniqueIdentifier == pUniqueIdentifier); }

        internal void SendNPCDetails(Player pPlayer)
        {
            int index = 0;
            foreach (NPC npc in mNPCs)
            {
                Packet packet = new Packet(EOpcode.SMSG_NPC_DETAILS);
                packet.WriteInt(index + 0x64);
                packet.WriteInt(npc.Data.NPCIdentifier);
                packet.WriteShort(npc.Data.X);
                packet.WriteShort(npc.Data.Y);
                packet.WriteBool((npc.Data.Flags & MapData.MapNPCData.EMapNPCFlags.FacesLeft) != MapData.MapNPCData.EMapNPCFlags.None);
                packet.WriteUShort(npc.Data.Foothold);
                packet.WriteShort(npc.Data.MinClickX);
                packet.WriteShort(npc.Data.MaxClickX);
                packet.WriteBool(true);
                pPlayer.SendPacket(packet);

                packet = new Packet(EOpcode.SMSG_NPC_CONTROL);
                packet.WriteBool(true);
                packet.WriteInt(index + 0x64);
                packet.WriteInt(npc.Data.NPCIdentifier);
                packet.WriteShort(npc.Data.X);
                packet.WriteShort(npc.Data.Y);
                packet.WriteBool((npc.Data.Flags & MapData.MapNPCData.EMapNPCFlags.FacesLeft) != MapData.MapNPCData.EMapNPCFlags.None);
                packet.WriteUShort(npc.Data.Foothold);
                packet.WriteShort(npc.Data.MinClickX);
                packet.WriteShort(npc.Data.MaxClickX);
                packet.WriteBool(true);
                pPlayer.SendPacket(packet);

                ++index;
            }
        }

        internal void SendReactorDetails(Player pPlayer)
        {
            int slot = 0;
            foreach (Reactor reactor in mReactors)
            {
                Packet packet = new Packet(EOpcode.SMSG_REACTOR_DETAILS);
                packet.WriteInt(slot++);
                packet.WriteInt(reactor.Data.ReactorIdentifier);
                packet.WriteByte(reactor.State);
                packet.WriteShort(reactor.Data.X);
                packet.WriteShort(reactor.Data.Y);
                packet.WriteByte(0x00);
                pPlayer.SendPacket(packet);
            }
        }

        internal void SendMobDetails(Player pPlayer)
        {
            foreach (Mob mob in mMobs)
            {
                Packet packet = new Packet(EOpcode.SMSG_MOB_DETAILS);
                packet.WriteInt(mob.UniqueIdentifier);
                packet.WriteByte(0x00);
                packet.WriteInt(mob.Data.MobIdentifier);
                mob.WriteStatus(packet);
                packet.WriteCoordinates(mob.Position);
                byte bits = 0x02;
                if (mob.FacingLeft) bits |= 0x01;
                packet.WriteByte(bits);
                packet.WriteUShort(mob.Foothold);
                packet.WriteUShort(mob.Data.Foothold);
                packet.WriteSByte(-1);
                packet.WriteSByte(0);
                packet.WriteInt(0);
                pPlayer.SendPacket(packet);

            }
        }

        internal void UpdateMobControllers(bool pFromMovement)
        {
            if (pFromMovement && DateTime.Now.Subtract(mLastMobControllersUpdate) < TimeSpan.FromSeconds(3)) return;
            foreach (Mob mob in mMobs)
            {
                Player controller = null;
                int closest = int.MaxValue;
                foreach (Player player in mPlayers)
                {
                    int distance = mob.Position - player.Position;
                    if (distance <= 700 && distance < closest && mMobs.Count(m => m.Controller == player) < 22)
                    {
                        controller = player;
                        distance = closest;
                    }
                }
                mob.AssignController(controller);
            }
            mLastMobControllersUpdate = DateTime.Now;
        }

        internal bool ReadMovement(IMoveable pMoveable, Packet pPacket)
        {
            byte movements;
            Coordinates position = null;
            ushort foothold = 0;
            byte stance = 0;
            if (!pPacket.ReadByte(out movements)) return false;
            while (movements-- > 0)
            {
                byte type;
                if (!pPacket.ReadByte(out type)) return false;
                switch (type)
                {
                    case 0:
                    case 5:
                        if (!pPacket.ReadCoordinates(out position) ||
                            !pPacket.ReadSkip(4) ||
                            !pPacket.ReadUShort(out foothold) ||
                            !pPacket.ReadByte(out stance) ||
                            !pPacket.ReadSkip(2)) return false;
                        break;
                    case 1:
                    case 2:
                    case 6:
                    case 13:
                        if (!pPacket.ReadCoordinates(out position) ||
                            !pPacket.ReadByte(out stance) ||
                            !pPacket.ReadUShort(out foothold)) return false;
                        break;
                    case 3:
                    case 4:
                    case 7:
                    case 8:
                    case 9:
                        if (!pPacket.ReadCoordinates(out position) ||
                            !pPacket.ReadSkip(4) ||
                            !pPacket.ReadByte(out stance)) return false;
                        break;
                    case 10:
                        if (!pPacket.ReadSkip(1)) return false;
                        break;
                    case 11:
                        if (!pPacket.ReadCoordinates(out position) ||
                            !pPacket.ReadUShort(out foothold) ||
                            !pPacket.ReadByte(out stance) ||
                            !pPacket.ReadSkip(2)) return false;
                        break;
                    case 12:
                    case 16:
                        if (!pPacket.ReadSkip(7)) return false;
                        break;
                    case 14:
                        if (!pPacket.ReadSkip(9)) return false;
                        break;
                    case 15:
                        if (!pPacket.ReadCoordinates(out position) ||
                            !pPacket.ReadSkip(6) ||
                            !pPacket.ReadUShort(out foothold) ||
                            !pPacket.ReadByte(out stance) ||
                            !pPacket.ReadSkip(2)) return false;
                        break;
                    case 17:
                        if (!pPacket.ReadCoordinates(out position) ||
                            !pPacket.ReadUShort(out foothold) ||
                            !pPacket.ReadByte(out stance) ||
                            !pPacket.ReadSkip(6)) return false;
                        break;
                    default: return false;
                }
            }

            if (position != null)
            {
                pMoveable.Position = position;
                pMoveable.Foothold = foothold;
                pMoveable.Stance = stance;
            }
            return true;
        }
    }
}
