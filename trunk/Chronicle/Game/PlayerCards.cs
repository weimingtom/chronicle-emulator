using Chronicle.Data;
using Chronicle.Network;
using Chronicle.Utility;
using System;
using System.Collections.Generic;

namespace Chronicle.Game
{
    public sealed class PlayerCards
    {
        private Dictionary<int, PlayerCard> mCards = new Dictionary<int, PlayerCard>();

        internal PlayerCards(DatabaseQuery pQuery)
        {
            while (pQuery.NextRow())
            {
                PlayerCard card = new PlayerCard(pQuery);
                mCards.Add(card.CardIdentifier, card);
            }
        }

        internal void WriteInitial(Packet pPacket)
        {
            pPacket.WriteUInt(0);
            pPacket.WriteByte(0x00);
            pPacket.WriteUShort((ushort)mCards.Count);
            foreach (PlayerCard card in mCards.Values)
            {
                pPacket.WriteUShort(ItemData.ReduceCardIdentifier(card.CardIdentifier));
                pPacket.WriteByte(card.Level);
            }
        }
    }
}
