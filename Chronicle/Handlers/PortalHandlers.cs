using Chronicle.Data;
using Chronicle.Enums;
using Chronicle.Game;
using Chronicle.Network;
using Chronicle.Script;
using Chronicle.Utility;
using System;
using System.Collections.Generic;

namespace Chronicle.Handlers
{
    internal sealed class PortalHandlers
    {
        [PacketHandler(EOpcode.CMSG_PORTAL_SCRIPT_TRIGGER)]
        public static void ScriptTrigger(Client pClient, Packet pPacket)
        {
            string name;
            if (!pPacket.ReadSkip(1) ||
                !pPacket.ReadString(out name))
            {
                pClient.Disconnect();
                return;
            }
            Portal portal = pClient.Account.Player.Map.GetPortal(name);
            if (portal == null || portal.Script == null)
            {
                Log.WriteLine(ELogLevel.Debug, "[{0}] Portal Script Blocked {1}", pClient.Host, name);
                pClient.Account.Player.SendPortalBlocked();
                return;
            }

            Log.WriteLine(ELogLevel.Info, "[{0}] Portal Script Triggered {1}", pClient.Host, portal.Script.GetType().FullName);
            portal.Script.Execute(pClient.Account.Player, portal);
        }
    }
}
