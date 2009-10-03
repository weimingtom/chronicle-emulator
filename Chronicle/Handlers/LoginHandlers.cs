using Chronicle.Data;
using Chronicle.Enums;
using Chronicle.Game;
using Chronicle.Network;
using Chronicle.Utility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Net;

namespace Chronicle.Handlers
{
    internal sealed class LoginHandlers
    {
        private enum EAuthenticationResult : uint
        {
            Ok = 0,
            Incorrect = 4,
            Invalid = 5,
            Online = 7
        }

        [PacketHandler(EOpcode.CMSG_AUTHENTICATION)]
        public static void Authentication(Client pClient, Packet pPacket)
        {
            string username;
            string password;
            byte[] macBytes = new byte[6];
            if (!pPacket.ReadString(out username) ||
                !pPacket.ReadString(out password) ||
                !pPacket.ReadBytes(macBytes))
            {
                pClient.Disconnect();
                return;
            }
            Account account = null;
            using (DatabaseQuery query = Database.Query("SELECT * FROM account WHERE username=@username", new MySqlParameter("@username", username)))
            {
                if (!query.NextRow())
                {
                    SendAuthentication(pClient, EAuthenticationResult.Invalid);
                    return;
                }
                account = new Account(query);
            }
            if (password != account.Password)
            {
                SendAuthentication(pClient, EAuthenticationResult.Incorrect);
                return;
            }
            if (Server.IsAccountOnline(account.Identifier) ||
                Server.IsPendingPlayerLogin(account.Identifier))
            {
                SendAuthentication(pClient, EAuthenticationResult.Online);
                return;
            }
            pClient.Account = account;
            Log.WriteLine(ELogLevel.Info, "[{0}] Authenticated {1}", pClient.Host, pClient.Account.Username);
            SendAuthentication(pClient, EAuthenticationResult.Ok);
        }
        private static void SendAuthentication(Client pClient, EAuthenticationResult pResult)
        {
            Packet packet = new Packet(EOpcode.SMSG_AUTHENTICATION);
            packet.WriteUInt((uint)pResult);
            packet.WriteSkip(2);
            if (pClient.Account != null)
            {
                packet.WriteInt(pClient.Account.Identifier);
                packet.WriteSkip(4);
                packet.WriteString(pClient.Account.Username);
                packet.WriteSkip(22);
            }
            pClient.SendPacket(packet);
        }

        [PacketHandler(EOpcode.CMSG_PIN)]
        public static void Pin(Client pClient, Packet pPacket)
        {
            SendPin(pClient);
        }
        private static void SendPin(Client pClient)
        {
            Packet packet = new Packet(EOpcode.SMSG_PIN);
            packet.WriteByte(0x00);
            pClient.SendPacket(packet);
        }

        [PacketHandler(EOpcode.CMSG_WORLD_LIST)]
        public static void WorldList(Client pClient, Packet pPacket)
        {
            SendWorldList(pClient);
        }
        private static void SendWorldList(Client pClient)
        {
            Packet packet = new Packet(EOpcode.SMSG_WORLD_LIST);
            packet.WriteByte(0);
            packet.WriteString("Scania");
            packet.WriteByte(Config.Instance.World.Ribbon);
            packet.WriteString(Config.Instance.World.EventMessage);
            packet.WriteByte(Config.Instance.World.ExperienceModifier);
            packet.WriteByte(0);
            packet.WriteByte(Config.Instance.World.DropModifier);
            packet.WriteByte(0);
            packet.WriteByte(0);

            packet.WriteByte(1);
            packet.WriteString("Scania-1");
            packet.WriteInt(Server.Population);
            packet.WriteByte(0);
            packet.WriteByte(0);
            packet.WriteByte(0);

            packet.WriteShort(0);
            pClient.SendPacket(packet);

            packet = new Packet(EOpcode.SMSG_WORLD_LIST);
            packet.WriteByte(0xFF);
            pClient.SendPacket(packet);
        }

