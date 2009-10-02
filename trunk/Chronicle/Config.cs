using System;
using System.Xml;
using System.Xml.Serialization;

namespace Chronicle
{
    public sealed class Config
    {
        internal static Config Instance { get; private set; }

        internal static void Load()
        {
            using (XmlReader reader = XmlReader.Create("Config.xml")) Instance = (Config)(new XmlSerializer(typeof(Config))).Deserialize(reader);
        }

        public string Database;
        public string Binary;
        public ushort Build;
        public LoginConfig Login = new LoginConfig();
        public WorldConfig World = new WorldConfig();
        public ChannelConfig Channel = new ChannelConfig();
        public string[] Scripts = new string[0];

        public sealed class ListenerConfig
        {
            public ushort Port;
            public byte Backlog;
        }
        public sealed class LoginConfig
        {
            public ListenerConfig Listener = new ListenerConfig();
            public byte MaxPlayersPerAccount;
        }
        public sealed class WorldConfig
        {
            public byte Ribbon;
            public string EventMessage;
            public byte ExperienceModifier;
            public byte DropModifier;
            public byte PendingPlayerLoginDelay;
        }
        public sealed class ChannelConfig
        {
            public ListenerConfig Listener = new ListenerConfig();
            public int MaxPopulation;
            public string ExternalAddress;
        }
    }
}
