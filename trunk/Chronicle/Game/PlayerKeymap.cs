using Chronicle.Network;
using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerKeymap
    {
        public const byte MAX_KEYS = 90;

        private byte[] mTypes = new byte[MAX_KEYS];
        private uint[] mActions = new uint[MAX_KEYS];

        internal PlayerKeymap(DatabaseQuery pQuery)
        {
            for (byte index = 0; index < MAX_KEYS; ++index)
            {
                mTypes[index] = (byte)pQuery["type_" + index];
                mActions[index] = (uint)pQuery["action_" + index];
            }
        }

        internal void WriteInitial(Packet pPacket)
        {
            for (byte index = 0; index < MAX_KEYS; ++index)
            {
                pPacket.WriteByte(mTypes[index]);
                pPacket.WriteUInt(mActions[index]);
            }
        }
    }
}