        [PacketHandler(EOpcode.CMSG_ALL_PLAYER_LIST)]
        public static void AllPlayerList(Client pClient, Packet pPacket)
        {
            SendAllPlayerList(pClient);
        }
        private static void SendAllPlayerList(Client pClient)
        {
            Packet packet = new Packet(EOpcode.SMSG_ALL_PLAYER_LIST);
            packet.WriteByte(0x01);
            packet.WriteUInt(1);
            byte playerCount = (byte)(long)Database.Scalar("SELECT COUNT(*) FROM player WHERE account_identifier=@account_identifier", new MySqlParameter("@account_identifier", pClient.Account.Identifier));
            packet.WriteInt(playerCount);
            pClient.SendPacket(packet);

            packet = new Packet(EOpcode.SMSG_ALL_PLAYER_LIST);
            packet.WriteByte(0x00);
            packet.WriteByte(0x00);
            packet.WriteByte(playerCount);
            if (playerCount > 0)
            {
                using (DatabaseQuery query = Database.Query("SELECT * FROM player WHERE account_identifier=@account_identifier", new MySqlParameter("@account_identifier", pClient.Account.Identifier)))
                {
                    while (query.NextRow()) WritePlayer(packet, query);
                }
            }
            pClient.SendPacket(packet);
        }

        [PacketHandler(EOpcode.CMSG_WORLD_STATUS)]
        public static void WorldStatus(Client pClient, Packet pPacket)
        {
            SendWorldStatus(pClient);
        }
        private static void SendWorldStatus(Client pClient)
        {
            int population = Server.Population;
            Packet packet = new Packet(EOpcode.SMSG_WORLD_STATUS);
            if (population >= Config.Instance.Channel.MaxPopulation) packet.WriteByte(0x02);
            else if (population >= (Config.Instance.Channel.MaxPopulation * 0.9)) packet.WriteByte(0x01);
            else packet.WriteByte(0x00);
            packet.WriteByte(0);
            pClient.SendPacket(packet);
        }

