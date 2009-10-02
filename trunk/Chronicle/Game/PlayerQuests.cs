using Chronicle.Network;
using Chronicle.Utility;
using System;
using System.Collections.Generic;

namespace Chronicle.Game
{
    public sealed class PlayerQuests
    {
        private Dictionary<int, PlayerQuest> mQuests = new Dictionary<int, PlayerQuest>();

        internal PlayerQuests(DatabaseQuery pQuery)
        {
            while (pQuery.NextRow())
            {
                PlayerQuest quest = new PlayerQuest(pQuery);
                mQuests.Add(quest.QuestIdentifier, quest);
            }
        }

        internal void WriteInitial(Packet pPacket)
        {
            List<PlayerQuest> active = new List<PlayerQuest>(mQuests.Values);
            active.RemoveAll(q => q.Completed != 0);
            pPacket.WriteUShort((ushort)active.Count);
            foreach (PlayerQuest quest in active)
            {
                pPacket.WriteUShort(quest.QuestIdentifier);
                pPacket.WriteString(quest.State);
            }
            List<PlayerQuest> completed = new List<PlayerQuest>(mQuests.Values);
            completed.RemoveAll(q => q.Completed == 0);
            pPacket.WriteUShort((ushort)completed.Count);
            foreach (PlayerQuest quest in completed)
            {
                pPacket.WriteInt(quest.QuestIdentifier);
                pPacket.WriteLong(quest.Completed);
            }
        }
    }
}
