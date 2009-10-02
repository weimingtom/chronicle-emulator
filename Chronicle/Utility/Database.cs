using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace Chronicle.Utility
{
    public sealed class Database
    {
        private static string sConnectionString;

        [Initializer(0)]
        public static bool Initialize()
        {
            sConnectionString = Config.Instance.Database;
            try
            {
                MySqlConnection connection = new MySqlConnection(sConnectionString);
                connection.Open();
                connection.Close();
            }
            catch (Exception exc)
            {
                Log.WriteLine(ELogLevel.Exception, "[Database] Exception: {0}", exc.Message);
                return false;
            }
            Log.WriteLine(ELogLevel.Info, "[Database] Initialized");
            return true;
        }

        private static bool Test()
        {
            return true;
        }

        public static DatabaseQuery Query(string pQuery, params MySqlParameter[] pParams)
        {
            MySqlConnection connection = new MySqlConnection(sConnectionString);
            connection.Open();
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = pQuery;
            Array.ForEach(pParams, p => command.Parameters.Add(p));
            return new DatabaseQuery(connection, command.ExecuteReader());
        }

        public static void Execute(string pStatement, params MySqlParameter[] pParams)
        {
            using (MySqlConnection connection = new MySqlConnection(sConnectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = pStatement;
                Array.ForEach(pParams, p => command.Parameters.Add(p));
                command.ExecuteNonQuery();
            }
        }

        public static object Scalar(string pQuery, params MySqlParameter[] pParams)
        {
            using (MySqlConnection connection = new MySqlConnection(sConnectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = pQuery;
                Array.ForEach(pParams, p => command.Parameters.Add(p));
                return command.ExecuteScalar();
            }
        }

        public static int InsertAndReturnIdentifier(string pStatement, params MySqlParameter[] pParams)
        {
            using (MySqlConnection connection = new MySqlConnection(sConnectionString))
            {
                connection.Open();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = pStatement;
                Array.ForEach(pParams, p => command.Parameters.Add(p));
                command.ExecuteNonQuery();
                command.CommandText = "SELECT LAST_INSERT_ID()";
                command.Parameters.Clear();
                return (int)(long)command.ExecuteScalar();
            }
        }
    }
}