        [PacketHandler(EOpcode.CMSG_PLAYER_LIST)]
        public static void PlayerList(Client pClient, Packet pPacket)
        {
            SendPlayerList(pClient);
        }
        private static void SendPlayerList(Client pClient)
        {
            Packet packet = new Packet(EOpcode.SMSG_PLAYER_LIST);
            packet.WriteByte(0x00);
            byte playerCount = (byte)(long)Database.Scalar("SELECT COUNT(*) FROM player WHERE account_identifier=@account_identifier", new MySqlParameter("@account_identifier", pClient.Account.Identifier));
            packet.WriteByte(playerCount);
            if (playerCount > 0)
            {
                using (DatabaseQuery query = Database.Query("SELECT * FROM player WHERE account_identifier=@account_identifier", new MySqlParameter("@account_identifier", pClient.Account.Identifier)))
                {
                    while (query.NextRow()) WritePlayer(packet, query);
                }
            }
            packet.WriteInt(Config.Instance.Login.MaxPlayersPerAccount);
            pClient.SendPacket(packet);
        }
        private static void WritePlayer(Packet pPacket, DatabaseQuery pQuery)
        {
            pPacket.WriteInt((int)pQuery["identifier"]);
            pPacket.WritePaddedString((string)pQuery["name"], 13);

            pPacket.WriteByte((byte)pQuery["gender"]);
            pPacket.WriteByte((byte)pQuery["skin"]);
            pPacket.WriteInt((int)pQuery["eyes_identifier"]);
            pPacket.WriteInt((int)pQuery["hair_identifier"]);
            pPacket.WriteSkip(24);
            pPacket.WriteByte((byte)pQuery["level"]);
            pPacket.WriteUShort((ushort)pQuery["job"]);
            pPacket.WriteUShort((ushort)pQuery["strength"]);
            pPacket.WriteUShort((ushort)pQuery["dexterity"]);
            pPacket.WriteUShort((ushort)pQuery["intellect"]);
            pPacket.WriteUShort((ushort)pQuery["luck"]);
            pPacket.WriteUShort((ushort)pQuery["health"]);
            pPacket.WriteUShort((ushort)pQuery["max_health"]);
            pPacket.WriteUShort((ushort)pQuery["mana"]);
            pPacket.WriteUShort((ushort)pQuery["max_mana"]);
            pPacket.WriteUShort((ushort)pQuery["ability_points"]);
            pPacket.WriteUShort((ushort)pQuery["skill_points"]);
            pPacket.WriteInt((int)pQuery["experience"]);
            pPacket.WriteUShort((ushort)pQuery["fame"]);
            pPacket.WriteSkip(4);
            pPacket.WriteInt((int)pQuery["map_identifier"]);
            pPacket.WriteByte((byte)pQuery["map_spawn"]);
            pPacket.WriteSkip(4);

            pPacket.WriteByte((byte)pQuery["gender"]);
            pPacket.WriteByte((byte)pQuery["skin"]);
            pPacket.WriteInt((int)pQuery["eyes_identifier"]);
            pPacket.WriteBool(true);
            pPacket.WriteInt((int)pQuery["hair_identifier"]);

            SortedDictionary<byte, Doublet<int, int>> equipment = new SortedDictionary<byte, Doublet<int, int>>();
            using (DatabaseQuery queryEquipment = Database.Query("SELECT inventory_slot,item_identifier FROM player_item WHERE player_identifier=@player_identifier AND inventory_type=0 AND inventory_slot<0", new MySqlParameter("@player_identifier", (int)pQuery["identifier"])))
            {
                while (queryEquipment.NextRow())
                {
                    short slot = (short)(-((short)queryEquipment["inventory_slot"]));
                    if (slot > 100) slot -= 100;
                    Doublet<int, int> pair = equipment.GetOrDefault((byte)slot, null);
                    if (pair == null)
                    {
                        pair = new Doublet<int, int>((int)queryEquipment["item_identifier"], 0);
                        equipment.Add((byte)slot, pair);
                    }
                    else if ((short)queryEquipment["inventory_slot"] < -100)
                    {
                        pair.Second = pair.First;
                        pair.First = (int)queryEquipment["item_identifier"];
                    }
                    else pair.Second = (int)queryEquipment["item_identifier"];
                }
            }
            foreach (KeyValuePair<byte, Doublet<int, int>> pair in equipment)
            {
                pPacket.WriteByte(pair.Key);
                if (pair.Key == 11 && pair.Value.Second > 0) pPacket.WriteInt(pair.Value.Second);
                else pPacket.WriteInt(pair.Value.First);
            }
            pPacket.WriteByte(0xFF);
            foreach (KeyValuePair<byte, Doublet<int, int>> pair in equipment)
            {
                if (pair.Key != 11 && pair.Value.Second > 0)
                {
                    pPacket.WriteByte(pair.Key);
                    pPacket.WriteInt(pair.Value.Second);
                }
            }
            pPacket.WriteByte(0xFF);
            Doublet<int, int> cashWeapon = equipment.GetOrDefault((byte)11, null);
            pPacket.WriteInt(cashWeapon == null ? 0 : cashWeapon.First);
            pPacket.WriteSkip(12);

            pPacket.WriteBool(false);
        }

        [PacketHandler(EOpcode.CMSG_WORLD_LIST_REFRESH)]
        public static void WorldListRefresh(Client pClient, Packet pPacket)
        {
            SendWorldList(pClient);
        }

        [PacketHandler(EOpcode.CMSG_ALL_PLAYER_CONNECT)]
        public static void AllPlayerConnect(Client pClient, Packet pPacket)
        {
            int identifier;
            if (!pPacket.ReadInt(out identifier) ||
                (long)Database.Scalar("SELECT COUNT(*) FROM player WHERE identifier=@identifier AND account_identifier=@account_identifier",
                                      new MySqlParameter("@identifier", identifier),
                                      new MySqlParameter("@account_identifier", pClient.Account.Identifier)) == 0)
            {
                pClient.Disconnect();
                return;
            }
            Server.RegisterPlayerLogin(pClient.Account.Identifier, identifier, pClient.Host);
            SendChannelConnect(pClient, identifier);
        }
        private static void SendChannelConnect(Client pClient, int pPlayerIdentifier)
        {
            Packet packet = new Packet(EOpcode.SMSG_CHANNEL_CONNECT);
            packet.WriteByte(0x00);
            packet.WriteByte(0x00);
            packet.WriteBytes(IPAddress.Parse(Config.Instance.Channel.ExternalAddress).GetAddressBytes());
            packet.WriteUShort(Config.Instance.Channel.Listener.Port);
            packet.WriteInt(pPlayerIdentifier);
            packet.WriteSkip(5);
            pClient.SendPacket(packet);
        }

