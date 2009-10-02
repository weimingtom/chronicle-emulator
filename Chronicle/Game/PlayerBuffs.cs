using Chronicle.Utility;
using System;
using System.Collections.Generic;

namespace Chronicle.Game
{
    public sealed class PlayerBuffs
    {
        internal const byte BUFF_BYTES = 16;
        internal const byte BUFF_ENTRY_BYTES = 8;

        internal const byte BUFF_BYTE_1 = 12;
        internal const byte BUFF_BYTE_2 = 13;
        internal const byte BUFF_BYTE_3 = 14;
        internal const byte BUFF_BYTE_4 = 15;
        internal const byte BUFF_BYTE_5 = 8;
        internal const byte BUFF_BYTE_6 = 9;
        internal const byte BUFF_BYTE_7 = 10;
        internal const byte BUFF_BYTE_8 = 11;
        internal const byte BUFF_BYTE_9 = 4;
        internal const byte BUFF_BYTE_10 = 5;
        internal const byte BUFF_BYTE_11 = 6;
        internal const byte BUFF_BYTE_12 = 7;
        internal const byte BUFF_BYTE_13 = 0;
        internal const byte BUFF_BYTE_14 = 1;
        internal const byte BUFF_BYTE_15 = 2;
        internal const byte BUFF_BYTE_16 = 3;

        internal static byte[] BUFF_ENTRY_ORDER = new byte[] { BUFF_BYTE_1, BUFF_BYTE_2, BUFF_BYTE_3, BUFF_BYTE_4,
                                                               BUFF_BYTE_5, BUFF_BYTE_6, BUFF_BYTE_7, BUFF_BYTE_8 };

        public sealed class MapValue
        {
            private bool mUse = false;
            private bool mDebuff = false;
            private short mValue = 0;
            private ushort mSkillIdentifier = 0;

            public bool Use { get { return mUse; } set { mUse = value; } }
            public bool Debuff { get { return mDebuff; } set { mDebuff = value; } }
            public short Value { get { return mValue; } set { mValue = value; } }
            public ushort SkillIdentifier { get { return mSkillIdentifier; } set { mSkillIdentifier = value; } }
        }

        private byte mCombo = 0;
        private ushort mEnergy = 0;
        private int mChargeSkillIdentifier = 0;
        private int mBoosterSkillIdentifier = 0;
        private uint mDebuffs = 0;
        private bool mBerserk = false;
        private Dictionary<byte, Dictionary<byte, int>> mByType = new Dictionary<byte, Dictionary<byte, int>>();
        private byte[] mMapTypes = new byte[BUFF_BYTES];
        private Dictionary<byte, Dictionary<byte, MapValue>> mMapValues = new Dictionary<byte, Dictionary<byte, MapValue>>();
        private int mMountIdentifier = 0;
        private int mMountSkillIdentifier = 0;
        private Dictionary<int, byte> mLevels = new Dictionary<int, byte>();

        public byte Combo { get { return mCombo; } set { mCombo = value; } }
        public ushort Energy { get { return mEnergy; } set { mEnergy = value; } }
        public int ChargeSkillIdentifier { get { return mChargeSkillIdentifier; } set { mChargeSkillIdentifier = value; } }
        public int BoosterSkillIdentifier { get { return mBoosterSkillIdentifier; } set { mBoosterSkillIdentifier = value; } }
        public uint Debuffs { get { return mDebuffs; } set { mDebuffs = value; } }
        public bool Berserk { get { return mBerserk; } set { mBerserk = value; } }
        public Dictionary<byte, Dictionary<byte, int>> ByType { get { return mByType; } }
        public byte[] MapTypes { get { return mMapTypes; } }
        public Dictionary<byte, Dictionary<byte, MapValue>> MapValues { get { return mMapValues; } }
        public int MountIdentifier { get { return mMountIdentifier; } set { mMountIdentifier = value; } }
        public int MountSkillIdentifier { get { return mMountSkillIdentifier; } set { mMountSkillIdentifier = value; } }
        public Dictionary<int, byte> Levels { get { return mLevels; } }
    }
}
