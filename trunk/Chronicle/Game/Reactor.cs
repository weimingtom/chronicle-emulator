using Chronicle.Data;
using System;

namespace Chronicle.Game
{
    public sealed class Reactor
    {
        private MapData.MapReactorData mData = null;

        private byte mState = 0;

        internal Reactor(MapData.MapReactorData pData)
        {
            mData = pData;
        }

        public MapData.MapReactorData Data { get { return mData; } }
        public byte State { get { return mState; } set { mState = value; } }
    }
}
