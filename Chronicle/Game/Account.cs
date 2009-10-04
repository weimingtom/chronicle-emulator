using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class Account
    {
        private int mIdentifier;
        private string mUsername;
        private string mPassword;
        private byte mLevel;

        internal Account(DatabaseQuery pQuery)
        {
            mIdentifier = (int)pQuery["identifier"];
            mUsername = (string)pQuery["username"];
            mPassword = (string)pQuery["password"];
            mLevel = (byte)pQuery["level"];
        }

        public int Identifier { get { return mIdentifier; } }
        public string Username { get { return mUsername; } }
        public string Password { get { return mPassword; } }
        public byte Level { get { return mLevel; } }
    }
}
