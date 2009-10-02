using Chronicle.Network;
using Chronicle.Utility;
using System;
using System.Collections.Generic;

namespace Chronicle.Game
{
    public sealed class PlayerMacros
    {
        private PlayerMacro[] mMacros;

        internal PlayerMacros(DatabaseQuery pQuery)
        {
            mMacros = new PlayerMacro[0];
            while (pQuery.NextRow())
            {
                PlayerMacro macro = new PlayerMacro(pQuery);
                if (mMacros.Length <= macro.Slot) Array.Resize(ref mMacros, macro.Slot + 1);
                mMacros[macro.Slot] = macro;
            }
        }

        internal void WriteInitial(Packet pPacket)
        {
            pPacket.WriteByte((byte)mMacros.Length);
            Array.ForEach(mMacros, m =>
            {
                if (m != null)
                {
                    pPacket.WriteString(m.Name);
                    pPacket.WriteBool(m.Shout);
                    pPacket.WriteInt(m.FirstSkillIdentifier);
                    pPacket.WriteInt(m.SecondSkillIdentifier);
                    pPacket.WriteInt(m.ThirdSkillIdentifier);
                }
                else
                {
                    pPacket.WriteString("");
                    pPacket.WriteBool(false);
                    pPacket.WriteInt(0);
                    pPacket.WriteInt(0);
                    pPacket.WriteInt(0);
                }
            });
        }
    }
}
