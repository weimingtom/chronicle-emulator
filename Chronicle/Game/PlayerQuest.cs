using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerQuest
    {
        private int mPlayerIdentifier;
        private ushort mQuestIdentifier;
        private int mMobIdentifier;
        private ushort mMobKills;
        private string mState;
        private long mCompleted;

        internal PlayerQuest(DatabaseQuery pQuery)
        {
            mPlayerIdentifier = (int)pQuery["player_identifier"];
            mQuestIdentifier = (ushort)pQuery["quest_identifier"];
            mMobIdentifier = (int)pQuery["mob_identifier"];
            mMobKills = (ushort)pQuery["mob_kills"];
            mState = (string)pQuery["state"];
            mCompleted = (long)pQuery["completed"];
        }

        public int PlayerIdentifier { get { return mPlayerIdentifier; } set { mPlayerIdentifier = value; } }
        public ushort QuestIdentifier { get { return mQuestIdentifier; } set { mQuestIdentifier = value; } }
        public int MobIdentifier { get { return mMobIdentifier; } set { mMobIdentifier = value; } }
        public ushort MobKills { get { return mMobKills; } set { mMobKills = value; } }
        public string State { get { return mState; } set { mState = value; } }
        public long Completed { get { return mCompleted; } set { mCompleted = value; } }
    }
}
