using Chronicle.Data;
using Chronicle.Enums;
using Chronicle.Game;
using Chronicle.Network;
using Chronicle.Script;
using Chronicle.Utility;
using System;
using System.Collections.Generic;

namespace Chronicle.Handlers
{
    internal sealed class NPCHandlers
    {
        [PacketHandler(EOpcode.CMSG_NPC_ACTION)]
        public static void Action(Client pClient, Packet pPacket)
        {
            int firstUnknown;
            short secondUnknown;
            if (!pPacket.ReadInt(out firstUnknown) ||
                !pPacket.ReadShort(out secondUnknown))
            {
                pClient.Disconnect();
                return;
            }
            Packet packet = new Packet(EOpcode.SMSG_NPC_ACTION);
            byte thirdUnknown;
            if (!pPacket.ReadByte(out thirdUnknown))
            {
                packet.WriteInt(firstUnknown);
                packet.WriteShort(secondUnknown);
            }
            else packet.WriteBytes(pPacket.InnerBuffer, pPacket.Cursor, pPacket.Remaining);
            pClient.SendPacket(packet);
        }
    }
}
