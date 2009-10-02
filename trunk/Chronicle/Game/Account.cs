using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class Account
    {
        internal Account(DatabaseQuery pQuery)
        {
            Identifier = (int)pQuery["identifier"];
            Username = (string)pQuery["username"];
            Password = (string)pQuery["password"];
            Level = (byte)pQuery["level"];
        }

        public int Identifier { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public byte Level { get; private set; }
        public Player Player { get; internal set; }
    }
}
