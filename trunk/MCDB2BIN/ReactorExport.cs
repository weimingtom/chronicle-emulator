using Chronicle.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCDB2BIN
{
    internal static class ReactorExport
    {
        public static void Export(BinaryWriter pWriter)
        {
            PerformanceTimer timer = new PerformanceTimer();
            long dataCount = 0;
            timer.Unpause();

            List<ReactorData> datas = new List<ReactorData>();
            using (MySqlConnection connection1 = new MySqlConnection(Program.Database))
            {
                connection1.Open();
                MySqlCommand command1 = connection1.CreateCommand();
                command1.CommandText = "SELECT * FROM reactor_data ORDER BY reactorid ASC";
                using (MySqlDataReader reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        ReactorData data = new ReactorData();
                        data.Identifier = (int)Convert.ChangeType(reader1["reactorid"], TypeCode.Int32);
                        if ((string)Convert.ChangeType(reader1["flags"], TypeCode.String) != "") data.Flags = (ReactorData.EReactorFlags)Enum.Parse(typeof(ReactorData.EReactorFlags), (string)Convert.ChangeType(reader1["flags"], TypeCode.String), true);
                        data.LinkIdentifier = (int)Convert.ChangeType(reader1["link"], TypeCode.Int32);
                        data.States = new List<ReactorData.ReactorStateData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM reactor_events WHERE reactorid=@reactorid ORDER BY state ASC";
                            command2.Parameters.AddWithValue("@reactorid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    ReactorData.ReactorStateData stateData = new ReactorData.ReactorStateData();
                                    stateData.State = (byte)Convert.ChangeType(reader2["state"], TypeCode.Byte);
                                    stateData.Type = (ReactorData.ReactorStateData.EReactorStateType)Enum.Parse(typeof(ReactorData.ReactorStateData.EReactorStateType), (string)Convert.ChangeType(reader2["type"], TypeCode.String), true);
                                    stateData.Timeout = (int)Convert.ChangeType(reader2["timeout"], TypeCode.Int32);
                                    stateData.ItemIdentifier = (int)Convert.ChangeType(reader2["itemid"], TypeCode.Int32);
                                    stateData.Quantity = (byte)Convert.ChangeType(reader2["quantity"], TypeCode.Byte);
                                    stateData.LeftTopX = (short)Convert.ChangeType(reader2["ltx"], TypeCode.Int16);
                                    stateData.LeftTopY = (short)Convert.ChangeType(reader2["lty"], TypeCode.Int16);
                                    stateData.RightBottomX = (short)Convert.ChangeType(reader2["rbx"], TypeCode.Int16);
                                    stateData.RightBottomY = (short)Convert.ChangeType(reader2["rby"], TypeCode.Int16);

                                    data.States.Add(stateData);
                                }
                            }
                        }
                        data.Drops = new List<ReactorData.ReactorDropData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM drop_data WHERE dropperid=@dropperid ORDER BY id ASC";
                            command2.Parameters.AddWithValue("@dropperid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    ReactorData.ReactorDropData dropData = new ReactorData.ReactorDropData();
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") dropData.Flags = (ReactorData.ReactorDropData.EReactorDropFlags)Enum.Parse(typeof(ReactorData.ReactorDropData.EReactorDropFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    dropData.ItemIdentifier = (int)Convert.ChangeType(reader2["itemid"], TypeCode.Int32);
                                    dropData.Minimum = (int)Convert.ChangeType(reader2["minimum_quantity"], TypeCode.Int32);
                                    dropData.Maximum = (int)Convert.ChangeType(reader2["maximum_quantity"], TypeCode.Int32);
                                    dropData.QuestIdentifier = (ushort)Convert.ChangeType(reader2["questid"], TypeCode.UInt16);
                                    dropData.Chance = (int)Convert.ChangeType(reader2["chance"], TypeCode.Int32);

                                    data.Drops.Add(dropData);
                                }
                            }
                        }

                        datas.Add(data);
                        ++dataCount;
                        ++Program.AllDataCounter;
                    }
                }
            }

            pWriter.Write(datas.Count);
            datas.ForEach(d => d.Save(pWriter));

            timer.Pause();
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", "ReactorData", dataCount, timer.Duration);
        }
    }
}
