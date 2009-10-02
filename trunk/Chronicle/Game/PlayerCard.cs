using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerCard
    {
        private int mPlayerIdentifier;
        private int mCardIdentifier;
        private byte mLevel;

        internal PlayerCard(DatabaseQuery pQuery)
        {
            mPlayerIdentifier = (int)pQuery["player_identifier"];
            mCardIdentifier = (int)pQuery["card_identifier"];
            mLevel = (byte)pQuery["level"];
        }

        public int PlayerIdentifier { get { return mPlayerIdentifier; } set { mPlayerIdentifier = value; } }
        public int CardIdentifier { get { return mCardIdentifier; } set { mCardIdentifier = value; } }
        public byte Level { get { return mLevel; } set { mLevel = value; } }
    }
}
