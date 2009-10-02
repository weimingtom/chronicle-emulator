using Chronicle.Data;
using Chronicle.Enums;
using Chronicle.Network;
using Chronicle.Utility;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Chronicle.Game
{
    public sealed class Player : IMoveable
    {
        private Client mClient;
        private Random mRandom;

        private int mIdentifier;
        private string mName;
        private byte mGender;
        private byte mSkin;
        private int mEyesIdentifier;
        private int mHairIdentifier;
        private byte mLevel;
        private ushort mJob;
        private ushort mStrength;
        private ushort mDexterity;
        private ushort mIntellect;
        private ushort mLuck;
        private ushort mHealth;
        private ushort mMaxHealth;
        private ushort mMana;
        private ushort mMaxMana;
        private ushort mAbilityPoints;
        private ushort mSkillPoints;
        private int mExperience;
        private ushort mFame;
        private Map mMap;
        private byte mSpawn;
        private byte mStance = 0;
        private ushort mFoothold = 0;
        private byte mFallCount = 0;
        private Coordinates mPosition;
        private PlayerItems mItems;
        private PlayerSkills mSkills;
        private PlayerQuests mQuests;
        private PlayerTeleports mTeleports;
        private PlayerCards mCards;
        private PlayerMacros mMacros;
        private PlayerKeymap mKeymap;
        private PlayerBuddies mBuddies;
        private PlayerBuffs mBuffs = new PlayerBuffs();

        public Player(Client pClient, DatabaseQuery pQuery)
        {
            mClient = pClient;
            mRandom = new Random();

            mIdentifier = (int)pQuery["identifier"];
            mName = (string)pQuery["name"];
            mGender = (byte)pQuery["gender"];
            mSkin = (byte)pQuery["skin"];
            mEyesIdentifier = (int)pQuery["eyes_identifier"];
            mHairIdentifier = (int)pQuery["hair_identifier"];
            mLevel = (byte)pQuery["level"];
            mJob = (ushort)pQuery["job"];
            mStrength = (ushort)pQuery["strength"];
            mDexterity = (ushort)pQuery["dexterity"];
            mIntellect = (ushort)pQuery["intellect"];
            mLuck = (ushort)pQuery["luck"];
            mHealth = (ushort)pQuery["health"];
            mMaxHealth = (ushort)pQuery["max_health"];
            mMana = (ushort)pQuery["mana"];
            mMaxMana = (ushort)pQuery["max_mana"];
            mAbilityPoints = (ushort)pQuery["ability_points"];
            mSkillPoints = (ushort)pQuery["skill_points"];
            mExperience = (int)pQuery["experience"];
            mFame = (ushort)pQuery["fame"];

            int mapIdentifier = (int)pQuery["map_identifier"];
            MapData mapData = Server.GetMapData(mapIdentifier);
            byte spawn = (byte)pQuery["map_spawn"];
            if (mapData.ForcedReturnMapIdentifier != MapData.INVALID_MAP_IDENTIFIER)
            {
                mapData = Server.GetMapData(mapData.ForcedReturnMapIdentifier);
                spawn = 0;
                if (mHealth == 0) mHealth = 50;
            }
            else if (mHealth == 0)
            {
                mapData = Server.GetMapData(mapData.ReturnMapIdentifier);
                spawn = 0;
                mHealth = 50;
            }
            mMap = Server.GetActiveMap(mapData.Identifier);
            mSpawn = spawn;
            mPosition = new Coordinates(mapData.Portals[spawn].X, mapData.Portals[spawn].Y);

            using (DatabaseQuery query = Database.Query("SELECT * FROM player_item WHERE player_identifier=@player_identifier", new MySqlParameter("@player_identifier", mIdentifier)))
            {
                byte[] slots = new byte[(byte)EInventoryType.Count];
                slots[(byte)EInventoryType.Equipment] = (byte)pQuery["equipment_slots"];
                slots[(byte)EInventoryType.Use] = (byte)pQuery["use_slots"];
                slots[(byte)EInventoryType.Setup] = (byte)pQuery["setup_slots"];
                slots[(byte)EInventoryType.Etc] = (byte)pQuery["etc_slots"];
                slots[(byte)EInventoryType.Cash] = (byte)pQuery["cash_slots"];
                mItems = new PlayerItems((int)pQuery["mesos"], slots, query);
            }
            using (DatabaseQuery query = Database.Query("SELECT * FROM player_skill WHERE player_identifier=@player_identifier", new MySqlParameter("@player_identifier", mIdentifier)))
            {
                mSkills = new PlayerSkills(query);
            }
            using (DatabaseQuery query = Database.Query("SELECT * FROM player_quest WHERE player_identifier=@player_identifier", new MySqlParameter("@player_identifier", mIdentifier)))
            {
                mQuests = new PlayerQuests(query);
            }
            using (DatabaseQuery query = Database.Query("SELECT * FROM player_teleport WHERE player_identifier=@player_identifier", new MySqlParameter("@player_identifier", mIdentifier)))
            {
                mTeleports = new PlayerTeleports(query);
            }
            using (DatabaseQuery query = Database.Query("SELECT * FROM player_card WHERE player_identifier=@player_identifier", new MySqlParameter("@player_identifier", mIdentifier)))
            {
                mCards = new PlayerCards(query);
            }
            using (DatabaseQuery query = Database.Query("SELECT * FROM player_macro WHERE player_identifier=@player_identifier", new MySqlParameter("@player_identifier", mIdentifier)))
            {
                mMacros = new PlayerMacros(query);
            }
            using (DatabaseQuery query = Database.Query("SELECT * FROM player_keymap WHERE player_identifier=@player_identifier", new MySqlParameter("@player_identifier", mIdentifier)))
            {
                query.NextRow();
                mKeymap = new PlayerKeymap(query);
            }
            using (DatabaseQuery query = Database.Query("SELECT * FROM player_buddy WHERE player_identifier=@player_identifier", new MySqlParameter("@player_identifier", mIdentifier)))
            {
                mBuddies = new PlayerBuddies((byte)pQuery["buddy_slots"], query);
            }
        }

        public int Identifier { get { return mIdentifier; } }
        public string Name { get { return mName; } }
        public byte Gender { get { return mGender; } set { mGender = value; } }
        public byte Skin { get { return mSkin; } set { mSkin = value; } }
        public int EyesIdentifier { get { return mEyesIdentifier; } set { mEyesIdentifier = value; } }
        public int HairIdentifier { get { return mHairIdentifier; } set { mHairIdentifier = value; } }
        public byte Level { get { return mLevel; } set { mLevel = value; } }
        public ushort Job { get { return mJob; } set { mJob = value; } }
        public ushort Strength { get { return mStrength; } set { mStrength = value; } }
        public ushort Dexterity { get { return mDexterity; } set { mDexterity = value; } }
        public ushort Intellect { get { return mIntellect; } set { mIntellect = value; } }
        public ushort Luck { get { return mLuck; } set { mLuck = value; } }
        public ushort Health { get { return mHealth; } set { mHealth = value; } }
        public ushort MaxHealth { get { return mMaxHealth; } set { mMaxHealth = value; } }
        public ushort Mana { get { return mMana; } set { mMana = value; } }
        public ushort MaxMana { get { return mMaxMana; } set { mMaxMana = value; } }
        public ushort AbilityPoints { get { return mAbilityPoints; } set { mAbilityPoints = value; } }
        public ushort SkillPoints { get { return mSkillPoints; } set { mSkillPoints = value; } }
        public int Experience { get { return mExperience; } set { mExperience = value; } }
        public ushort Fame { get { return mFame; } set { mFame = value; } }
        public Map Map { get { return mMap; } set { mMap = value; } }
        public byte Spawn { get { return mSpawn; } set { mSpawn = value; } }
        public bool FacingLeft { get { return mStance % 2 == 0; } }
        public byte Stance { get { return mStance; } set { mStance = value; } }
        public ushort Foothold { get { return mFoothold; } set { mFoothold = value; } }
        public byte FallCount { get { return mFallCount; } set { mFallCount = value; } }
        public Coordinates Position { get { return mPosition; } set { mPosition = value; } }
        public PlayerItems Items { get { return mItems; } }
        public PlayerSkills Skills { get { return mSkills; } }
        public PlayerQuests Quests { get { return mQuests; } }
        public PlayerTeleports Teleports { get { return mTeleports; } }
        public PlayerCards Cards { get { return mCards; } }
        public PlayerMacros Macros { get { return mMacros; } }
        public PlayerKeymap Keymap { get { return mKeymap; } }
        public PlayerBuddies Buddies { get { return mBuddies; } }
        public PlayerBuffs Buffs { get { return mBuffs; } }

        internal void SendPacket(Packet pPacket) { mClient.SendPacket(pPacket); }

        internal void SendInitialMapChange()
        {
            Packet packet = new Packet(EOpcode.SMSG_MAP_CHANGE);
            packet.WriteUInt(0);
            packet.WriteByte(0x01);
            packet.WriteByte(0x01);
            packet.WriteUShort(0);
            packet.WriteInt(mRandom.Next());
            packet.WriteInt(mRandom.Next());
            packet.WriteInt(mRandom.Next());
            packet.WriteInt(-1);
            packet.WriteInt(-1);
            packet.WriteInt(mIdentifier);
            packet.WritePaddedString(mName, 13);
            packet.WriteByte(mGender);
            packet.WriteByte(mSkin);
            packet.WriteInt(mEyesIdentifier);
            packet.WriteInt(mHairIdentifier);
            packet.WriteSkip(24);
            packet.WriteByte(mLevel);
            packet.WriteUShort(mJob);
            packet.WriteUShort(mStrength);
            packet.WriteUShort(mDexterity);
            packet.WriteUShort(mIntellect);
            packet.WriteUShort(mLuck);
            packet.WriteUShort(mHealth);
            packet.WriteUShort(mMaxHealth);
            packet.WriteUShort(mMana);
            packet.WriteUShort(mMaxMana);
            packet.WriteUShort(mAbilityPoints);
            packet.WriteUShort(mSkillPoints);
            packet.WriteInt(mExperience);
            packet.WriteUShort(mFame);
            packet.WriteUInt(0);
            packet.WriteInt(mMap.Data.Identifier);
            packet.WriteByte(mSpawn);
            packet.WriteUInt(0);
            packet.WriteByte(mBuddies.MaxBuddies);
            mItems.WriteInitial(packet);
            mSkills.WriteInitial(packet);
            mQuests.WriteInitial(packet);
            packet.WriteUInt(0);
            packet.WriteUInt(0);
            mTeleports.WriteInitial(packet);
            mCards.WriteInitial(packet);
            packet.WriteUInt(0);
            packet.WriteUShort(0);
            packet.WriteLong(DateTime.Now.ToBinary());
            mClient.SendPacket(packet);
        }

        internal void SendKeymap()
        {
            Packet packet = new Packet(EOpcode.SMSG_KEYMAP);
            packet.WriteByte(0x00);
            mKeymap.WriteInitial(packet);
            mClient.SendPacket(packet);
        }

        internal void SendBuddyUpdate(EBuddyUpdateType pType)
        {
            Packet packet = new Packet(EOpcode.SMSG_BUDDY_UPDATE);
            mBuddies.WriteUpdate(packet, pType);
            mClient.SendPacket(packet);
        }

        internal void SendMacroList()
        {
            Packet packet = new Packet(EOpcode.SMSG_MACRO_LIST);
            mMacros.WriteInitial(packet);
            mClient.SendPacket(packet);
        }

        internal Packet GetPlayerDetails()
        {
            Packet packet = new Packet(EOpcode.SMSG_PLAYER_DETAILS);
            packet.WriteInt(mIdentifier);
            packet.WriteString(mName);

            packet.WriteString("");
            packet.WriteUShort(0);
            packet.WriteByte(0x00);
            packet.WriteUShort(0);
            packet.WriteByte(0x00);

            packet.WriteUInt(0);
            packet.WriteByte(0xF8);
            packet.WriteByte(0x03);
            packet.WriteUShort(0);

            byte[] types = mBuffs.MapTypes;
            packet.WriteBytes(types, PlayerBuffs.BUFF_BYTE_5, 4);
            packet.WriteBytes(types, PlayerBuffs.BUFF_BYTE_1, 4);
            for (byte index = 0; index < PlayerBuffs.BUFF_ENTRY_ORDER.Length; ++index)
            {
                byte entryIndex = PlayerBuffs.BUFF_ENTRY_ORDER[index];
                if (types[entryIndex] != 0)
                {
                    foreach (KeyValuePair<byte, PlayerBuffs.MapValue> kv in mBuffs.MapValues[entryIndex])
                    {
                        if (kv.Value.Debuff)
                        {
                            if (!(kv.Key == 0x01 && entryIndex == PlayerBuffs.BUFF_BYTE_5))
                            {
                                packet.WriteUShort(kv.Value.SkillIdentifier);
                                packet.WriteShort(kv.Value.Value);
                            }
                        }
                        else if (kv.Value.Use)
                        {
                            if (entryIndex == PlayerBuffs.BUFF_BYTE_3)
                            {
                                if (kv.Key == 0x20) packet.WriteByte((byte)(mBuffs.Combo + 1));
                                else if (kv.Key == 0x40) packet.WriteInt(mBuffs.ChargeSkillIdentifier);
                            }
                            else if (entryIndex == PlayerBuffs.BUFF_BYTE_5) packet.WriteShort(kv.Value.Value);
                            else packet.WriteByte((byte)kv.Value.Value);
                        }
                    }
                }
            }
            packet.WriteUShort(0);

            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(1065638850);
            packet.WriteUShort(0);
            packet.WriteByte(0x00);

            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(1065638850);
            packet.WriteUShort(0);
            packet.WriteByte(0x00);

            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(1065638850);
            packet.WriteUShort(0);
            packet.WriteByte(0x00);

            packet.WriteInt(mBuffs.MountIdentifier);
            packet.WriteInt(mBuffs.MountSkillIdentifier);
            packet.WriteUInt(1065638850);
            packet.WriteByte(0x00);

            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(1065638850);
            packet.WriteUInt(0);
            packet.WriteUInt(0);

            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(1065638850);
            packet.WriteUInt(0);
            packet.WriteByte(0x00);

            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(1065638850);
            packet.WriteUShort(0);
            packet.WriteByte(0x00);

            packet.WriteUShort(mJob);
            WriteAppearance(packet);

            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteCoordinates(mPosition);
            packet.WriteByte(0x00);
            packet.WriteUShort(0);
            packet.WriteByte(0x00);

            packet.WriteByte(0x00);
            packet.WriteUShort(1);
            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUInt(0);
            packet.WriteUShort(0);

            return packet;
        }

        internal void WriteAppearance(Packet pPacket)
        {
            pPacket.WriteByte(mGender);
            pPacket.WriteByte(mSkin);
            pPacket.WriteInt(mEyesIdentifier);
            pPacket.WriteBool(true);
            pPacket.WriteInt(mHairIdentifier);
            mItems.WriteEquipment(pPacket);
            pPacket.WriteUInt(0);
            pPacket.WriteUInt(0);
            pPacket.WriteUInt(0);
        }

        internal void EnterMap()
        {
            if (mMap.Data.Music.Length > 0)
            {
                Packet packet = new Packet(EOpcode.SMSG_MAP_EFFECT);
                packet.WriteByte(0x06);
                packet.WriteString(mMap.Data.Music);
                mClient.SendPacket(packet);
            }
            if (mMap.Data.FieldTypes == MapData.EMapFieldType.ForceMapEquip) mClient.SendPacket(new Packet(EOpcode.SMSG_MAP_FORCE_EQUIPMENT));

            mMap.SendPlayerDetails(this);
            mMap.SendNPCDetails(this);
            mMap.SendReactorDetails(this);
            mMap.SendMobDetails(this);
        }

        public void SendMessage(EMessageType pType, string pMessage, params object[] pArgs)
        {
            Packet packet = new Packet(EOpcode.SMSG_MESSAGE);
            packet.WriteByte((byte)pType);
            packet.WriteString(string.Format(pMessage, pArgs));
            if (pType == EMessageType.GMText) packet.WriteInt(0);
            mClient.SendPacket(packet);
        }

        public void SendPortalBlocked()
        {
            Packet packet = new Packet(EOpcode.SMSG_PLAYER_UPDATE);
            packet.WriteByte(0x01);
            packet.WriteUInt(0);
            SendPacket(packet);
        }
    }
}
