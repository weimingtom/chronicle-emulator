using System;
using System.Collections.Generic;
using System.IO;

namespace Chronicle.Data
{
    public sealed class NPCData
    {
        [Flags]
        public enum ENPCFlags : byte
        {
            None = 0 << 0,
            Maple_TV = 1 << 0,
            MapleTV = 1 << 0
        }

        public sealed class NPCShopData
        {
            public sealed class NPCShopItemData
            {
                public int ItemIdentifier { get; set; }
                public ushort Quantity { get; set; }
                public int Price { get; set; }
                public float RechargePrice { get; set; }

                public void Save(BinaryWriter pWriter)
                {
                    pWriter.Write(ItemIdentifier);
                    pWriter.Write(Quantity);
                    pWriter.Write(Price);
                    pWriter.Write(RechargePrice);
                }

                public void Load(BinaryReader pReader)
                {
                    ItemIdentifier = pReader.ReadInt32();
                    Quantity = pReader.ReadUInt16();
                    Price = pReader.ReadInt32();
                    RechargePrice = pReader.ReadSingle();
                }
            }


            public int ShopIdentifier { get; set; }
            public byte RechargeTier { get; set; }
            public List<NPCShopItemData> Items { get; set; }

            public void Save(BinaryWriter pWriter)
            {
                pWriter.Write(ShopIdentifier);
                pWriter.Write(RechargeTier);

                pWriter.Write(Items.Count);
                Items.ForEach(i => i.Save(pWriter));
            }

            public void Load(BinaryReader pReader)
            {
                ShopIdentifier = pReader.ReadInt32();
                RechargeTier = pReader.ReadByte();

                int itemsCount = pReader.ReadInt32();
                Items = new List<NPCShopItemData>(itemsCount);
                while (itemsCount-- > 0)
                {
                    NPCShopItemData item = new NPCShopItemData();
                    item.Load(pReader);
                    Items.Add(item);
                }
            }
        }


        public int Identifier { get; set; }
        public ENPCFlags Flags { get; set; }
        public ushort StorageCost { get; set; }
        public List<NPCShopData> Shops { get; set; }

        public void Save(BinaryWriter pWriter)
        {
            pWriter.Write(Identifier);
            pWriter.Write((byte)Flags);
            pWriter.Write(StorageCost);

            pWriter.Write(Shops.Count);
            Shops.ForEach(s => s.Save(pWriter));
        }

        public void Load(BinaryReader pReader)
        {
            Identifier = pReader.ReadInt32();
            Flags = (ENPCFlags)pReader.ReadByte();
            StorageCost = pReader.ReadUInt16();

            int shopsCount = pReader.ReadInt32();
            Shops = new List<NPCShopData>(shopsCount);
            while (shopsCount-- > 0)
            {
                NPCShopData shop = new NPCShopData();
                shop.Load(pReader);
                Shops.Add(shop);
            }
        }
    }
}
