using Chronicle.Utility;

namespace Chronicle.Script.Default
{
    public static class Initialization
    {
        [Initializer(1)]
        public static bool Initialize()
        {
            if (!InitializePortals()) return false;
            Log.WriteLine(ELogLevel.Info, "[Chronicle.Script.Default] Initialized");
            return true;
        }

        private static bool InitializePortals()
        {
            PortalScript.Register("tutorial8", typeof(Portals.tutorial8));
            return true;
        }
    }
}
