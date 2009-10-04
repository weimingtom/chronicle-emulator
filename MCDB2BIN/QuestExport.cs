using Chronicle.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCDB2BIN
{
    internal static class QuestExport
    {
        public static void Export(BinaryWriter pWriter)
        {
            PerformanceTimer timer = new PerformanceTimer();
            long dataCount = 0;
            timer.Unpause();

            List<QuestData> datas = new List<QuestData>();
            using (MySqlConnection connection1 = new MySqlConnection(Program.Database))
            {
                connection1.Open();
                MySqlCommand command1 = connection1.CreateCommand();
                command1.CommandText = "SELECT COUNT(*) FROM quest_data";
                Program.ResetCounter((int)(long)command1.ExecuteScalar());
                command1.CommandText = "SELECT * FROM quest_data ORDER BY questid ASC";
                using (MySqlDataReader reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        QuestData data = new QuestData();
                        data.Identifier = (ushort)Convert.ChangeType(reader1["questid"], TypeCode.UInt16);
                        data.NextIdentifier = (ushort)Convert.ChangeType(reader1["next_quest"], TypeCode.UInt16);
                        data.Area = (byte)Convert.ChangeType(reader1["quest_area"], TypeCode.Byte);
                        data.MinLevel = (byte)Convert.ChangeType(reader1["min_level"], TypeCode.Byte);
                        data.MaxLevel = (byte)Convert.ChangeType(reader1["max_level"], TypeCode.Byte);
                        data.PetCloseness = (ushort)Convert.ChangeType(reader1["pet_closeness"], TypeCode.UInt16);
                        data.TamingMobLevel = (byte)Convert.ChangeType(reader1["taming_mob_level"], TypeCode.Byte);
                        data.RepeatWait = (int)Convert.ChangeType(reader1["repeat_wait"], TypeCode.Int32);
                        data.Fame = (ushort)Convert.ChangeType(reader1["fame"], TypeCode.UInt16);
                        data.Jobs = new List<ushort>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM quest_required_jobs WHERE questid=@questid ORDER BY valid_jobid ASC";
                            command2.Parameters.AddWithValue("@questid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    data.Jobs.Add((ushort)Convert.ChangeType(reader2["valid_jobid"], TypeCode.UInt16));
                                }
                            }
                        }
                        data.Requirements = new List<QuestData.QuestRequirementData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM quest_requests WHERE questid=@questid ORDER BY quest_state+0 ASC,request_type+0 ASC";
                            command2.Parameters.AddWithValue("@questid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    QuestData.QuestRequirementData requirementData = new QuestData.QuestRequirementData();
                                    requirementData.State = (QuestData.QuestRequirementData.EQuestRequirementState)Enum.Parse(typeof(QuestData.QuestRequirementData.EQuestRequirementState), (string)Convert.ChangeType(reader2["quest_state"], TypeCode.String), true);
                                    requirementData.Type = (QuestData.QuestRequirementData.EQuestRequirementType)Enum.Parse(typeof(QuestData.QuestRequirementData.EQuestRequirementType), (string)Convert.ChangeType(reader2["request_type"], TypeCode.String), true);
                                    requirementData.Parameter1 = (int)Convert.ChangeType(reader2["objectid"], TypeCode.Int32);
                                    requirementData.Parameter2 = (int)Convert.ChangeType(reader2["count"], TypeCode.Int32);

                                    data.Requirements.Add(requirementData);
                                }
                            }
                        }
                        data.Rewards = new List<QuestData.QuestRewardData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM quest_rewards WHERE questid=@questid ORDER BY quest_state+0 ASC,reward_type+0 ASC";
                            command2.Parameters.AddWithValue("@questid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    QuestData.QuestRewardData rewardData = new QuestData.QuestRewardData();
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") rewardData.Flags = (QuestData.QuestRewardData.EQuestRewardFlags)Enum.Parse(typeof(QuestData.QuestRewardData.EQuestRewardFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    rewardData.State = (QuestData.QuestRewardData.EQuestRewardState)Enum.Parse(typeof(QuestData.QuestRewardData.EQuestRewardState), (string)Convert.ChangeType(reader2["quest_state"], TypeCode.String), true);
                                    rewardData.Type = (QuestData.QuestRewardData.EQuestRewardType)Enum.Parse(typeof(QuestData.QuestRewardData.EQuestRewardType), (string)Convert.ChangeType(reader2["reward_type"], TypeCode.String), true);
                                    rewardData.Parameter1 = (int)Convert.ChangeType(reader2["rewardid"], TypeCode.Int32);
                                    rewardData.Parameter2 = (int)Convert.ChangeType(reader2["count"], TypeCode.Int32);
                                    rewardData.MasterLevel = (byte)Convert.ChangeType(reader2["master_level"], TypeCode.Byte);
                                    rewardData.Gender = (QuestData.QuestRewardData.EQuestRewardGender)Enum.Parse(typeof(QuestData.QuestRewardData.EQuestRewardGender), (string)Convert.ChangeType(reader2["gender"], TypeCode.String), true);
                                    if ((string)Convert.ChangeType(reader2["job_tracks"], TypeCode.String) != "") rewardData.Tracks = (QuestData.QuestRewardData.EQuestRewardTracks)Enum.Parse(typeof(QuestData.QuestRewardData.EQuestRewardTracks), (string)Convert.ChangeType(reader2["job_tracks"], TypeCode.String), true);
                                    rewardData.Job = (short)Convert.ChangeType(reader2["job"], TypeCode.Int16);
                                    rewardData.Prop = (int)Convert.ChangeType(reader2["prop"], TypeCode.Int32);

                                    data.Rewards.Add(rewardData);
                                }
                            }
                        }

                        datas.Add(data);
                        ++dataCount;
                        ++Program.AllDataCounter;
                        Program.IncrementCounter();
                    }
                }
            }

            pWriter.Write(datas.Count);
            datas.ForEach(d => d.Save(pWriter));

            timer.Pause();
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", "QuestData", dataCount, timer.Duration);
        }
    }
}
