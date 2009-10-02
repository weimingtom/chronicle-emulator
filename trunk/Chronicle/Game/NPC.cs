using Chronicle.Data;
using System;

namespace Chronicle.Game
{
    public sealed class NPC
    {
        private MapData.MapNPCData mData = null;

        internal NPC(MapData.MapNPCData pData)
        {
            mData = pData;
        }

        public MapData.MapNPCData Data { get { return mData; } }
    }
}