        [PacketHandler(EOpcode.CMSG_PLAYER_NAME_CHECK)]
        public static void PlayerNameCheck(Client pClient, Packet pPacket)
        {
            string name;
            if (!pPacket.ReadString(out name))
            {
                pClient.Disconnect();
                return;
            }
            bool unusable = name.Length < 4 ||
                            name.Length > 16 ||
                            (long)Database.Scalar("SELECT COUNT(*) FROM player WHERE name=@name", new MySqlParameter("@name", name)) != 0;
            SendPlayerNameCheck(pClient, name, unusable);
        }
        private static void SendPlayerNameCheck(Client pClient, string pName, bool pUnusable)
        {
            Packet packet = new Packet(EOpcode.SMSG_PLAYER_NAME_CHECK);
            packet.WriteString(pName);
            packet.WriteBool(pUnusable);
            pClient.SendPacket(packet);
        }

        [PacketHandler(EOpcode.CMSG_PLAYER_CREATE)]
        public static void PlayerCreate(Client pClient, Packet pPacket)
        {
            string name;
            int eyesIdentifier;
            int hairIdentifier;
            int hairColor;
            int skin;
            int shirtIdentifier;
            int pantsIdentifier;
            int shoesIdentifier;
            int weaponIdentifier;
            byte gender;

            if (!pPacket.ReadString(out name) ||
                !pPacket.ReadInt(out eyesIdentifier) ||
                !pPacket.ReadInt(out hairIdentifier) ||
                !pPacket.ReadInt(out hairColor) ||
                !pPacket.ReadInt(out skin) ||
                !pPacket.ReadInt(out shirtIdentifier) ||
                !pPacket.ReadInt(out pantsIdentifier) ||
                !pPacket.ReadInt(out shoesIdentifier) ||
                !pPacket.ReadInt(out weaponIdentifier) ||
                !pPacket.ReadByte(out gender))
            {
                pClient.Disconnect();
                return;
            }
            bool unusable = name.Length < 4 ||
                            name.Length > 16 ||
                            (long)Database.Scalar("SELECT COUNT(*) FROM player WHERE name=@name", new MySqlParameter("@name", name)) != 0;
            if (unusable)
            {
                SendPlayerNameCheck(pClient, name, true);
                return;
            }
            hairIdentifier += hairColor;
            int identifier = Database.InsertAndReturnIdentifier("INSERT INTO player(account_identifier,name,gender,skin,eyes_identifier,hair_identifier) " +
                                                                "VALUES(@account_identifier,@name,@gender,@skin,@eyes_identifier,@hair_identifier)",
                                                                new MySqlParameter("@account_identifier", pClient.Account.Identifier),
                                                                new MySqlParameter("@name", name),
                                                                new MySqlParameter("@gender", gender),
                                                                new MySqlParameter("@skin", skin),
                                                                new MySqlParameter("@eyes_identifier", eyesIdentifier),
                                                                new MySqlParameter("@hair_identifier", hairIdentifier));
            Database.Execute("INSERT INTO player_keymap(player_identifier) VALUES(@player_identifier)", new MySqlParameter("@player_identifier", identifier));
            Database.Execute("INSERT INTO player_item(player_identifier,inventory_type,inventory_slot,item_identifier,weapon_defense) " +
                             "VALUES(@player_identifier,@inventory_type,-5,@item_identifier,3)",
                             new MySqlParameter("@player_identifier", identifier),
                             new MySqlParameter("@inventory_type", EInventoryType.Equipment),
                             new MySqlParameter("@item_identifier", shirtIdentifier));
            Database.Execute("INSERT INTO player_item(player_identifier,inventory_type,inventory_slot,item_identifier,weapon_defense) " +
                             "VALUES(@player_identifier,@inventory_type,-6,@item_identifier,2)",
                             new MySqlParameter("@player_identifier", identifier),
                             new MySqlParameter("@inventory_type", EInventoryType.Equipment),
                             new MySqlParameter("@item_identifier", pantsIdentifier));
            Database.Execute("INSERT INTO player_item(player_identifier,inventory_type,inventory_slot,item_identifier,unused_scroll_slots,weapon_defense) " +
                             "VALUES(@player_identifier,@inventory_type,-7,@item_identifier,5,3)",
                             new MySqlParameter("@player_identifier", identifier),
                             new MySqlParameter("@inventory_type", EInventoryType.Equipment),
                             new MySqlParameter("@item_identifier", shoesIdentifier));
            Database.Execute("INSERT INTO player_item(player_identifier,inventory_type,inventory_slot,item_identifier,weapon_attack) " +
                             "VALUES(@player_identifier,@inventory_type,-11,@item_identifier,17)",
                             new MySqlParameter("@player_identifier", identifier),
                             new MySqlParameter("@inventory_type", EInventoryType.Equipment),
                             new MySqlParameter("@item_identifier", weaponIdentifier));
            Database.Execute("INSERT INTO player_item(player_identifier,inventory_type,inventory_slot,item_identifier) VALUES(@player_identifier,@inventory_type,1,4161001)",
                             new MySqlParameter("@player_identifier", identifier),
                             new MySqlParameter("@inventory_type", EInventoryType.Etc));
            using (DatabaseQuery query = Database.Query("SELECT * FROM player WHERE identifier=@identifier", new MySqlParameter("@identifier", identifier)))
            {
                query.NextRow();
                SendPlayerCreate(pClient, query);
            }
        }
        private static void SendPlayerCreate(Client pClient, DatabaseQuery pQuery)
        {
            Packet packet = new Packet(EOpcode.SMSG_PLAYER_CREATE);
            packet.WriteByte(0x00);
            WritePlayer(packet, pQuery);
            pClient.SendPacket(packet);
        }

