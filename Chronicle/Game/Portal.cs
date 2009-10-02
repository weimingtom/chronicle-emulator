using Chronicle.Data;
using Chronicle.Script;
using System;

namespace Chronicle.Game
{
    public sealed class Portal
    {
        private MapData.MapPortalData mData = null;
        private PortalScript mScript = null;

        internal Portal(MapData.MapPortalData pData, PortalScript pScript)
        {
            mData = pData;
            mScript = pScript;
        }

        public MapData.MapPortalData Data { get { return mData; } }
        public PortalScript Script { get { return mScript; } }
    }
}
