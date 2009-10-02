using Chronicle.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCDB2BIN
{
    internal static class NPCExport
    {
        public static void Export(BinaryWriter pWriter)
        {
            PerformanceTimer timer = new PerformanceTimer();
            long dataCount = 0;
            timer.Unpause();

            List<NPCData> datas = new List<NPCData>();
            using (MySqlConnection connection1 = new MySqlConnection(Program.Database))
            {
                connection1.Open();
                MySqlCommand command1 = connection1.CreateCommand();
                command1.CommandText = "SELECT * FROM npc_data ORDER BY npcid ASC";
                using (MySqlDataReader reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        NPCData data = new NPCData();
                        data.Identifier = (int)Convert.ChangeType(reader1["npcid"], TypeCode.Int32);
                        if ((string)Convert.ChangeType(reader1["flags"], TypeCode.String) != "") data.Flags = (NPCData.ENPCFlags)Enum.Parse(typeof(NPCData.ENPCFlags), (string)Convert.ChangeType(reader1["flags"], TypeCode.String), true);
                        data.StorageCost = (ushort)Convert.ChangeType(reader1["storage_cost"], TypeCode.UInt16);
                        data.Shops = new List<NPCData.NPCShopData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM shop_data WHERE npcid=@npcid ORDER BY rechargetier ASC";
                            command2.Parameters.AddWithValue("@npcid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    NPCData.NPCShopData shopData = new NPCData.NPCShopData();
                                    shopData.ShopIdentifier = (int)Convert.ChangeType(reader2["shopid"], TypeCode.Int32);
                                    shopData.RechargeTier = (byte)Convert.ChangeType(reader2["rechargetier"], TypeCode.Byte);
                                    shopData.Items = new List<NPCData.NPCShopData.NPCShopItemData>();
                                    using (MySqlConnection connection3 = new MySqlConnection(Program.Database))
                                    {
                                        connection3.Open();
                                        MySqlCommand command3 = connection3.CreateCommand();
                                        command3.CommandText = "SELECT * FROM shop_items WHERE shopid=@shopid ORDER BY sort ASC";
                                        command3.Parameters.AddWithValue("@shopid", shopData.ShopIdentifier);
                                        using (MySqlDataReader reader3 = command3.ExecuteReader())
                                        {
                                            while (reader3.Read())
                                            {
                                                NPCData.NPCShopData.NPCShopItemData itemData = new NPCData.NPCShopData.NPCShopItemData();
                                                itemData.ItemIdentifier = (int)Convert.ChangeType(reader3["itemid"], TypeCode.Int32);
                                                itemData.Quantity = (ushort)Convert.ChangeType(reader3["quantity"], TypeCode.UInt16);
                                                itemData.Price = (int)Convert.ChangeType(reader3["price"], TypeCode.Int32);
                                                using (MySqlConnection connection4 = new MySqlConnection(Program.Database))
                                                {
                                                    connection4.Open();
                                                    MySqlCommand command4 = connection4.CreateCommand();
                                                    command4.CommandText = "SELECT * FROM shop_recharge_data WHERE tierid=@tierid AND itemid=@itemid";
                                                    command4.Parameters.AddWithValue("@tierid", shopData.RechargeTier);
                                                    command4.Parameters.AddWithValue("@itemid", itemData.ItemIdentifier);
                                                    using (MySqlDataReader reader4 = command4.ExecuteReader())
                                                    {
                                                        if (reader4.Read())
                                                        {
                                                            itemData.RechargePrice = (float)Convert.ChangeType(reader4["price"], TypeCode.Single);
                                                        }
                                                    }
                                                }

                                                shopData.Items.Add(itemData);
                                            }
                                        }
                                    }

                                    data.Shops.Add(shopData);
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
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", "NPCData", dataCount, timer.Duration);
        }
    }
}
