using Chronicle.Network;
using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerBuddy
    {
        private int mPlayerIdentifier;
        private int mBuddyIdentifier;
        private string mName;
        private byte mStatus;

        internal PlayerBuddy(DatabaseQuery pQuery)
        {
            mPlayerIdentifier = (int)pQuery["player_identifier"];
            mBuddyIdentifier = (int)pQuery["buddy_identifier"];
            mName = (string)pQuery["name"];
            mStatus = (byte)pQuery["status"];
        }

        public int PlayerIdentifier { get { return mPlayerIdentifier; } set { mPlayerIdentifier = value; } }
        public int BuddyIdentifier { get { return mBuddyIdentifier; } set { mBuddyIdentifier = value; } }
        public string Name { get { return mName; } set { mName = value; } }
        public byte Status { get { return mStatus; } set { mStatus = value; } }

        internal void WriteGeneral(Packet pPacket)
        {
            pPacket.WriteInt(mBuddyIdentifier);
            pPacket.WritePaddedString(mName, 13);
            pPacket.WriteByte(mStatus);
            pPacket.WriteInt(0);
            pPacket.WritePaddedString("Default Group", 17);
        }
    }
}
