using Chronicle.Game;

namespace Chronicle.Script.Default.Portals
{
    public class tutorial8 : PortalScript
    {
        protected override void Execute(Player pPlayer, Portal pPortal)
        {
            pPlayer.SendPortalBlocked();
        }
    }
}
