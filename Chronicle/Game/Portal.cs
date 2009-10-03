using Chronicle.Data;
using Chronicle.Script;
using System;

namespace Chronicle.Game
{
    public sealed class Portal
    {
        private MapData.MapPortalData mData = null;
        private byte mIndex = 0;
        private PortalScript mScript = null;

        internal Portal(MapData.MapPortalData pData, byte pIndex, PortalScript pScript)
        {
            mData = pData;
            mIndex = pIndex;
            mScript = pScript;
        }

        public MapData.MapPortalData Data { get { return mData; } }
        public byte Index { get { return mIndex; } }
        public PortalScript Script { get { return mScript; } }
    }
}
