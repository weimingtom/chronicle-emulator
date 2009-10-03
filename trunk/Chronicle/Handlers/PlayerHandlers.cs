﻿using Chronicle.Data;
using Chronicle.Enums;
using Chronicle.Game;
using Chronicle.Network;
using Chronicle.Utility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Chronicle.Handlers
{
    internal sealed class PlayerHandlers
    {
        [PacketHandler(EOpcode.CMSG_PLAYER_LOAD)]
        public static void Load(Client pClient, Packet pPacket)
        {
            int accountIdentifier;
            int playerIdentifier;
            if (!pPacket.ReadInt(out playerIdentifier) ||
                (accountIdentifier = Server.ValidatePlayerLogin(playerIdentifier, pClient.Host)) == 0)
            {
                pClient.Disconnect();
                return;
            }
            using (DatabaseQuery query = Database.Query("SELECT * FROM account WHERE identifier=@identifier", new MySqlParameter("@identifier", accountIdentifier)))
            {
                query.NextRow();
                pClient.Account = new Account(query);
            }
            using (DatabaseQuery query = Database.Query("SELECT * FROM player WHERE identifier=@identifier", new MySqlParameter("@identifier", playerIdentifier)))
            {
                query.NextRow();
                pClient.Account.Player = new Player(pClient, query);
            }
            pClient.Account.Player.Map.AddPlayer(pClient.Account.Player);
            Server.UnregisterPlayerLogin(playerIdentifier);
            Log.WriteLine(ELogLevel.Info, "[{0}] Loaded {1}", pClient.Host, pClient.Account.Player.Name);

            pClient.Account.Player.SendInitialMapChange();
            pClient.Account.Player.SendKeymap();
            pClient.Account.Player.SendBuddyUpdate(EBuddyUpdateType.Add);
            pClient.Account.Player.SendMacroList();
            pClient.Account.Player.SendMessage(EMessageType.ErrorText, "Welcome to Chronicle {0}", Server.Version);
            pClient.Account.Player.EnterMap();
        }

        [PacketHandler(EOpcode.CMSG_PLAYER_TELEPORT)]
        public static void Teleport(Client pClient, Packet pPacket)
        {
            int mapIdentifier;
            if (!pPacket.ReadSkip(1) ||
                !pPacket.ReadInt(out mapIdentifier))
            {
                pClient.Disconnect();
                return;
            }
            if (mapIdentifier == -1)
            {
                string portalName;
                if (!pPacket.ReadString(out portalName))
                {
                    pClient.Disconnect();
                    return;
                }
                Portal portal = pClient.Account.Player.Map.GetPortal(portalName);
                if (portal == null)
                {
                    Log.WriteLine(ELogLevel.Debug, "[{0}] Portal Blocked {1}", pClient.Host, portalName);
                    pClient.Account.Player.SendPortalBlocked();
                    return;
                }
                Map mapDestination = Server.GetActiveMap(portal.Data.ToMapIdentifier);
                if (mapDestination == null)
                {
                    Log.WriteLine(ELogLevel.Debug, "[{0}] Portal Blocked {1}", pClient.Host, portalName);
                    pClient.Account.Player.SendPortalBlocked();
                    return;
                }
                Portal portalDestination = mapDestination.GetPortal(portal.Data.ToName);
                if (portalDestination == null)
                {
                    Log.WriteLine(ELogLevel.Debug, "[{0}] Portal Blocked {1}", pClient.Host, portalName);
                    pClient.Account.Player.SendPortalBlocked();
                    return;
                }

                Log.WriteLine(ELogLevel.Info, "[{0}] Portal Triggered {1}", pClient.Host, portal.Data.Name);
                pClient.Account.Player.Map.RemovePlayer(pClient.Account.Player);
                pClient.Account.Player.Map = mapDestination;
                pClient.Account.Player.Map.AddPlayer(pClient.Account.Player);
                pClient.Account.Player.Spawn = portalDestination.Index;
                pClient.Account.Player.Position.X = portalDestination.Data.X;
                pClient.Account.Player.Position.Y = portalDestination.Data.Y;
                pClient.Account.Player.Stance = 0;
                pClient.Account.Player.Foothold = 0;
                pClient.Account.Player.FallCount = 0;
                pClient.Account.Player.SendMapChange();
                pClient.Account.Player.EnterMap();
            }
        }

        [PacketHandler(EOpcode.CMSG_PLAYER_MOVE)]
        public static void Move(Client pClient, Packet pPacket)
        {
            if (!pPacket.ReadSkip(9))
            {
                pClient.Disconnect();
                return;
            }
            int rewindOffset = pPacket.Cursor;
            pClient.Account.Player.Map.ReadMovement(pClient.Account.Player, pPacket);
            pPacket.Rewind(rewindOffset);

            Packet packet = new Packet(EOpcode.SMSG_PLAYER_MOVE);
            packet.WriteInt(pClient.Account.Player.Identifier);
            packet.WriteInt(0);
            packet.WriteBytes(pPacket.InnerBuffer, pPacket.Cursor, pPacket.Remaining);
            pClient.Account.Player.Map.SendPacketToAllExcept(packet, pClient.Account.Player);

            if (pClient.Account.Player.Foothold == 0)
            {
            }
            else pClient.Account.Player.FallCount = 0;

            pClient.Account.Player.Map.UpdateMobControllers(true);
        }

        [PacketHandler(EOpcode.CMSG_PLAYER_CHAT)]
        public static void Chat(Client pClient, Packet pPacket)
        {
            string message;
            bool bubble;
            if (!pPacket.ReadString(out message) ||
                !pPacket.ReadBool(out bubble))
            {
                pClient.Disconnect();
                return;
            }

            Packet packet = new Packet(EOpcode.SMSG_PLAYER_CHAT);
            packet.WriteInt(pClient.Account.Player.Identifier);
            packet.WriteBool(pClient.Account.Level > 0);
            packet.WriteString(message);
            packet.WriteBool(bubble);
            pClient.Account.Player.Map.SendPacketToAll(packet);
        }

        [PacketHandler(EOpcode.CMSG_PLAYER_EMOTE)]
        public static void Emote(Client pClient, Packet pPacket)
        {
            int emote;
            if (!pPacket.ReadInt(out emote))
            {
                pClient.Disconnect();
                return;
            }

            Packet packet = new Packet(EOpcode.SMSG_PLAYER_EMOTE);
            packet.WriteInt(pClient.Account.Player.Identifier);
            packet.WriteInt(emote);
            pClient.Account.Player.Map.SendPacketToAllExcept(packet, pClient.Account.Player);
        }
    }
}