        [PacketHandler(EOpcode.CMSG_PLAYER_DELETE)]
        public static void PlayerDelete(Client pClient, Packet pPacket)
        {
            int identifier;
            if (!pPacket.ReadSkip(4) ||
                !pPacket.ReadInt(out identifier) ||
                (long)Database.Scalar("SELECT COUNT(*) FROM player WHERE identifier=@identifier AND account_identifier=@account_identifier",
                                      new MySqlParameter("@identifier", identifier),
                                      new MySqlParameter("@account_identifier", pClient.Account.Identifier)) == 0)
            {
                pClient.Disconnect();
                return;
            }
            Database.Execute("DELETE FROM player WHERE identifier=@identifier", new MySqlParameter("@identifier", identifier));
            SendPlayerDelete(pClient, identifier);
        }
        private static void SendPlayerDelete(Client pClient, int pPlayerIdentifier)
        {
            Packet packet = new Packet(EOpcode.SMSG_PLAYER_DELETE);
            packet.WriteInt(pPlayerIdentifier);
            packet.WriteByte(0x00);
            pClient.SendPacket(packet);
        }

        [PacketHandler(EOpcode.CMSG_PLAYER_CONNECT)]
        public static void PlayerConnect(Client pClient, Packet pPacket)
        {
            int identifier;
            if (!pPacket.ReadInt(out identifier) ||
                (long)Database.Scalar("SELECT COUNT(*) FROM player WHERE identifier=@identifier AND account_identifier=@account_identifier",
                                      new MySqlParameter("@identifier", identifier),
                                      new MySqlParameter("@account_identifier", pClient.Account.Identifier)) == 0)
            {
                pClient.Disconnect();
                return;
            }
            Server.RegisterPlayerLogin(pClient.Account.Identifier, identifier, pClient.Host);
            SendChannelConnect(pClient, identifier);
        }

        [PacketHandler(EOpcode.CMSG_DISCONNECT)]
        public static void Disconnect(Client pClient, Packet pPacket)
        {
            if (pClient.Account == null) pClient.Disconnect();
        }
    }
}
