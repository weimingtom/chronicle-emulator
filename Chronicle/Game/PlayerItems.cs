using Chronicle.Enums;
using Chronicle.Network;
using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerItems
    {
        private int mMesos;
        private PlayerItem[] mEquipped;
        private PlayerItem[] mCashEquipped;
        private PlayerItem[][] mItems;

        internal PlayerItems(int pMesos, byte[] pSlots, DatabaseQuery pQuery)
        {
            mMesos = pMesos;
            mEquipped = new PlayerItem[(byte)EEquipmentSlot.Count];
            mCashEquipped = new PlayerItem[(byte)EEquipmentSlot.Count];
            mItems = new PlayerItem[pSlots.Length][];
            for (int index = 0; index < pSlots.Length; ++index) mItems[index] = new PlayerItem[pSlots[index]];
            while (pQuery.NextRow())
            {
                PlayerItem item = new PlayerItem(pQuery);
                if (item.InventoryType == EInventoryType.Equipment && item.InventorySlot < 0)
                {
                    if (item.InventorySlot < -100) mCashEquipped[(-item.InventorySlot) - 100] = item;
                    else mEquipped[-item.InventorySlot] = item;
                }
                else mItems[(byte)item.InventoryType][item.InventorySlot] = item;
            }
        }

        internal void WriteInitial(Packet pPacket)
        {
            pPacket.WriteInt(mMesos);
            Array.ForEach(mItems, a => pPacket.WriteByte((byte)a.Length));
            Array.ForEach(mEquipped, i => { if (i != null) i.WriteGeneral(pPacket, false); });
            pPacket.WriteByte(0x00);
            Array.ForEach(mCashEquipped, i => { if (i != null) i.WriteGeneral(pPacket, false); });
            pPacket.WriteByte(0x00);
            Array.ForEach(mItems[(byte)EInventoryType.Equipment], i => { if (i != null) i.WriteGeneral(pPacket, false); });
            pPacket.WriteByte(0x00);
            Array.ForEach(mItems[(byte)EInventoryType.Use], i => { if (i != null) i.WriteGeneral(pPacket, false); });
            pPacket.WriteByte(0x00);
            Array.ForEach(mItems[(byte)EInventoryType.Setup], i => { if (i != null) i.WriteGeneral(pPacket, false); });
            pPacket.WriteByte(0x00);
            Array.ForEach(mItems[(byte)EInventoryType.Etc], i => { if (i != null) i.WriteGeneral(pPacket, false); });
            pPacket.WriteByte(0x00);
            Array.ForEach(mItems[(byte)EInventoryType.Cash], i => { if (i != null) i.WriteGeneral(pPacket, false); });
            pPacket.WriteByte(0x00);
        }

        internal void WriteEquipment(Packet pPacket)
        {
            for (byte index = 0; index < (byte)EEquipmentSlot.Count; ++index)
            {
                if (mEquipped[index] == null && mCashEquipped[index] == null) continue;
                if (index == (byte)EEquipmentSlot.Weapon && mEquipped[index] != null) pPacket.WriteInt(mEquipped[index].ItemIdentifier);
                else if (mCashEquipped[index] != null) pPacket.WriteInt(mCashEquipped[index].ItemIdentifier);
                else if (mEquipped[index] != null) pPacket.WriteInt(mEquipped[index].ItemIdentifier);
            }
            pPacket.WriteByte(0xFF);
            for (byte index = 0; index < (byte)EEquipmentSlot.Count; ++index)
            {
                if (index == (byte)EEquipmentSlot.Weapon || mEquipped[index] == null || mCashEquipped[index] == null) continue;
                pPacket.WriteInt(mEquipped[index].ItemIdentifier);
            }
            pPacket.WriteByte(0xFF);
            pPacket.WriteInt(mCashEquipped[(byte)EEquipmentSlot.Weapon] == null ? 0 : mCashEquipped[(byte)EEquipmentSlot.Weapon].ItemIdentifier);
        }
    }
}
