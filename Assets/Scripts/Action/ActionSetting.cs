namespace ASeKi.action
{
    public enum ActionRoleType
    {
        ART_PLAYER_NONE = -1,

        // Player
        ART_PLAYER_SWORD = 0,
        ART_PLAYER_HAMMER = 1,
        ART_PLAYER_DUALBLADE = 2,
        ART_PLAYER_BOWGUN = 3,

        // Other
        ART_PLAYER_NORMAL = 5,
        ART_PLAYER_CHARACTER_CUSTOM = 6,

        // PlayerNewWeapon
        // (old data has ScriptObject ref)
        // So add in new index!
        ART_PLAYER_SPEAR = 11,
        
        ART_WEAPON_SWORD     = 51,
        ART_WEAPON_HAMMER    = 52,
        ART_WEAPON_DUALBLADE = 53,
        ART_WEAPON_BOWGUN    = 54,
        ART_WEAPON_SPEAR     = 55,
        
        // Monster
        ART_MONSTER_101 = 101,
        ART_MONSTER_102 = 102,
        ART_MONSTER_103 = 103,
        ART_MONSTER_104 = 104,
        ART_MONSTER_105 = 105,
        ART_MONSTER_1051 = 1051,
        ART_MONSTER_1052 = 1052,
        ART_MONSTER_106 = 106,
        ART_MONSTER_107 = 107,
        ART_MONSTER_108 = 108,
        ART_MONSTER_109 = 109,
        ART_MONSTER_110 = 110,

        /// <summary>
        /// 木樁怪
        /// </summary>
        ART_MONSTER_999 = 999,

        ART_MONSTER_1000 = 1000,
        ART_MONSTER_1001 = 1001,
        ART_MONSTER_1002 = 1002,
        ART_MONSTER_1003 = 1003,
        ART_MONSTER_1004 = 1004,
        ART_MONSTER_1005 = 1005,
        ART_MONSTER_1006 = 1006,
        ART_MONSTER_1007 = 1007,
        ART_MONSTER_1008 = 1008,
        ART_MONSTER_1009 = 1009,
        ART_MONSTER_1010 = 1010,

        ART_MONSTER_1011 = 1011,
        ART_MONSTER_1012 = 1012,
        ART_MONSTER_1013 = 1013,
        ART_MONSTER_1014 = 1014,
        ART_MONSTER_1015 = 1015,
        ART_MONSTER_1016 = 1016,
        ART_MONSTER_1017 = 1017,
        ART_MONSTER_1018 = 1018,
        ART_MONSTER_1019 = 1019,
        ART_MONSTER_1020 = 1020,
        
        //PET
        ART_PET_10000 = 10000,
    }

    /// <summary>
    /// 用於統整 Action 所需要的配置, 以後增加角色可以直接在此操作, 包含 SO 與 Controller
    /// (PS.如果有少部份還得在各處代碼追加, 請在這裡整理出SOP)
    /// </summary>
    public class ActionSetting
    {
        /// <summary>
        /// 取得 ActionRoleType By WeaponType
        /// </summary>
        /// <param name="weaponType"></param>
        /// <returns></returns>
        public static ActionRoleType GetActionRoleType(WeaponType weaponType)
        {
            switch (weaponType)
            {
                case WeaponType.WT_SWORD: return ActionRoleType.ART_PLAYER_SWORD;
                case WeaponType.WT_HAMMER: return ActionRoleType.ART_PLAYER_HAMMER;
                case WeaponType.WT_DUAL_BLADE: return ActionRoleType.ART_PLAYER_DUALBLADE;
                case WeaponType.WT_BOW_GUN: return ActionRoleType.ART_PLAYER_BOWGUN;
                case WeaponType.WT_SPEAR: return ActionRoleType.ART_PLAYER_SPEAR;
            }

            debug.PrintSystem.LogError("Can't Find RoleType!!");
            return ActionRoleType.ART_PLAYER_NONE;
        }
    }
}