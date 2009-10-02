using Chronicle.Data;
using Chronicle.Network;
using Chronicle.Utility;
using System;
using System.Collections.Generic;

namespace Chronicle.Game
{
    public sealed class PlayerSkills
    {
        private Dictionary<int, PlayerSkill> mSkills = new Dictionary<int, PlayerSkill>();

        internal PlayerSkills(DatabaseQuery pQuery)
        {
            while (pQuery.NextRow())
            {
                PlayerSkill skill = new PlayerSkill(pQuery);
                mSkills.Add(skill.SkillIdentifier, skill);
            }
        }

        internal void WriteInitial(Packet pPacket)
        {
            pPacket.WriteUShort((ushort)mSkills.Count);
            foreach (PlayerSkill skill in mSkills.Values) skill.WriteGeneral(pPacket);
            List<PlayerSkill> cooldowns = new List<PlayerSkill>(mSkills.Values);
            cooldowns.RemoveAll(s => s.Cooldown == 0);
            pPacket.WriteUShort((ushort)cooldowns.Count);
            foreach (PlayerSkill cooldown in cooldowns)
            {
                pPacket.WriteInt(cooldown.SkillIdentifier);
                pPacket.WriteUShort(cooldown.Cooldown);
            }
        }
    }
}
