using Chronicle.Data;
using Chronicle.Network;
using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerTeleports
    {
        public const byte MAX_TELEPORTS = 5;
        public const byte MAX_VIP_TELEPORTS = 10;

        private PlayerTeleport[] mTeleports;

        internal PlayerTeleports(DatabaseQuery pQuery)
        {
            mTeleports = new PlayerTeleport[MAX_TELEPORTS + MAX_VIP_TELEPORTS];
            while (pQuery.NextRow())
            {
                PlayerTeleport teleport = new PlayerTeleport(pQuery);
                mTeleports[teleport.Slot] = teleport;
            }
        }

        internal void WriteInitial(Packet pPacket)
        {
            Array.ForEach(mTeleports, t => pPacket.WriteInt(t == null ? MapData.INVALID_MAP_IDENTIFIER : t.MapIdentifier));
        }
    }
}
