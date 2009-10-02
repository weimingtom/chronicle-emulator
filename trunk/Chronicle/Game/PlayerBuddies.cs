using Chronicle.Enums;
using Chronicle.Network;
using Chronicle.Utility;
using System;
using System.Collections.Generic;

namespace Chronicle.Game
{
    public sealed class PlayerBuddies
    {
        private byte mMaxBuddies;
        private List<PlayerBuddy> mBuddies;

        internal PlayerBuddies(byte pMaxBuddies, DatabaseQuery pQuery)
        {
            mMaxBuddies = pMaxBuddies;
            mBuddies = new List<PlayerBuddy>(mMaxBuddies);
            while (pQuery.NextRow()) mBuddies.Add(new PlayerBuddy(pQuery));
        }

        public byte MaxBuddies { get { return mMaxBuddies; } set { mMaxBuddies = value; } }

        internal void WriteUpdate(Packet pPacket, EBuddyUpdateType pType)
        {
            pPacket.WriteByte((byte)pType);
            pPacket.WriteByte((byte)mBuddies.Count);
            foreach (PlayerBuddy buddy in mBuddies) buddy.WriteGeneral(pPacket);
            foreach (PlayerBuddy buddy in mBuddies) pPacket.WriteUInt(0);
        }
    }
}
