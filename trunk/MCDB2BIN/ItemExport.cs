using Chronicle.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCDB2BIN
{
    internal static class ItemExport
    {
        public static void Export(BinaryWriter pWriter)
        {
            PerformanceTimer timer = new PerformanceTimer();
            long dataCount = 0;
            timer.Unpause();

            List<ItemData> datas = new List<ItemData>();
            using (MySqlConnection connection1 = new MySqlConnection(Program.Database))
            {
                connection1.Open();
                MySqlCommand command1 = connection1.CreateCommand();
                command1.CommandText = "SELECT COUNT(*) FROM item_data";
                Program.ResetCounter((int)(long)command1.ExecuteScalar());
                command1.CommandText = "SELECT * FROM item_data ORDER BY itemid ASC";
                using (MySqlDataReader reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        ItemData data = new ItemData();
                        data.Identifier = (int)Convert.ChangeType(reader1["itemid"], TypeCode.Int32);
                        if ((string)Convert.ChangeType(reader1["flags"], TypeCode.String) != "") data.Flags = (ItemData.EItemFlags)Enum.Parse(typeof(ItemData.EItemFlags), (string)Convert.ChangeType(reader1["flags"], TypeCode.String), true);
                        data.Price = (int)Convert.ChangeType(reader1["price"], TypeCode.Int32);
                        data.MaxSlotQuantity = (ushort)Convert.ChangeType(reader1["max_slot_quantity"], TypeCode.UInt16);
                        data.MaxPossessionCount = (byte)Convert.ChangeType(reader1["max_possession_count"], TypeCode.Byte);
                        data.MinLevel = (byte)Convert.ChangeType(reader1["min_level"], TypeCode.Byte);
                        data.MaxLevel = (byte)Convert.ChangeType(reader1["max_level"], TypeCode.Byte);
                        data.Experience = (int)Convert.ChangeType(reader1["exp"], TypeCode.Int32);
                        data.MakerLevel = (byte)Convert.ChangeType(reader1["level_for_maker"], TypeCode.Byte);
                        data.Money = (int)Convert.ChangeType(reader1["money"], TypeCode.Int32);
                        data.StateChangeItem = (int)Convert.ChangeType(reader1["state_change_item"], TypeCode.Int32);
                        data.NPC = (int)Convert.ChangeType(reader1["npc"], TypeCode.Int32);
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM item_equip_data WHERE equipid=@equipid";
                            command2.Parameters.AddWithValue("@equipid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    data.Equipment = new ItemData.ItemEquipmentData();
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") data.Equipment.Flags = (ItemData.ItemEquipmentData.EItemEquipmentFlags)Enum.Parse(typeof(ItemData.ItemEquipmentData.EItemEquipmentFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    data.Equipment.Slots = (ItemData.ItemEquipmentData.EItemEquipmentSlots)Enum.Parse(typeof(ItemData.ItemEquipmentData.EItemEquipmentSlots), (string)Convert.ChangeType(reader2["equip_slots"], TypeCode.String), true);
                                    data.Equipment.AttackSpeed = (byte)Convert.ChangeType(reader2["attack_speed"], TypeCode.Byte);
                                    data.Equipment.HealHP = (byte)Convert.ChangeType(reader2["heal_hp"], TypeCode.Byte);
                                    data.Equipment.Scrolls = (byte)Convert.ChangeType(reader2["scroll_slots"], TypeCode.Byte);
                                    data.Equipment.RequiredStrength = (ushort)Convert.ChangeType(reader2["req_str"], TypeCode.UInt16);
                                    data.Equipment.RequiredDexterity = (ushort)Convert.ChangeType(reader2["req_dex"], TypeCode.UInt16);
                                    data.Equipment.RequiredIntellect = (ushort)Convert.ChangeType(reader2["req_int"], TypeCode.UInt16);
                                    data.Equipment.RequiredLuck = (ushort)Convert.ChangeType(reader2["req_luk"], TypeCode.UInt16);
                                    data.Equipment.RequiredFame = (byte)Convert.ChangeType(reader2["req_fame"], TypeCode.Byte);
                                    data.Equipment.RequiredJob = (ItemData.ItemEquipmentData.EItemEquipmentJobFlags)Enum.Parse(typeof(ItemData.ItemEquipmentData.EItemEquipmentJobFlags), (string)Convert.ChangeType(reader2["req_job"], TypeCode.String), true);
                                    data.Equipment.HP = (ushort)Convert.ChangeType(reader2["hp"], TypeCode.UInt16);
                                    data.Equipment.MP = (ushort)Convert.ChangeType(reader2["mp"], TypeCode.UInt16);
                                    data.Equipment.Strength = (ushort)Convert.ChangeType(reader2["str"], TypeCode.UInt16);
                                    data.Equipment.Dexterity = (ushort)Convert.ChangeType(reader2["dex"], TypeCode.UInt16);
                                    data.Equipment.Intellect = (ushort)Convert.ChangeType(reader2["int"], TypeCode.UInt16);
                                    data.Equipment.Luck = (ushort)Convert.ChangeType(reader2["luk"], TypeCode.UInt16);
                                    data.Equipment.Hands = (byte)Convert.ChangeType(reader2["hands"], TypeCode.Byte);
                                    data.Equipment.WeaponAttack = (byte)Convert.ChangeType(reader2["weapon_attack"], TypeCode.Byte);
                                    data.Equipment.MagicAttack = (byte)Convert.ChangeType(reader2["magic_attack"], TypeCode.Byte);
                                    data.Equipment.WeaponDefense = (byte)Convert.ChangeType(reader2["weapon_defense"], TypeCode.Byte);
                                    data.Equipment.MagicDefense = (byte)Convert.ChangeType(reader2["magic_defense"], TypeCode.Byte);
                                    data.Equipment.Accuracy = (byte)Convert.ChangeType(reader2["accuracy"], TypeCode.Byte);
                                    data.Equipment.Avoidance = (byte)Convert.ChangeType(reader2["avoid"], TypeCode.Byte);
                                    data.Equipment.Speed = (byte)Convert.ChangeType(reader2["speed"], TypeCode.Byte);
                                    data.Equipment.Jump = (byte)Convert.ChangeType(reader2["jump"], TypeCode.Byte);
                                    data.Equipment.Traction = (byte)Convert.ChangeType(reader2["traction"], TypeCode.Byte);
                                    data.Equipment.TamingMob = (byte)Convert.ChangeType(reader2["taming_mob"], TypeCode.Byte);
                                    data.Equipment.IceDamage = (byte)Convert.ChangeType(reader2["inc_ice_damage"], TypeCode.Byte);
                                    data.Equipment.FireDamage = (byte)Convert.ChangeType(reader2["inc_fire_damage"], TypeCode.Byte);
                                    data.Equipment.PoisonDamage = (byte)Convert.ChangeType(reader2["inc_poison_damage"], TypeCode.Byte);
                                    data.Equipment.LightningDamage = (byte)Convert.ChangeType(reader2["inc_lightning_damage"], TypeCode.Byte);
                                    data.Equipment.ElementalDefault = (byte)Convert.ChangeType(reader2["elemental_default"], TypeCode.Byte);
                                    data.Equipment.TimelessLevels = new List<ItemData.ItemEquipmentData.ItemEquipmentTimelessLevelData>();
                                    using (MySqlConnection connection3 = new MySqlConnection(Program.Database))
                                    {
                                        connection3.Open();
                                        MySqlCommand command3 = connection3.CreateCommand();
                                        command3.CommandText = "SELECT * FROM item_timeless_levels WHERE equipid=@equipid ORDER BY level ASC";
                                        command3.Parameters.AddWithValue("@equipid", data.Identifier);
                                        using (MySqlDataReader reader3 = command3.ExecuteReader())
                                        {
                                            while (reader3.Read())
                                            {
                                                ItemData.ItemEquipmentData.ItemEquipmentTimelessLevelData levelData = new ItemData.ItemEquipmentData.ItemEquipmentTimelessLevelData();
                                                levelData.Level = (byte)Convert.ChangeType(reader3["level"], TypeCode.Byte);
                                                levelData.Experience = (byte)Convert.ChangeType(reader3["exp"], TypeCode.Byte);
                                                levelData.MinStrength = (byte)Convert.ChangeType(reader3["str_min"], TypeCode.Byte);
                                                levelData.MaxStrength = (byte)Convert.ChangeType(reader3["str_max"], TypeCode.Byte);
                                                levelData.MinDexterity = (byte)Convert.ChangeType(reader3["dex_min"], TypeCode.Byte);
                                                levelData.MaxDexterity = (byte)Convert.ChangeType(reader3["dex_max"], TypeCode.Byte);
                                                levelData.MinIntellect = (byte)Convert.ChangeType(reader3["int_min"], TypeCode.Byte);
                                                levelData.MaxIntellect = (byte)Convert.ChangeType(reader3["int_max"], TypeCode.Byte);
                                                levelData.MinLuck = (byte)Convert.ChangeType(reader3["luk_min"], TypeCode.Byte);
                                                levelData.MaxLuck = (byte)Convert.ChangeType(reader3["luk_max"], TypeCode.Byte);
                                                levelData.MinSpeed = (byte)Convert.ChangeType(reader3["speed_min"], TypeCode.Byte);
                                                levelData.MaxSpeed = (byte)Convert.ChangeType(reader3["speed_max"], TypeCode.Byte);
                                                levelData.MinJump = (byte)Convert.ChangeType(reader3["jump_min"], TypeCode.Byte);
                                                levelData.MaxJump = (byte)Convert.ChangeType(reader3["jump_max"], TypeCode.Byte);
                                                levelData.MinWeaponAttack = (byte)Convert.ChangeType(reader3["weapon_attack_min"], TypeCode.Byte);
                                                levelData.MaxWeaponAttack = (byte)Convert.ChangeType(reader3["weapon_attack_max"], TypeCode.Byte);
                                                levelData.MinWeaponDefense = (byte)Convert.ChangeType(reader3["weapon_defense_min"], TypeCode.Byte);
                                                levelData.MaxWeaponDefense = (byte)Convert.ChangeType(reader3["weapon_defense_max"], TypeCode.Byte);
                                                levelData.MinMagicAttack = (byte)Convert.ChangeType(reader3["magic_attack_min"], TypeCode.Byte);
                                                levelData.MaxMagicAttack = (byte)Convert.ChangeType(reader3["magic_attack_max"], TypeCode.Byte);
                                                levelData.MinMagicDefense = (byte)Convert.ChangeType(reader3["magic_defense_min"], TypeCode.Byte);
                                                levelData.MaxMagicDefense = (byte)Convert.ChangeType(reader3["magic_defense_max"], TypeCode.Byte);
                                                levelData.MinHP = (byte)Convert.ChangeType(reader3["hp_min"], TypeCode.Byte);
                                                levelData.MaxHP = (byte)Convert.ChangeType(reader3["hp_max"], TypeCode.Byte);
                                                levelData.MinMP = (byte)Convert.ChangeType(reader3["mp_min"], TypeCode.Byte);
                                                levelData.MaxMP = (byte)Convert.ChangeType(reader3["mp_max"], TypeCode.Byte);
                                                levelData.MinAccuracy = (byte)Convert.ChangeType(reader3["accuracy_min"], TypeCode.Byte);
                                                levelData.MaxAccuracy = (byte)Convert.ChangeType(reader3["accuracy_max"], TypeCode.Byte);
                                                levelData.MinAvoidance = (byte)Convert.ChangeType(reader3["avoidability_min"], TypeCode.Byte);
                                                levelData.MaxAvoidance = (byte)Convert.ChangeType(reader3["avoidability_max"], TypeCode.Byte);

                                                data.Equipment.TimelessLevels.Add(levelData);
                                            }
                                        }
                                    }
                                    data.Equipment.TimelessSkills = new List<ItemData.ItemEquipmentData.ItemEquipmentTimelessSkillData>();
                                    using (MySqlConnection connection3 = new MySqlConnection(Program.Database))
                                    {
                                        connection3.Open();
                                        MySqlCommand command3 = connection3.CreateCommand();
                                        command3.CommandText = "SELECT * FROM item_timeless_skills WHERE equipid=@equipid ORDER BY item_level ASC";
                                        command3.Parameters.AddWithValue("@equipid", data.Identifier);
                                        using (MySqlDataReader reader3 = command3.ExecuteReader())
                                        {
                                            while (reader3.Read())
                                            {
                                                ItemData.ItemEquipmentData.ItemEquipmentTimelessSkillData skillData = new ItemData.ItemEquipmentData.ItemEquipmentTimelessSkillData();
                                                skillData.Level = (byte)Convert.ChangeType(reader3["item_level"], TypeCode.Byte);
                                                skillData.SkillIdentifier = (int)Convert.ChangeType(reader3["skillid"], TypeCode.Int32);
                                                skillData.SkillLevel = (byte)Convert.ChangeType(reader3["skill_level"], TypeCode.Byte);
                                                skillData.Probability = (byte)Convert.ChangeType(reader3["probability"], TypeCode.Byte);

                                                data.Equipment.TimelessSkills.Add(skillData);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM item_consume_data WHERE itemid=@itemid";
                            command2.Parameters.AddWithValue("@itemid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    data.Consume = new ItemData.ItemConsumeData();
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") data.Consume.Flags = (ItemData.ItemConsumeData.EItemConsumeFlags)Enum.Parse(typeof(ItemData.ItemConsumeData.EItemConsumeFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    data.Consume.Effect = (byte)Convert.ChangeType(reader2["effect"], TypeCode.Byte);
                                    data.Consume.HP = (ushort)Convert.ChangeType(reader2["hp"], TypeCode.UInt16);
                                    data.Consume.MP = (ushort)Convert.ChangeType(reader2["mp"], TypeCode.UInt16);
                                    data.Consume.HPPercent = (short)Convert.ChangeType(reader2["hp_percentage"], TypeCode.Int16);
                                    data.Consume.MPPercent = (short)Convert.ChangeType(reader2["mp_percentage"], TypeCode.Int16);
                                    data.Consume.MoveTo = (int)Convert.ChangeType(reader2["move_to"], TypeCode.Int32);
                                    data.Consume.DecreaseHunger = (byte)Convert.ChangeType(reader2["decrease_hunger"], TypeCode.Byte);
                                    data.Consume.DecreaseFatigue = (byte)Convert.ChangeType(reader2["decrease_fatigue"], TypeCode.Byte);
                                    data.Consume.CarnivalPoints = (byte)Convert.ChangeType(reader2["carnival_points"], TypeCode.Byte);
                                    data.Consume.CreateItem = (int)Convert.ChangeType(reader2["create_item"], TypeCode.Int32);
                                    data.Consume.Probability = (byte)Convert.ChangeType(reader2["prob"], TypeCode.Byte);
                                    data.Consume.Time = (ushort)Convert.ChangeType(reader2["time"], TypeCode.UInt16);
                                    data.Consume.WeaponAttack = (short)Convert.ChangeType(reader2["weapon_attack"], TypeCode.Int16);
                                    data.Consume.MagicAttack = (short)Convert.ChangeType(reader2["magic_attack"], TypeCode.Int16);
                                    data.Consume.WeaponDefense = (short)Convert.ChangeType(reader2["weapon_defense"], TypeCode.Int16);
                                    data.Consume.MagicDefense = (short)Convert.ChangeType(reader2["magic_defense"], TypeCode.Int16);
                                    data.Consume.Accuracy = (short)Convert.ChangeType(reader2["accuracy"], TypeCode.Int16);
                                    data.Consume.Avoidance = (byte)Convert.ChangeType(reader2["avoid"], TypeCode.Byte);
                                    data.Consume.Speed = (byte)Convert.ChangeType(reader2["speed"], TypeCode.Byte);
                                    data.Consume.Jump = (byte)Convert.ChangeType(reader2["jump"], TypeCode.Byte);
                                    data.Consume.Morph = (byte)Convert.ChangeType(reader2["morph"], TypeCode.Byte);
                                    data.Consume.LootIncrease = (ItemData.ItemConsumeData.EItemConsumeLootIncrease)Enum.Parse(typeof(ItemData.ItemConsumeData.EItemConsumeLootIncrease), (string)Convert.ChangeType(reader2["drop_up"], TypeCode.String), true);
                                    data.Consume.LootIncreaseItemIdentifier = (int)Convert.ChangeType(reader2["drop_up_item"], TypeCode.Int32);
                                    data.Consume.LootIncreaseItemIdentifierRange = (ushort)Convert.ChangeType(reader2["drop_up_item_range"], TypeCode.UInt16);
                                    data.Consume.LootIncreaseMapIdentifierRange = (byte)Convert.ChangeType(reader2["drop_up_map_ranges"], TypeCode.Byte);
                                    data.Consume.IceDefense = (byte)Convert.ChangeType(reader2["defense_vs_ice"], TypeCode.Byte);
                                    data.Consume.FireDefense = (byte)Convert.ChangeType(reader2["defense_vs_fire"], TypeCode.Byte);
                                    data.Consume.PoisonDefense = (byte)Convert.ChangeType(reader2["defense_vs_poison"], TypeCode.Byte);
                                    data.Consume.LightningDefense = (byte)Convert.ChangeType(reader2["defense_vs_lightning"], TypeCode.Byte);
                                    data.Consume.DarknessDefense = (byte)Convert.ChangeType(reader2["defense_vs_darkness"], TypeCode.Byte);
                                    data.Consume.CurseDefense = (byte)Convert.ChangeType(reader2["defense_vs_curse"], TypeCode.Byte);
                                    data.Consume.SealDefense = (byte)Convert.ChangeType(reader2["defense_vs_seal"], TypeCode.Byte);
                                    data.Consume.WeaknessDefense = (byte)Convert.ChangeType(reader2["defense_vs_weakness"], TypeCode.Byte);
                                    data.Consume.StunDefense = (byte)Convert.ChangeType(reader2["defense_vs_stun"], TypeCode.Byte);
                                }
                            }
                        }
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM item_monster_card_map_ranges WHERE itemid=@itemid";
                            command2.Parameters.AddWithValue("@itemid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    data.Card = new ItemData.ItemCardData();
                                    data.Card.StartMapIdentifier = (int)Convert.ChangeType(reader2["startmap"], TypeCode.Int32);
                                    data.Card.EndMapIdentifier = (int)Convert.ChangeType(reader2["endmap"], TypeCode.Int32);
                                }
                            }
                        }
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM item_pet_data WHERE id=@id";
                            command2.Parameters.AddWithValue("@id", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    data.Pet = new ItemData.ItemPetData();
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") data.Pet.Flags = (ItemData.ItemPetData.EItemPetFlags)Enum.Parse(typeof(ItemData.ItemPetData.EItemPetFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    data.Pet.Hunger = (byte)Convert.ChangeType(reader2["hunger"], TypeCode.Byte);
                                    data.Pet.Life = (byte)Convert.ChangeType(reader2["life"], TypeCode.Byte);
                                    data.Pet.LimitedLife = (ushort)Convert.ChangeType(reader2["limited_life"], TypeCode.UInt16);
                                    data.Pet.EvolutionItemIdentifier = (int)Convert.ChangeType(reader2["evolution_item"], TypeCode.Int32);
                                    data.Pet.RequiredLevelToEvolve = (byte)Convert.ChangeType(reader2["req_level_for_evolution"], TypeCode.Byte);
                                    data.Pet.Evolutions = new List<ItemData.ItemPetData.ItemPetEvolutionData>();
                                    using (MySqlConnection connection3 = new MySqlConnection(Program.Database))
                                    {
                                        connection3.Open();
                                        MySqlCommand command3 = connection3.CreateCommand();
                                        command3.CommandText = "SELECT * FROM item_pet_evolutions WHERE itemid=@itemid ORDER BY evolution_itemid ASC";
                                        command3.Parameters.AddWithValue("@itemid", data.Identifier);
                                        using (MySqlDataReader reader3 = command3.ExecuteReader())
                                        {
                                            while (reader3.Read())
                                            {
                                                ItemData.ItemPetData.ItemPetEvolutionData evolutionData = new ItemData.ItemPetData.ItemPetEvolutionData();
                                                evolutionData.NextEvolutionItemIdentifier = (int)Convert.ChangeType(reader3["evolution_itemid"], TypeCode.Int32);
                                                evolutionData.Chance = (ushort)Convert.ChangeType(reader3["chance"], TypeCode.UInt16);

                                                data.Pet.Evolutions.Add(evolutionData);
                                            }
                                        }
                                    }
                                    data.Pet.Interactions = new List<ItemData.ItemPetData.ItemPetInteractionData>();
                                    using (MySqlConnection connection3 = new MySqlConnection(Program.Database))
                                    {
                                        connection3.Open();
                                        MySqlCommand command3 = connection3.CreateCommand();
                                        command3.CommandText = "SELECT * FROM item_pet_interactions WHERE itemid=@itemid ORDER BY command ASC";
                                        command3.Parameters.AddWithValue("@itemid", data.Identifier);
                                        using (MySqlDataReader reader3 = command3.ExecuteReader())
                                        {
                                            while (reader3.Read())
                                            {
                                                ItemData.ItemPetData.ItemPetInteractionData interactionData = new ItemData.ItemPetData.ItemPetInteractionData();
                                                interactionData.Closeness = (byte)Convert.ChangeType(reader3["closeness"], TypeCode.Byte);
                                                interactionData.Success = (byte)Convert.ChangeType(reader3["success"], TypeCode.Byte);

                                                data.Pet.Interactions.Add(interactionData);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM item_rechargeable_data WHERE itemid=@itemid";
                            command2.Parameters.AddWithValue("@itemid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    data.Recharge = new ItemData.ItemRechargeData();
                                    data.Recharge.Price = (float)Convert.ChangeType(reader2["unit_price"], TypeCode.Single);
                                    data.Recharge.WeaponAttack = (byte)Convert.ChangeType(reader2["weapon_attack"], TypeCode.Byte);
                                }
                            }
                        }
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM item_scroll_data WHERE itemid=@itemid";
                            command2.Parameters.AddWithValue("@itemid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    data.Scroll = new ItemData.ItemScrollData();
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") data.Scroll.Flags = (ItemData.ItemScrollData.EItemScrollFlags)Enum.Parse(typeof(ItemData.ItemScrollData.EItemScrollFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    data.Scroll.Success = (byte)Convert.ChangeType(reader2["success"], TypeCode.Byte);
                                    data.Scroll.BreakItem = (byte)Convert.ChangeType(reader2["break_item"], TypeCode.Byte);
                                    data.Scroll.Strength = (byte)Convert.ChangeType(reader2["istr"], TypeCode.Byte);
                                    data.Scroll.Dexterity = (byte)Convert.ChangeType(reader2["idex"], TypeCode.Byte);
                                    data.Scroll.Intellect = (byte)Convert.ChangeType(reader2["iint"], TypeCode.Byte);
                                    data.Scroll.Luck = (byte)Convert.ChangeType(reader2["iluk"], TypeCode.Byte);
                                    data.Scroll.HP = (byte)Convert.ChangeType(reader2["ihp"], TypeCode.Byte);
                                    data.Scroll.MP = (byte)Convert.ChangeType(reader2["imp"], TypeCode.Byte);
                                    data.Scroll.WeaponAttack = (byte)Convert.ChangeType(reader2["iwatk"], TypeCode.Byte);
                                    data.Scroll.MagicAttack = (byte)Convert.ChangeType(reader2["imatk"], TypeCode.Byte);
                                    data.Scroll.WeaponDefense = (byte)Convert.ChangeType(reader2["iwdef"], TypeCode.Byte);
                                    data.Scroll.MagicDefense = (byte)Convert.ChangeType(reader2["imdef"], TypeCode.Byte);
                                    data.Scroll.Accuracy = (byte)Convert.ChangeType(reader2["iacc"], TypeCode.Byte);
                                    data.Scroll.Avoidance = (byte)Convert.ChangeType(reader2["iavo"], TypeCode.Byte);
                                    data.Scroll.Speed = (byte)Convert.ChangeType(reader2["ispeed"], TypeCode.Byte);
                                    data.Scroll.Jump = (byte)Convert.ChangeType(reader2["ijump"], TypeCode.Byte);
                                    data.Scroll.Targets = new List<int>();
                                    using (MySqlConnection connection3 = new MySqlConnection(Program.Database))
                                    {
                                        connection3.Open();
                                        MySqlCommand command3 = connection3.CreateCommand();
                                        command3.CommandText = "SELECT * FROM item_scroll_targets WHERE scrollid=@scrollid ORDER BY id ASC";
                                        command3.Parameters.AddWithValue("@scrollid", data.Identifier);
                                        using (MySqlDataReader reader3 = command3.ExecuteReader())
                                        {
                                            while (reader3.Read()) data.Scroll.Targets.Add((int)Convert.ChangeType(reader3["req_itemid"], TypeCode.Int32));
                                        }
                                    }
                                }
                            }
                        }
                        data.Morphs = new List<ItemData.ItemMorphData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM item_random_morphs WHERE itemid=@itemid ORDER BY morphid ASC";
                            command2.Parameters.AddWithValue("@itemid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    ItemData.ItemMorphData morphData = new ItemData.ItemMorphData();
                                    morphData.Morph = (byte)Convert.ChangeType(reader2["morphid"], TypeCode.Byte);
                                    morphData.Success = (byte)Convert.ChangeType(reader2["success"], TypeCode.Byte);

                                    data.Morphs.Add(morphData);
                                }
                            }
                        }
                        data.Skills = new List<ItemData.ItemSkillData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM item_skills WHERE itemid=@itemid ORDER BY skillid ASC";
                            command2.Parameters.AddWithValue("@itemid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    ItemData.ItemSkillData skillData = new ItemData.ItemSkillData();
                                    skillData.SkillIdentifier = (int)Convert.ChangeType(reader2["skillid"], TypeCode.Int32);
                                    skillData.SkillLevel = (byte)Convert.ChangeType(reader2["required_skill_level"], TypeCode.Byte);
                                    skillData.MasterSkillLevel = (byte)Convert.ChangeType(reader2["master_level"], TypeCode.Byte);

                                    data.Skills.Add(skillData);
                                }
                            }
                        }
                        data.Summons = new List<ItemData.ItemSummonData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM item_summons WHERE itemid=@itemid ORDER BY id ASC";
                            command2.Parameters.AddWithValue("@itemid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    ItemData.ItemSummonData summonData = new ItemData.ItemSummonData();
                                    summonData.MobIdentifier = (int)Convert.ChangeType(reader2["mobid"], TypeCode.Int32);
                                    summonData.Chance = (byte)Convert.ChangeType(reader2["chance"], TypeCode.Byte);

                                    data.Summons.Add(summonData);
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
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", "ItemData", dataCount, timer.Duration);
        }
    }
}
