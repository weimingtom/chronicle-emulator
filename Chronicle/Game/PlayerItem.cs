using Chronicle.Data;
using Chronicle.Enums;
using Chronicle.Network;
using Chronicle.Utility;
using System;

namespace Chronicle.Game
{
    public sealed class PlayerItem
    {
        private int mPlayerIdentifier;
        private EInventoryType mInventoryType;
        private short mInventorySlot;
        private int mItemIdentifier;
        private byte mUnusedScrollSlots;
        private byte mUsedScrollSlots;
        private ushort mStrength;
        private ushort mDexterity;
        private ushort mIntellect;
        private ushort mLuck;
        private ushort mHealth;
        private ushort mMana;
        private ushort mWeaponAttack;
        private ushort mMagicAttack;
        private ushort mWeaponDefense;
        private ushort mMagicDefense;
        private ushort mAccuracy;
        private ushort mAvoidance;
        private ushort mHands;
        private ushort mSpeed;
        private ushort mJump;
        private ushort mQuantity;
        private string mOwner;
        private ushort mFlags;

        internal PlayerItem(DatabaseQuery pQuery)
        {
            mPlayerIdentifier = (int)pQuery["player_identifier"];
            mInventoryType = (EInventoryType)(byte)pQuery["inventory_type"];
            mInventorySlot = (short)pQuery["inventory_slot"];
            mItemIdentifier = (int)pQuery["item_identifier"];
            mUnusedScrollSlots = (byte)pQuery["unused_scroll_slots"];
            mUsedScrollSlots = (byte)pQuery["used_scroll_slots"];
            mStrength = (ushort)pQuery["strength"];
            mDexterity = (ushort)pQuery["dexterity"];
            mIntellect = (ushort)pQuery["intellect"];
            mLuck = (ushort)pQuery["luck"];
            mHealth = (ushort)pQuery["health"];
            mMana = (ushort)pQuery["mana"];
            mWeaponAttack = (ushort)pQuery["weapon_attack"];
            mMagicAttack = (ushort)pQuery["magic_attack"];
            mWeaponDefense = (ushort)pQuery["weapon_defense"];
            mMagicDefense = (ushort)pQuery["magic_defense"];
            mAccuracy = (ushort)pQuery["accuracy"];
            mAvoidance = (ushort)pQuery["avoidance"];
            mHands = (ushort)pQuery["hands"];
            mSpeed = (ushort)pQuery["speed"];
            mJump = (ushort)pQuery["jump"];
            mQuantity = (ushort)pQuery["quantity"];
            mOwner = (string)pQuery["owner"];
            mFlags = (ushort)pQuery["flags"];
        }

        public int PlayerIdentifier { get { return mPlayerIdentifier; } set { mPlayerIdentifier = value; } }
        public EInventoryType InventoryType { get { return mInventoryType; } set { mInventoryType = value; } }
        public short InventorySlot { get { return mInventorySlot; } set { mInventorySlot = value; } }
        public int ItemIdentifier { get { return mItemIdentifier; } set { mItemIdentifier = value; } }
        public byte UnusedScrollSlots { get { return mUnusedScrollSlots; } set { mUnusedScrollSlots = value; } }
        public byte UsedScrollSlots { get { return mUsedScrollSlots; } set { mUsedScrollSlots = value; } }
        public ushort Strength { get { return mStrength; } set { mStrength = value; } }
        public ushort Dexterity { get { return mDexterity; } set { mDexterity = value; } }
        public ushort Intellect { get { return mIntellect; } set { mIntellect = value; } }
        public ushort Luck { get { return mLuck; } set { mLuck = value; } }
        public ushort Health { get { return mHealth; } set { mHealth = value; } }
        public ushort Mana { get { return mMana; } set { mMana = value; } }
        public ushort WeaponAttack { get { return mWeaponAttack; } set { mWeaponAttack = value; } }
        public ushort MagicAttack { get { return mMagicAttack; } set { mMagicAttack = value; } }
        public ushort WeaponDefense { get { return mWeaponDefense; } set { mWeaponDefense = value; } }
        public ushort MagicDefense { get { return mMagicDefense; } set { mMagicDefense = value; } }
        public ushort Accuracy { get { return mAccuracy; } set { mAccuracy = value; } }
        public ushort Avoidance { get { return mAvoidance; } set { mAvoidance = value; } }
        public ushort Hands { get { return mHands; } set { mHands = value; } }
        public ushort Speed { get { return mSpeed; } set { mSpeed = value; } }
        public ushort Jump { get { return mJump; } set { mJump = value; } }
        public ushort Quantity { get { return mQuantity; } set { mQuantity = value; } }
        public string Owner { get { return mOwner; } set { mOwner = value; } }
        public ushort Flags { get { return mFlags; } set { mFlags = value; } }

        public void WriteGeneral(Packet pPacket, bool pRealSlot)
        {
            if (mInventorySlot != 0)
            {
                if (pRealSlot) pPacket.WriteShort(mInventorySlot);
                else
                {
                    byte slot = (byte)Math.Abs(mInventorySlot);
                    if (slot > 100) slot -= 100;
                    pPacket.WriteByte(slot);
                }
            }
            pPacket.WriteByte((byte)(mInventoryType == EInventoryType.Equipment ? 0x01 : 0x02));
            pPacket.WriteInt(mItemIdentifier);
            pPacket.WriteByte(0x00);
            pPacket.WriteLong(0);
            if (mInventoryType == EInventoryType.Equipment)
            {
                pPacket.WriteByte(mUnusedScrollSlots);
                pPacket.WriteByte(mUsedScrollSlots);
                pPacket.WriteUShort(mStrength);
                pPacket.WriteUShort(mDexterity);
                pPacket.WriteUShort(mIntellect);
                pPacket.WriteUShort(mLuck);
                pPacket.WriteUShort(mHealth);
                pPacket.WriteUShort(mMana);
                pPacket.WriteUShort(mWeaponAttack);
                pPacket.WriteUShort(mMagicAttack);
                pPacket.WriteUShort(mWeaponDefense);
                pPacket.WriteUShort(mMagicDefense);
                pPacket.WriteUShort(mAccuracy);
                pPacket.WriteUShort(mAvoidance);
                pPacket.WriteUShort(mHands);
                pPacket.WriteUShort(mSpeed);
                pPacket.WriteUShort(mJump);
                pPacket.WriteString(mOwner);
                pPacket.WriteUShort(mFlags);
                pPacket.WriteByte(0x00);
                pPacket.WriteByte(0x00);
                pPacket.WriteUShort(0);
                pPacket.WriteUShort(0);
                pPacket.WriteUInt(0);
                pPacket.WriteLong(-1);
                pPacket.WriteLong(0); // 0040E0FD3B374F01
                pPacket.WriteInt(-1);
            }
            else
            {
                pPacket.WriteUShort(mQuantity);
                pPacket.WriteString(mOwner);
                pPacket.WriteUShort(mFlags);
                if (ItemData.IsRechargeable(ItemData.GetType(mItemIdentifier))) pPacket.WriteLong(0);
            }
        }
    }
}
