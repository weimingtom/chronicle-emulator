using Chronicle.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCDB2BIN
{
    internal static class SkillExport
    {
        public static void Export(BinaryWriter pWriter)
        {
            PerformanceTimer timer = new PerformanceTimer();
            long dataCount = 0;
            timer.Unpause();

            List<SkillData> datas = new List<SkillData>();
            using (MySqlConnection connection1 = new MySqlConnection(Program.Database))
            {
                connection1.Open();
                MySqlCommand command1 = connection1.CreateCommand();
                command1.CommandText = "SELECT COUNT(*) FROM skill_player_data";
                Program.ResetCounter((int)(long)command1.ExecuteScalar());
                command1.CommandText = "SELECT * FROM skill_player_data ORDER BY skillid ASC,level ASC";
                using (MySqlDataReader reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        SkillData data = new SkillData();
                        data.Identifier = (int)Convert.ChangeType(reader1["skillid"], TypeCode.Int32);
                        data.Level = (byte)Convert.ChangeType(reader1["level"], TypeCode.Byte);
                        data.MobCount = (byte)Convert.ChangeType(reader1["mob_count"], TypeCode.Byte);
                        data.HitCount = (byte)Convert.ChangeType(reader1["hit_count"], TypeCode.Byte);
                        data.Range = (ushort)Convert.ChangeType(reader1["range"], TypeCode.UInt16);
                        data.Duration = (int)Convert.ChangeType(reader1["time"], TypeCode.Int32);
                        data.MPCost = (ushort)Convert.ChangeType(reader1["mp_cost"], TypeCode.UInt16);
                        data.HPCost = (byte)Convert.ChangeType(reader1["hp_cost"], TypeCode.Byte);
                        data.Damage = (ushort)Convert.ChangeType(reader1["damage"], TypeCode.UInt16);
                        data.FixedDamage = (byte)Convert.ChangeType(reader1["fixed_damage"], TypeCode.Byte);
                        data.CriticalDamage = (byte)Convert.ChangeType(reader1["critical_damage"], TypeCode.Byte);
                        data.Mastery = (byte)Convert.ChangeType(reader1["mastery"], TypeCode.Byte);
                        data.OptionalItemCost = (int)Convert.ChangeType(reader1["optional_item_cost"], TypeCode.Int32);
                        data.ItemCost = (int)Convert.ChangeType(reader1["item_cost"], TypeCode.Int32);
                        data.ItemCount = (byte)Convert.ChangeType(reader1["item_count"], TypeCode.Byte);
                        data.BulletCost = (byte)Convert.ChangeType(reader1["bullet_cost"], TypeCode.Byte);
                        data.MoneyCost = (ushort)Convert.ChangeType(reader1["money_cost"], TypeCode.UInt16);
                        data.Parameter1 = (int)Convert.ChangeType(reader1["x"], TypeCode.Int32);
                        data.Parameter2 = (int)Convert.ChangeType(reader1["y"], TypeCode.Int32);
                        data.Speed = (short)Convert.ChangeType(reader1["speed"], TypeCode.Int16);
                        data.Jump = (byte)Convert.ChangeType(reader1["jump"], TypeCode.Byte);
                        data.Strength = (byte)Convert.ChangeType(reader1["str"], TypeCode.Byte);
                        data.WeaponAttack = (short)Convert.ChangeType(reader1["weapon_atk"], TypeCode.Int16);
                        data.WeaponDefense = (short)Convert.ChangeType(reader1["weapon_def"], TypeCode.Int16);
                        data.MagicAttack = (short)Convert.ChangeType(reader1["magic_atk"], TypeCode.Int16);
                        data.MagicDefense = (short)Convert.ChangeType(reader1["magic_def"], TypeCode.Int16);
                        data.Accuracy = (byte)Convert.ChangeType(reader1["accuracy"], TypeCode.Byte);
                        data.Avoidance = (byte)Convert.ChangeType(reader1["avoid"], TypeCode.Byte);
                        data.HP = (ushort)Convert.ChangeType(reader1["hp"], TypeCode.UInt16);
                        data.MP = (byte)Convert.ChangeType(reader1["mp"], TypeCode.Byte);
                        data.Prop = (byte)Convert.ChangeType(reader1["prop"], TypeCode.Byte);
                        data.Morph = (ushort)Convert.ChangeType(reader1["morph"], TypeCode.UInt16);
                        data.LeftTopX = (short)Convert.ChangeType(reader1["ltx"], TypeCode.Int16);
                        data.LeftTopY = (short)Convert.ChangeType(reader1["lty"], TypeCode.Int16);
                        data.RightBottomX = (short)Convert.ChangeType(reader1["rbx"], TypeCode.Int16);
                        data.RightBottomY = (short)Convert.ChangeType(reader1["rby"], TypeCode.Int16);
                        data.Cooldown = (ushort)Convert.ChangeType(reader1["cooldown_time"], TypeCode.UInt16);

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
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", "SkillData", dataCount, timer.Duration);
        }
    }
}
