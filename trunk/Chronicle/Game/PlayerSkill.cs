using Chronicle.Data;
using Chronicle.Network;
using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerSkill
    {
        private int mPlayerIdentifier;
        private int mSkillIdentifier;
        private byte mLevel;
        private byte mMaxLevel;
        private ushort mCooldown;

        internal PlayerSkill(DatabaseQuery pQuery)
        {
            mPlayerIdentifier = (int)pQuery["player_identifier"];
            mSkillIdentifier = (int)pQuery["skill_identifier"];
            mLevel = (byte)pQuery["level"];
            mMaxLevel = (byte)pQuery["max_level"];
            mCooldown = (ushort)pQuery["cooldown"];
        }

        public int PlayerIdentifier { get { return mPlayerIdentifier; } set { mPlayerIdentifier = value; } }
        public int SkillIdentifier { get { return mSkillIdentifier; } set { mSkillIdentifier = value; } }
        public byte Level { get { return mLevel; } set { mLevel = value; } }
        public byte MaxLevel { get { return mMaxLevel; } set { mMaxLevel = value; } }
        public ushort Cooldown { get { return mCooldown; } set { mCooldown = value; } }

        internal void WriteGeneral(Packet pPacket)
        {
            pPacket.WriteInt(mSkillIdentifier);
            pPacket.WriteUInt(mLevel);
            if (SkillData.IsFourthJobRelated(mSkillIdentifier)) pPacket.WriteUInt(mMaxLevel);
        }
    }
}
