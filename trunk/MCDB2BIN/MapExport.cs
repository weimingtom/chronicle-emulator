using Chronicle.Data;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;

namespace MCDB2BIN
{
    internal static class MapExport
    {
        public static void Export(BinaryWriter pWriter)
        {
            PerformanceTimer timer = new PerformanceTimer();
            long dataCount = 0;
            timer.Unpause();

            List<MapData> datas = new List<MapData>();
            using (MySqlConnection connection1 = new MySqlConnection(Program.Database))
            {
                connection1.Open();
                MySqlCommand command1 = connection1.CreateCommand();
                command1.CommandText = "SELECT COUNT(*) FROM map_data";
                Program.ResetCounter((int)(long)command1.ExecuteScalar());
                command1.CommandText = "SELECT * FROM map_data ORDER BY mapid ASC";
                using (MySqlDataReader reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        MapData data = new MapData();
                        data.Identifier = (int)Convert.ChangeType(reader1["mapid"], TypeCode.Int32);
                        if ((string)Convert.ChangeType(reader1["flags"], TypeCode.String) != "") data.Flags = (MapData.EMapFlags)Enum.Parse(typeof(MapData.EMapFlags), (string)Convert.ChangeType(reader1["flags"], TypeCode.String), true);
                        data.ShuffleName = (string)Convert.ChangeType(reader1["shuffle_name"], TypeCode.String);
                        data.Music = (string)Convert.ChangeType(reader1["default_bgm"], TypeCode.String);
                        data.MinLevelLimit = (byte)Convert.ChangeType(reader1["min_level_limit"], TypeCode.Byte);
                        data.TimeLimit = (ushort)Convert.ChangeType(reader1["time_limit"], TypeCode.UInt16);
                        data.RegenRate = (byte)Convert.ChangeType(reader1["regen_rate"], TypeCode.Byte);
                        data.Traction = (float)Convert.ChangeType(reader1["default_traction"], TypeCode.Single);
                        data.LeftTopX = (short)Convert.ChangeType(reader1["map_ltx"], TypeCode.Int16);
                        data.LeftTopY = (short)Convert.ChangeType(reader1["map_lty"], TypeCode.Int16);
                        data.RightBottomX = (short)Convert.ChangeType(reader1["map_rbx"], TypeCode.Int16);
                        data.RightBottomY = (short)Convert.ChangeType(reader1["map_rby"], TypeCode.Int16);
                        data.ReturnMapIdentifier = (int)Convert.ChangeType(reader1["return_map"], TypeCode.Int32);
                        data.ForcedReturnMapIdentifier = (int)Convert.ChangeType(reader1["forced_return_map"], TypeCode.Int32);
                        if ((string)Convert.ChangeType(reader1["field_type"], TypeCode.String) != "") data.FieldTypes = (MapData.EMapFieldType)Enum.Parse(typeof(MapData.EMapFieldType), (string)Convert.ChangeType(reader1["field_type"], TypeCode.String), true);
                        if ((string)Convert.ChangeType(reader1["field_limitations"], TypeCode.String) != "") data.FieldLimits = (MapData.EMapFieldLimit)Enum.Parse(typeof(MapData.EMapFieldLimit), (string)Convert.ChangeType(reader1["field_limitations"], TypeCode.String), true);
                        data.DecreaseHP = (byte)Convert.ChangeType(reader1["decrease_hp"], TypeCode.Byte);
                        data.DamagePerSecond = (ushort)Convert.ChangeType(reader1["damage_per_second"], TypeCode.UInt16);
                        data.ProtectItemIdentifier = (int)Convert.ChangeType(reader1["protect_item"], TypeCode.Int32);
                        data.MobRate = (float)Convert.ChangeType(reader1["mob_rate"], TypeCode.Single);
                        data.LinkIdentifier = (int)Convert.ChangeType(reader1["link"], TypeCode.Int32);
                        data.Footholds = new List<MapData.MapFootholdData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM map_footholds WHERE mapid=@mapid ORDER BY id ASC";
                            command2.Parameters.AddWithValue("@mapid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    MapData.MapFootholdData footholdData = new MapData.MapFootholdData();
                                    footholdData.Identifier = (ushort)Convert.ChangeType(reader2["id"], TypeCode.UInt16);
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") footholdData.Flags = (MapData.MapFootholdData.EMapFootholdFlags)Enum.Parse(typeof(MapData.MapFootholdData.EMapFootholdFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    footholdData.PreviousIdentifier = (ushort)Convert.ChangeType(reader2["prev"], TypeCode.UInt16);
                                    footholdData.NextIdentifier = (ushort)Convert.ChangeType(reader2["next"], TypeCode.UInt16);
                                    footholdData.DragForce = (short)Convert.ChangeType(reader2["drag_force"], TypeCode.Int16);
                                    footholdData.X1 = (short)Convert.ChangeType(reader2["x1"], TypeCode.Int16);
                                    footholdData.Y1 = (short)Convert.ChangeType(reader2["y1"], TypeCode.Int16);
                                    footholdData.X2 = (short)Convert.ChangeType(reader2["x2"], TypeCode.Int16);
                                    footholdData.Y2 = (short)Convert.ChangeType(reader2["y2"], TypeCode.Int16);

                                    data.Footholds.Add(footholdData);
                                }
                            }
                        }
                        data.NPCs = new List<MapData.MapNPCData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM map_life WHERE mapid=@mapid AND life_type='npc' ORDER BY id ASC";
                            command2.Parameters.AddWithValue("@mapid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    MapData.MapNPCData npcData = new MapData.MapNPCData();
                                    npcData.NPCIdentifier = (int)Convert.ChangeType(reader2["lifeid"], TypeCode.Int32);
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") npcData.Flags = (MapData.MapNPCData.EMapNPCFlags)Enum.Parse(typeof(MapData.MapNPCData.EMapNPCFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    npcData.Foothold = (ushort)Convert.ChangeType(reader2["foothold"], TypeCode.UInt16);
                                    npcData.X = (short)Convert.ChangeType(reader2["x"], TypeCode.Int16);
                                    npcData.Y = (short)Convert.ChangeType(reader2["y"], TypeCode.Int16);
                                    npcData.MinClickX = (short)Convert.ChangeType(reader2["min_click_pos"], TypeCode.Int16);
                                    npcData.MaxClickX = (short)Convert.ChangeType(reader2["max_click_pos"], TypeCode.Int16);

                                    data.NPCs.Add(npcData);
                                }
                            }
                        }
                        data.Reactors = new List<MapData.MapReactorData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM map_life WHERE mapid=@mapid AND life_type='reactor' ORDER BY id ASC";
                            command2.Parameters.AddWithValue("@mapid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    MapData.MapReactorData reactorData = new MapData.MapReactorData();
                                    reactorData.ReactorIdentifier = (int)Convert.ChangeType(reader2["lifeid"], TypeCode.Int32);
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") reactorData.Flags = (MapData.MapReactorData.EMapReactorFlags)Enum.Parse(typeof(MapData.MapReactorData.EMapReactorFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    reactorData.Foothold = (ushort)Convert.ChangeType(reader2["foothold"], TypeCode.UInt16);
                                    reactorData.X = (short)Convert.ChangeType(reader2["x"], TypeCode.Int16);
                                    reactorData.Y = (short)Convert.ChangeType(reader2["y"], TypeCode.Int16);
                                    reactorData.MinClickX = (short)Convert.ChangeType(reader2["min_click_pos"], TypeCode.Int16);
                                    reactorData.MaxClickX = (short)Convert.ChangeType(reader2["max_click_pos"], TypeCode.Int16);
                                    reactorData.RespawnTime = (int)Convert.ChangeType(reader2["respawn_time"], TypeCode.Int32);
                                    reactorData.Name = (string)Convert.ChangeType(reader2["name"], TypeCode.String);

                                    data.Reactors.Add(reactorData);
                                }
                            }
                        }
                        data.Mobs = new List<MapData.MapMobData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM map_life WHERE mapid=@mapid AND life_type='mob' ORDER BY id ASC";
                            command2.Parameters.AddWithValue("@mapid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    MapData.MapMobData mobData = new MapData.MapMobData();
                                    mobData.MobIdentifier = (int)Convert.ChangeType(reader2["lifeid"], TypeCode.Int32);
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") mobData.Flags = (MapData.MapMobData.EMapMobFlags)Enum.Parse(typeof(MapData.MapMobData.EMapMobFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    mobData.Foothold = (ushort)Convert.ChangeType(reader2["foothold"], TypeCode.UInt16);
                                    mobData.X = (short)Convert.ChangeType(reader2["x"], TypeCode.Int16);
                                    mobData.Y = (short)Convert.ChangeType(reader2["y"], TypeCode.Int16);
                                    mobData.MinClickX = (short)Convert.ChangeType(reader2["min_click_pos"], TypeCode.Int16);
                                    mobData.MaxClickX = (short)Convert.ChangeType(reader2["max_click_pos"], TypeCode.Int16);
                                    mobData.RespawnTime = (int)Convert.ChangeType(reader2["respawn_time"], TypeCode.Int32);
                                    mobData.Announcement = "";
                                    using (MySqlConnection connection3 = new MySqlConnection(Program.Database))
                                    {
                                        connection3.Open();
                                        MySqlCommand command3 = connection3.CreateCommand();
                                        command3.CommandText = "SELECT * FROM map_time_mob WHERE mapid=@mapid AND mobid=@mobid";
                                        command3.Parameters.AddWithValue("@mapid", data.Identifier);
                                        command3.Parameters.AddWithValue("@mobid", mobData.MobIdentifier);
                                        using (MySqlDataReader reader3 = command3.ExecuteReader())
                                        {
                                            if (reader3.Read())
                                            {
                                                mobData.StartHour = (byte)Convert.ChangeType(reader3["starthour"], TypeCode.Byte);
                                                mobData.EndHour = (byte)Convert.ChangeType(reader3["endhour"], TypeCode.Byte);
                                                mobData.Announcement = (string)Convert.ChangeType(reader3["message"], TypeCode.String);
                                            }
                                        }
                                    }

                                    data.Mobs.Add(mobData);
                                }
                            }
                        }
                        data.Portals = new List<MapData.MapPortalData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM map_portals WHERE mapid=@mapid ORDER BY id ASC";
                            command2.Parameters.AddWithValue("@mapid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    MapData.MapPortalData portalData = new MapData.MapPortalData();
                                    if ((string)Convert.ChangeType(reader2["flags"], TypeCode.String) != "") portalData.Flags = (MapData.MapPortalData.EMapPortalFlags)Enum.Parse(typeof(MapData.MapPortalData.EMapPortalFlags), (string)Convert.ChangeType(reader2["flags"], TypeCode.String), true);
                                    portalData.X = (short)Convert.ChangeType(reader2["x"], TypeCode.Int16);
                                    portalData.Y = (short)Convert.ChangeType(reader2["y"], TypeCode.Int16);
                                    portalData.Name = (string)Convert.ChangeType(reader2["name"], TypeCode.String);
                                    portalData.ToMapIdentifier = (int)Convert.ChangeType(reader2["tomap"], TypeCode.Int32);
                                    portalData.ToName = (string)Convert.ChangeType(reader2["toname"], TypeCode.String);
                                    portalData.Script = (string)Convert.ChangeType(reader2["script"], TypeCode.String);

                                    data.Portals.Add(portalData);
                                }
                            }
                        }
                        data.Seats = new List<MapData.MapSeatData>();
                        using (MySqlConnection connection2 = new MySqlConnection(Program.Database))
                        {
                            connection2.Open();
                            MySqlCommand command2 = connection2.CreateCommand();
                            command2.CommandText = "SELECT * FROM map_seats WHERE mapid=@mapid ORDER BY seatid ASC";
                            command2.Parameters.AddWithValue("@mapid", data.Identifier);
                            using (MySqlDataReader reader2 = command2.ExecuteReader())
                            {
                                while (reader2.Read())
                                {
                                    MapData.MapSeatData seatData = new MapData.MapSeatData();
                                    seatData.X = (short)Convert.ChangeType(reader2["x"], TypeCode.Int16);
                                    seatData.Y = (short)Convert.ChangeType(reader2["y"], TypeCode.Int16);

                                    data.Seats.Add(seatData);
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
            Console.WriteLine("| {0,-24} | {1,-16} | {2,-24} |", "MapData", dataCount, timer.Duration);
        }
    }
}
