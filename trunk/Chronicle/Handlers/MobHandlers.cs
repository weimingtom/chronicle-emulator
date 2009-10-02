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
    internal sealed class MobHandlers
    {
        [PacketHandler(EOpcode.CMSG_MOB_ACTION)]
        public static void Action(Client pClient, Packet pPacket)
        {
            int uniqueIdentifier;
            short moveIdentifier;
            bool isUsingAbility;
            byte usingAbility;
            Coordinates projectileTarget;

            if (!pPacket.ReadInt(out uniqueIdentifier) ||
                !pPacket.ReadShort(out moveIdentifier) ||
                !pPacket.ReadBool(out isUsingAbility) ||
                !pPacket.ReadByte(out usingAbility) ||
                !pPacket.ReadCoordinates(out projectileTarget) ||
                !pPacket.ReadSkip(5))
            {
                pClient.Disconnect();
                return;
            }
            Mob mob = pClient.Account.Player.Map.GetMob(uniqueIdentifier);
            if (mob == null || mob.Controller != pClient.Account.Player) return;
            int rewindOffset = pPacket.Cursor;
            Coordinates unknownPosition;
            if (!pPacket.ReadCoordinates(out unknownPosition) ||
                !pClient.Account.Player.Map.ReadMovement(mob, pPacket))
            {
                pClient.Disconnect();
                return;
            }

            Packet packet = new Packet(EOpcode.SMSG_MOB_ACTION_CONFIRM);
            packet.WriteInt(uniqueIdentifier);
            packet.WriteShort(moveIdentifier);
            packet.WriteBool(isUsingAbility);
            packet.WriteUShort((ushort)mob.Mana);
            packet.WriteByte(0x00); // Ability Identifier
            packet.WriteByte(0x00); // Ability Level
            pClient.SendPacket(packet);

            pPacket.Rewind(rewindOffset);

            packet = new Packet(EOpcode.SMSG_MOB_ACTION);
            packet.WriteInt(uniqueIdentifier);
            packet.WriteBool(isUsingAbility);
            packet.WriteByte(usingAbility);
            packet.WriteCoordinates(projectileTarget);
            packet.WriteBytes(pPacket.InnerBuffer, pPacket.Cursor, pPacket.Remaining);
            pClient.Account.Player.Map.SendPacketToAllExcept(packet, pClient.Account.Player);
        }
    }
}
