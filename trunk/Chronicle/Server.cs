using Chronicle.Data;
using Chronicle.Game;
using Chronicle.Network;
using Chronicle.Script;
using Chronicle.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Text;

namespace Chronicle
{
    internal static class Server
    {
        private static LockFreeQueue<Callback> sCallbacks = new LockFreeQueue<Callback>();
        private static List<PendingPlayerLogin> sPendingPlayerLoginRegistry = new List<PendingPlayerLogin>();

        private static Socket sLoginListener;
        private static Socket sChannelListener;
        private static List<Client> sClients = new List<Client>();

        private static Dictionary<byte, Dictionary<byte, AbilityData>> sAbilities = new Dictionary<byte,Dictionary<byte,AbilityData>>();
        private static Dictionary<int, Dictionary<byte, SkillData>> sSkills = new Dictionary<int, Dictionary<byte, SkillData>>();
        private static Dictionary<int, NPCData> sNPCs = new Dictionary<int, NPCData>();
        private static Dictionary<int, ReactorData> sReactors = new Dictionary<int, ReactorData>();
        private static Dictionary<int, MobData> sMobs = new Dictionary<int, MobData>();
        private static Dictionary<ushort, QuestData> sQuests = new Dictionary<ushort, QuestData>();
        private static Dictionary<int, ItemData> sItems = new Dictionary<int, ItemData>();
        private static Dictionary<int, MapData> sMaps = new Dictionary<int, MapData>();

        private static Dictionary<int, Map> sActiveMaps = new Dictionary<int, Map>();
        private static Dictionary<string, Type> sPortalScripts = new Dictionary<string, Type>();


        private static void Main()
        {
            Console.Title = "Chronicle " + Version;
            Console.SetWindowSize(128, 64);
            Config.Load();
            if (!Initialize()) return;
            TimeSpan pendingPlayerLoginDelay = TimeSpan.FromSeconds(Config.Instance.World.PendingPlayerLoginDelay);
            Callback callback;
            while (true)
            {
                while (sCallbacks.Dequeue(out callback)) callback();
                sPendingPlayerLoginRegistry.RemoveAll(p => DateTime.Now.Subtract(p.Started) >= pendingPlayerLoginDelay);
                Thread.Sleep(1);
            }
        }

        internal static string Version { get { return Assembly.GetEntryAssembly().GetName().Version.ToString(); } }
        public static void AddCallback(Callback pCallback) { sCallbacks.Enqueue(pCallback); }

