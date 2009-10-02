using Chronicle.Game;
using System;

namespace Chronicle.Script
{
    public abstract class PortalScript
    {
        public static void Register(string pName, Type pType) { if (pType.IsSubclassOf(typeof(PortalScript))) Server.RegisterPortalScript(pName, pType); }

        protected internal abstract void Execute(Player pPlayer, Portal pPortal);
    }
}
