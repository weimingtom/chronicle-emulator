using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerMacro
    {
        private int mPlayerIdentifier;
        private byte mSlot;
        private string mName;
        private bool mShout;
        private int mFirstSkillIdentifier;
        private int mSecondSkillIdentifier;
        private int mThirdSkillIdentifier;

        internal PlayerMacro(DatabaseQuery pQuery)
        {
            mPlayerIdentifier = (int)pQuery["player_identifier"];
            mSlot = (byte)pQuery["slot"];
            mName = (string)pQuery["name"];
            mShout = (bool)pQuery["shout"];
            mFirstSkillIdentifier = (int)pQuery["first_skill_identifier"];
            mSecondSkillIdentifier = (int)pQuery["second_skill_identifier"];
            mThirdSkillIdentifier = (int)pQuery["third_skill_identifier"];
        }

        public int PlayerIdentifier { get { return mPlayerIdentifier; } set { mPlayerIdentifier = value; } }
        public byte Slot { get { return mSlot; } set { mSlot = value; } }
        public string Name { get { return mName; } set { mName = value; } }
        public bool Shout { get { return mShout; } set { mShout = value; } }
        public int FirstSkillIdentifier { get { return mFirstSkillIdentifier; } set { mFirstSkillIdentifier = value; } }
        public int SecondSkillIdentifier { get { return mSecondSkillIdentifier; } set { mSecondSkillIdentifier = value; } }
        public int ThirdSkillIdentifier { get { return mThirdSkillIdentifier; } set { mThirdSkillIdentifier = value; } }
    }
}