        private static bool Initialize()
        {
            int count;
            using (BinaryReader reader = new BinaryReader(new FileStream(Config.Instance.Binary, FileMode.Open, FileAccess.Read), Encoding.ASCII))
            {
                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    AbilityData ability = new AbilityData();
                    ability.Load(reader);
                    Dictionary<byte, AbilityData> levels = sAbilities.GetOrDefault(ability.Identifier, null);
                    if (levels == null)
                    {
                        levels = new Dictionary<byte,AbilityData>();
                        sAbilities.Add(ability.Identifier, levels);
                    }
                    levels.Add(ability.Level, ability);
                }

                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    SkillData skill = new SkillData();
                    skill.Load(reader);
                    Dictionary<byte, SkillData> levels = sSkills.GetOrDefault(skill.Identifier, null);
                    if (levels == null)
                    {
                        levels = new Dictionary<byte, SkillData>();
                        sSkills.Add(skill.Identifier, levels);
                    }
                    levels.Add(skill.Level, skill);
                }

                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    NPCData npc = new NPCData();
                    npc.Load(reader);
                    sNPCs.Add(npc.Identifier, npc);
                }

                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    ReactorData reactor = new ReactorData();
                    reactor.Load(reader);
                    sReactors.Add(reactor.Identifier, reactor);
                }

                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    MobData mob = new MobData();
                    mob.Load(reader);
                    sMobs.Add(mob.Identifier, mob);
                }

                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    QuestData quest = new QuestData();
                    quest.Load(reader);
                    sQuests.Add(quest.Identifier, quest);
                }

                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    ItemData item = new ItemData();
                    item.Load(reader);
                    sItems.Add(item.Identifier, item);
                }

                count = reader.ReadInt32();
                while (count-- > 0)
                {
                    MapData map = new MapData();
                    map.Load(reader);
                    sMaps.Add(map.Identifier, map);
                }
            }
            Log.WriteLine(ELogLevel.Info, "[Server] Initialized Data");

            foreach (string scriptPath in Config.Instance.Scripts)
            {
                if (!File.Exists(scriptPath)) continue;
                try { Assembly.LoadFile(Path.GetFullPath(scriptPath)); }
                catch { return false; }
            }
            List<Doublet<InitializerAttribute, InitializerCallback>> initializers = Reflector.FindAllMethods<InitializerAttribute, InitializerCallback>();
            initializers.Sort((p1, p2) => p1.First.Stage.CompareTo(p2.First.Stage));
            if (!initializers.TrueForAll(p => p.Second())) return false;

            sLoginListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sLoginListener.Bind(new IPEndPoint(IPAddress.Any, Config.Instance.Login.Listener.Port));
            sLoginListener.Listen(Config.Instance.Login.Listener.Backlog);
            Log.WriteLine(ELogLevel.Info, "[Server] Initialized Login Listener");

            sChannelListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sChannelListener.Bind(new IPEndPoint(IPAddress.Any, Config.Instance.Channel.Listener.Port));
            sChannelListener.Listen(Config.Instance.Channel.Listener.Backlog);
            Log.WriteLine(ELogLevel.Info, "[Server] Initialized Channel Listener");

            BeginLoginListenerAccept(null);
            BeginChannelListenerAccept(null);

            return true;
        }

        public static AbilityData GetAbilityData(byte pAbilityIdentifier, byte pLevel)
        {
            Dictionary<byte, AbilityData> levels = sAbilities.GetOrDefault(pAbilityIdentifier, null);
            if (levels == null) return null;
            return levels.GetOrDefault(pLevel, null);
        }
        public static SkillData GetSkillData(int pSkillIdentifier, byte pLevel)
        {
            Dictionary<byte, SkillData> levels = sSkills.GetOrDefault(pSkillIdentifier, null);
            if (levels == null) return null;
            return levels.GetOrDefault(pLevel, null);
        }
        public static NPCData GetNPCData(int pNPCIdentifier) { return sNPCs.GetOrDefault(pNPCIdentifier, null); }
        public static ReactorData GetReactorData(int pReactorIdentifier) { return sReactors.GetOrDefault(pReactorIdentifier, null); }
        public static MobData GetMobData(int pMobIdentifier) { return sMobs.GetOrDefault(pMobIdentifier, null); }
        public static QuestData GetQuestData(ushort pQuestIdentifier) { return sQuests.GetOrDefault(pQuestIdentifier, null); }
        public static ItemData GetItemData(int pItemIdentifier) { return sItems.GetOrDefault(pItemIdentifier, null); }
        public static MapData GetMapData(int pMapIdentifier) { return sMaps.GetOrDefault(pMapIdentifier, null); }
        public static Map GetActiveMap(int pMapIdentifier)
        {
            Map map = sActiveMaps.GetOrDefault(pMapIdentifier, null);
            if (map == null)
            {
                MapData mapData = GetMapData(pMapIdentifier);
                if (mapData != null)
                {
                    map = new Map(mapData);
                    sActiveMaps.Add(pMapIdentifier, map);
                }
            }
            return map;
        }

        internal static bool IsAccountOnline(int pAccountIdentifier) { lock (sClients) return sClients.Exists(c => c.Account != null && c.Account.Identifier == pAccountIdentifier); }
        internal static void ClientDisconnected(Client pClient) { lock (sClients) sClients.Remove(pClient); }

        private static void BeginLoginListenerAccept(SocketAsyncEventArgs pArgs)
        {
            if (pArgs == null)
            {
                pArgs = new SocketAsyncEventArgs();
                pArgs.Completed += (s, a) => EndLoginListenerAccept(a);
            }
            pArgs.AcceptSocket = null;
            if (!sLoginListener.AcceptAsync(pArgs)) EndLoginListenerAccept(pArgs);
        }
        private static void EndLoginListenerAccept(SocketAsyncEventArgs pArguments)
        {
            try
            {
                if (pArguments.SocketError == SocketError.Success)
                {
                    Client client = new Client(pArguments.AcceptSocket);
                    byte[] receiveIV = new byte[] { 0x46, 0x72, 0x7A, 0x52 }; // TODO: Randomize
                    byte[] sendIV = new byte[] { 0x52, 0x30, 0x78, 0x73 }; // TODO: Randomize
                    lock (sClients) sClients.Add(client);
                    client.SendHandshake(Config.Instance.Build, receiveIV, sendIV);
                    BeginLoginListenerAccept(pArguments);
                }
                else if (pArguments.SocketError != SocketError.OperationAborted) Log.WriteLine(ELogLevel.Error, "[Server] LoginListener Error: {0}", pArguments.SocketError);
            }
            catch (ObjectDisposedException) { }
            catch (Exception exc) { Log.WriteLine(ELogLevel.Exception, "[Server] LoginListener Exception: {0}", exc.Message); }
        }

        private static void BeginChannelListenerAccept(SocketAsyncEventArgs pArgs)
        {
            if (pArgs == null)
            {
                pArgs = new SocketAsyncEventArgs();
                pArgs.Completed += (s, a) => EndChannelListenerAccept(a);
            }
            pArgs.AcceptSocket = null;
            if (!sChannelListener.AcceptAsync(pArgs)) EndChannelListenerAccept(pArgs);
        }
        private static void EndChannelListenerAccept(SocketAsyncEventArgs pArguments)
        {
            try
            {
                if (pArguments.SocketError == SocketError.Success)
                {
                    Client client = new Client(pArguments.AcceptSocket);
                    byte[] receiveIV = new byte[] { 0x46, 0x72, 0x7A, 0x52 }; // TODO: Randomize
                    byte[] sendIV = new byte[] { 0x52, 0x30, 0x78, 0x73 }; // TODO: Randomize
                    lock (sClients) sClients.Add(client);
                    client.SendHandshake(Config.Instance.Build, receiveIV, sendIV);
                    BeginChannelListenerAccept(pArguments);
                }
                else if (pArguments.SocketError != SocketError.OperationAborted) Log.WriteLine(ELogLevel.Error, "[Server] ChannelListener Error: {0}", pArguments.SocketError);
            }
            catch (ObjectDisposedException) { }
            catch (Exception exc) { Log.WriteLine(ELogLevel.Exception, "[Server] ChannelListener Exception: {0}", exc.Message); }
        }

        internal static int Population { get { lock (sClients) return sClients.Count; } }

        internal static bool IsPendingPlayerLogin(int pAccountIdentifier) { return sPendingPlayerLoginRegistry.Exists(p => p.AccountIdentifier == pAccountIdentifier); }
        internal static int ValidatePlayerLogin(int pPlayerIdentifier, string pHost)
        {
            PendingPlayerLogin pendingPlayerLogin = sPendingPlayerLoginRegistry.Find(p => p.PlayerIdentifier == pPlayerIdentifier && p.Host == pHost);
            if (pendingPlayerLogin == null) return 0;
            return pendingPlayerLogin.AccountIdentifier;
        }
        internal static void RegisterPlayerLogin(int pAccountIdentifier, int pPlayerIdentifier, string pHost) { sPendingPlayerLoginRegistry.Add(new PendingPlayerLogin(pAccountIdentifier, pPlayerIdentifier, pHost)); }
        internal static void UnregisterPlayerLogin(int pPlayerIdentifier) { sPendingPlayerLoginRegistry.RemoveAll(p => p.PlayerIdentifier == pPlayerIdentifier); }

        internal static void RegisterPortalScript(string pName, Type pType) { sPortalScripts[pName] = pType; }
        internal static PortalScript GetPortalScript(string pName)
        {
            if (string.IsNullOrEmpty(pName)) return null;
            Type typeScript = sPortalScripts.GetOrDefault(pName, null);
            if (typeScript == null) return null;
            return typeScript.InvokeMember("", BindingFlags.CreateInstance, null, null, null) as PortalScript;
        }
    }
    public delegate void Callback();

    internal sealed class PendingPlayerLogin
    {
        private int mAccountIdentifier;
        private int mPlayerIdentifier;
        private string mHost;
        private DateTime mStarted = DateTime.Now;

        public PendingPlayerLogin(int pAccountIdentifier, int pPlayerIdentifier, string pHost)
        {
            mAccountIdentifier = pAccountIdentifier;
            mPlayerIdentifier = pPlayerIdentifier;
            mHost = pHost;
        }

        public int AccountIdentifier { get { return mAccountIdentifier; } }
        public int PlayerIdentifier { get { return mPlayerIdentifier; } }
        public string Host { get { return mHost; } }
        public DateTime Started { get { return mStarted; } }
    }
}
