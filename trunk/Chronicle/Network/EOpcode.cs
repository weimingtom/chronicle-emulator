namespace Chronicle.Network
{
    public enum EOpcode : ushort
    {
        SMSG_AUTHENTICATION             = 0x0000,
        SMSG_WORLD_STATUS               = 0x0003,
        SMSG_PIN                        = 0x0006,
        SMSG_ALL_PLAYER_LIST            = 0x0008,
        SMSG_WORLD_LIST                 = 0x000A,
        SMSG_PLAYER_LIST                = 0x000B,
        SMSG_CHANNEL_CONNECT            = 0x000C,
        SMSG_PLAYER_NAME_CHECK          = 0x000D,
        SMSG_PLAYER_CREATE              = 0x000E,
        SMSG_PLAYER_DELETE              = 0x000F,

        CMSG_AUTHENTICATION             = 0x0001,
        CMSG_WORLD_LIST_REFRESH         = 0x0004,
        CMSG_PLAYER_LIST                = 0x0005,
        CMSG_WORLD_STATUS               = 0x0006,
        CMSG_PIN                        = 0x0009,
        CMSG_WORLD_LIST                 = 0x000B,
        CMSG_ALL_PLAYER_LIST            = 0x000D,
        CMSG_ALL_PLAYER_CONNECT         = 0x000E,
        CMSG_PLAYER_CONNECT             = 0x0013,
        CMSG_PLAYER_NAME_CHECK          = 0x0015,
        CMSG_PLAYER_CREATE              = 0x0016,
        CMSG_PLAYER_DELETE              = 0x0018,
        CMSG_DISCONNECT                 = 0x00D5,


        //SMSG_INVENTORY_MOVE_ITEM        = 0x001A,
        SMSG_PLAYER_UPDATE              = 0x001C,
        SMSG_BUDDY_UPDATE               = 0x003C,
        SMSG_MESSAGE                    = 0x0041,
        SMSG_MACRO_LIST                 = 0x0071,
        SMSG_MAP_CHANGE                 = 0x0072,
        SMSG_MAP_FORCE_EQUIPMENT        = 0x0079,
        SMSG_MAP_EFFECT                 = 0x007E,
        SMSG_PLAYER_DETAILS             = 0x0091,
        SMSG_PLAYER_LEAVE               = 0x0092,
        SMSG_PLAYER_CHAT                = 0x0093,
        SMSG_PLAYER_MOVE                = 0x00A7,
        SMSG_PLAYER_EMOTE               = 0x00AF,
        SMSG_MOB_DETAILS                = 0x00D0,
        SMSG_MOB_CONTROL                = 0x00D2,
        SMSG_MOB_ACTION                 = 0x00D3,
        SMSG_MOB_ACTION_CONFIRM         = 0x00D4,
        SMSG_MOB_STATUS                 = 0x00D6,
        SMSG_NPC_DETAILS                = 0x00E3,
        SMSG_NPC_CONTROL                = 0x00E5,
        SMSG_NPC_ACTION                 = 0x00E6,
        SMSG_REACTOR_DETAILS            = 0x00F9,
        SMSG_KEYMAP                     = 0x012A,

        CMSG_PLAYER_LOAD                = 0x0014,
        CMSG_PLAYER_TELEPORT            = 0x0025,
        CMSG_PLAYER_MOVE                = 0x0028,
        //CMSG_PLAYER_ATTACK_MELEE        = 0x002B,
        //CMSG_PLAYER_DAMAGE              = 0x002F,
        CMSG_PLAYER_CHAT                = 0x0030,
        CMSG_PLAYER_EMOTE               = 0x0032,
        CMSG_PORTAL_SCRIPT_TRIGGER      = 0x0063,
        CMSG_MOB_ACTION                 = 0x00B2,
        CMSG_NPC_ACTION                 = 0x00BB,


        MSG_NONE                        = 0xFFFF
    }
}
