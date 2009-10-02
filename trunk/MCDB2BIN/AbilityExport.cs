using Chronicle.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCDB2BIN
{
    internal static class AbilityExport
    {
        public static void Export(BinaryWriter pWriter)
        {
            PerformanceTimer timer = new PerformanceTimer();
            long dataCount = 0;
            timer.Unpause();

            List<AbilityData> datas = new List<AbilityData>();
            using (MySqlConnection connection1 = new MySqlConnection(Program.Database))
            {
                connection1.Open();
                MySqlCommand command1 = connection1.CreateCommand();
                command1.CommandText = "SELECT * FROM skill_mob_data ORDER BY skillid ASC,level ASC";
                using (MySqlDataReader reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        AbilityData data = new AbilityData();
                        data.Identifier = (byte)Convert.ChangeType(reader1["skillid"], TypeCode.Byte);
                        data.Level = (byte)Convert.ChangeType(reader1["level"], TypeCode.Byte);
                        data.Duration = (ushort)Convert.ChangeType(reader1["time"], TypeCode.UInt16);
                        data.MPCost = (byte)Convert.ChangeType(reader1["mp_cost"], TypeCode.Byte);
                        data.Parameter1 = (int)Convert.ChangeType(reader1["x"], TypeCode.Int32);
                        data.Parameter2 = (int)Convert.ChangeType(reader1["y"], TypeCode.Int32);
                        data.Chance = (byte)Convert.ChangeType(reader1["chance"], TypeCode.Byte);
                        data.TargetCount = (byte)Convert.ChangeType(reader1["target_count"], TypeCode.Byte);
                        data.Cooldown = (ushort)Convert.ChangeType(reader1["cooldown"], TypeCode.UInt16);
                        data.LeftTopX = (short)Convert.ChangeType(reader1["ltx"], TypeCode.Int16);
                        data.LeftTopY = (short)Convert.ChangeType(reader1["lty"], TypeCode.Int16);
                        data.RightBottomX = (short)Convert.ChangeType(reader1["rbx"], TypeCode.Int16);
                        data.RightBottomY = (short)Convert.ChangeType(reader1["rby"], TypeCode.Int16);
                        data.HPLimitPercent = (byte)Convert.ChangeType(reader1["hp_limit_percentage"], TypeCode.Byte);
                        data.SummonLimit = (ushort)Convert.ChangeType(reader1["summon_limit"], TypeCode.UInt16);
                        data.SummonEffect = (byte)Convert.ChangeType(reader1["summon_effect"], TypeCode.Byte);
                        data.SummonIdentifiers = new List<int>();
                        if (data.Identifier == 200)
                        {
                            using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                            {
                                connection2.Open();
                                MySqlCommand command2 = connection2.CreateCommand();
                                command2.CommandText = "SELECT * FROM skill_mob_summons WHERE level=@level ORDER BY mob_index ASC";
                                command2.Parameters.AddWithValue("@level", data.Level);
                                using (MySqlDataReader reader2 = command2.ExecuteReader())
                                {
                                    while (reader2.Read())
                                    {
                                        data.SummonIdentifiers.Add((int)Convert.ChangeType(reader2["mobid"], TypeCode.Int32));
                                    }
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
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", "AbilityData", dataCount, timer.Duration);
        }
    }
}
