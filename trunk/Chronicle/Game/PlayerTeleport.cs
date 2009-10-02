using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerTeleport
    {
        private int mPlayerIdentifier;
        private int mMapIdentifier;
        private byte mSlot;
        private bool mVIP;

        internal PlayerTeleport(DatabaseQuery pQuery)
        {
            mPlayerIdentifier = (int)pQuery["player_identifier"];
            mMapIdentifier = (int)pQuery["map_identifier"];
            mSlot = (byte)pQuery["slot"];
            mVIP = (bool)pQuery["vip"];
        }

        public int PlayerIdentifier { get { return mPlayerIdentifier; } set { mPlayerIdentifier = value; } }
        public int MapIdentifier { get { return mMapIdentifier; } set { mMapIdentifier = value; } }
        public byte Slot { get { return mSlot; } set { mSlot = value; } }
        public bool VIP { get { return mVIP; } set { mVIP = value; } }
    }
}
