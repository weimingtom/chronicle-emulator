using Chronicle.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCDB2BIN
{
    internal static class MobExport
    {
        public static void Export(BinaryWriter pWriter)
        {
            PerformanceTimer timer = new PerformanceTimer();
            long dataCount = 0;
            timer.Unpause();

            List<MobData> datas = new List<MobData>();
            using (MySqlConnection connection1 = new MySqlConnection(Program.Database))
            {
                connection1.Open();
                MySqlCommand command1 = connection1.CreateCommand();
                command1.CommandText = "SELECT COUNT(*) FROM mob_data";
                Program.ResetCounter((int)(long)command1.ExecuteScalar());
                command1.CommandText = "SELECT * FROM mob_data ORDER BY mobid ASC";
                using (MySqlDataReader reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        MobData data = new MobData();
                        data.Identifier = (int)Convert.ChangeType(reader1["mobid"], TypeCode.Int32);
                        if ((string)Convert.ChangeType(reader1["flags"], TypeCode.String) != "") data.Flags = (MobData.EMobFlags)Enum.Parse(typeof(MobData.EMobFlags), (string)Convert.ChangeType(reader1["flags"], TypeCode.String), true);
                        data.Level = (byte)(ushort)Convert.ChangeType(reader1["level"], TypeCode.UInt16);
                        data.HP = (int)Convert.ChangeType(reader1["hp"], TypeCode.Int32);
                        data.MP = (int)Convert.ChangeType(reader1["mp"], TypeCode.Int32);
                        data.HPRecovery = (int)Convert.ChangeType(reader1["hp_recovery"], TypeCode.Int32);
                        data.MPRecovery = (ushort)Convert.ChangeType(reader1["mp_recovery"], TypeCode.UInt16);
                        data.ExplodeHP = (int)Convert.ChangeType(reader1["explode_hp"], TypeCode.Int32);
                        data.Experience = (int)Convert.ChangeType(reader1["exp"], TypeCode.Int32);
                        data.LinkIdentifier = (int)Convert.ChangeType(reader1["link"], TypeCode.Int32);
                        data.SummonType = (byte)Convert.ChangeType(reader1["summon_type"], TypeCode.Byte);
                        data.Knockback = (int)Convert.ChangeType(reader1["knockback"], TypeCode.Int32);
                        data.FixedDamage = (ushort)Convert.ChangeType(reader1["fixed_damage"], TypeCode.UInt16);
                        data.DeathBuffIdentifier = (int)Convert.ChangeType(reader1["death_buff"], TypeCode.Int32);
                        data.DeathAfter = (int)Convert.ChangeType(reader1["death_after"], TypeCode.Int32);
                        data.Traction = (byte)Convert.ChangeType(reader1["traction"], TypeCode.Byte);
                        data.DamagedBySkillIdentifierOnly = (int)Convert.ChangeType(reader1["damaged_by_skill_only"], TypeCode.Int32);
                        data.DamagedByMobIdentifierOnly = (int)Convert.ChangeType(reader1["damaged_by_mob_only"], TypeCode.Int32);
                        data.HPBarColor = (byte)Convert.ChangeType(reader1["hp_bar_color"], TypeCode.Byte);
                        data.HPBarBackgroundColor = (byte)Convert.ChangeType(reader1["hp_bar_bg_color"], TypeCode.Byte);
                        data.CarnivalPoints = (byte)Convert.ChangeType(reader1["carnival_points"], TypeCode.Byte);
                        data.PhysicalAttack = (ushort)Convert.ChangeType(reader1["physical_attack"], TypeCode.UInt16);
                        data.PhysicalDefense = (ushort)Convert.ChangeType(reader1["physical_defense"], TypeCode.UInt16);
                        data.MagicalAttack = (ushort)Convert.ChangeType(reader1["magical_attack"], TypeCode.UInt16);
                        data.MagicalDefense = (ushort)Convert.ChangeType(reader1["magical_defense"], TypeCode.UInt16);
                        data.Accuracy = (short)Convert.ChangeType(reader1["accuracy"], TypeCode.Int16);
                        data.Avoidance = (ushort)Convert.ChangeType(reader1["avoidability"], TypeCode.UInt16);
                        data.Speed = (short)Convert.ChangeType(reader1["speed"], TypeCode.Int16);
                        data.ChaseSpeed = (short)Convert.ChangeType(reader1["chase_speed"], TypeCode.Int16);
                        data.IceModifier = (MobData.EMobMagicModifier)Enum.Parse(typeof(MobData.EMobMagicModifier), (string)Convert.ChangeType(reader1["ice_modifier"], TypeCode.String), true);
                        data.FireModifier = (MobData.EMobMagicModifier)Enum.Parse(typeof(MobData.EMobMagicModifier), (string)Convert.ChangeType(reader1["fire_modifier"], TypeCode.String), true);
                        data.PoisonModifier = (MobData.EMobMagicModifier)Enum.Parse(typeof(MobData.EMobMagicModifier), (string)Convert.ChangeType(reader1["poison_modifier"], TypeCode.String), true);
                        data.LightningModifier = (MobData.EMobMagicModifier)Enum.Parse(typeof(MobData.EMobMagicModifier), (string)Convert.ChangeType(reader1["lightning_modifier"], TypeCode.String), true);
                        data.HolyModifier = (MobData.EMobMagicModifier)Enum.Parse(typeof(MobData.EMobMagicModifier), (string)Convert.ChangeType(reader1["holy_modifier"], TypeCode.String), true);
                        data.NonElementalModifier = (MobData.EMobMagicModifier)Enum.Parse(typeof(MobData.EMobMagicModifier), (string)Convert.ChangeType(reader1["nonelemental_modifier"], TypeCode.String), true);
                        data.Abilities = new List<MobData.MobAbilityData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM mob_skills WHERE mobid=@mobid ORDER BY skillid ASC,level ASC";
                            command2.Parameters.AddWithValue("@mobid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    MobData.MobAbilityData abilityData = new MobData.MobAbilityData();
                                    abilityData.AbilityIdentifier = (byte)Convert.ChangeType(reader2["skillid"], TypeCode.Byte);
                                    abilityData.AbilityLevel = (byte)Convert.ChangeType(reader2["level"], TypeCode.Byte);
                                    abilityData.EffectDelay = (ushort)Convert.ChangeType(reader2["effect_delay"], TypeCode.UInt16);

                                    data.Abilities.Add(abilityData);
                                }
                            }
                        }
                        data.Attacks = new List<MobData.MobAttackData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM mob_attacks WHERE mobid=@mobid ORDER BY attackid ASC";
                            command2.Parameters.AddWithValue("@mobid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    MobData.MobAttackData attackData = new MobData.MobAttackData();
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") attackData.Flags = (MobData.MobAttackData.EMobAttackFlags)Enum.Parse(typeof(MobData.MobAttackData.EMobAttackFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    attackData.MPCost = (byte)Convert.ChangeType(reader2["mp_cost"], TypeCode.Byte);
                                    attackData.MPBurn = (ushort)Convert.ChangeType(reader2["mp_burn"], TypeCode.UInt16);
                                    attackData.AbilityIdentifier = (byte)Convert.ChangeType(reader2["disease"], TypeCode.Byte);
                                    attackData.AbilityLevel = (byte)Convert.ChangeType(reader2["level"], TypeCode.Byte);

                                    data.Attacks.Add(attackData);
                                }
                            }
                        }
                        data.Summons = new List<int>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM mob_summons WHERE mobid=@mobid ORDER BY id ASC";
                            command2.Parameters.AddWithValue("@mobid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    data.Summons.Add((int)Convert.ChangeType(reader2["summonid"], TypeCode.Int32));
                                }
                            }
                        }
                        data.Drops = new List<MobData.MobDropData>();
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
                                    MobData.MobDropData dropData = new MobData.MobDropData();
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") dropData.Flags = (MobData.MobDropData.EMobDropFlags)Enum.Parse(typeof(MobData.MobDropData.EMobDropFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
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
                        Program.IncrementCounter();
                    }
                }
            }

            pWriter.Write(datas.Count);
            datas.ForEach(d => d.Save(pWriter));

            timer.Pause();
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", "MobData", dataCount, timer.Duration);
        }
    }
}
