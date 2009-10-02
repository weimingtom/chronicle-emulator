using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Chronicle.Utility
{
    public sealed class DatabaseQuery : IDisposable
    {
        private MySqlConnection mConnection;
        private MySqlDataReader mReader;

        internal DatabaseQuery(MySqlConnection pConnection, MySqlDataReader pReader)
        {
            mConnection = pConnection;
            mReader = pReader;
        }

        public bool NextRow() { return mReader.Read(); }

        public void Dispose() { mReader.Close(); mConnection.Close(); }

        public object this[string pField] { get { return mReader[pField]; } }

        public bool IsNullOrEmpty(string pField)
        {
            object value = mReader[pField];
            if (value == null || value is DBNull || !(value is string)) return true;
            return ((string)value).Length == 0;
        }
    }
}
