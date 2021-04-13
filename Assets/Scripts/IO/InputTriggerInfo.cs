using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.input
{
    // NOTE: 這邊的Enum只能往下加不要改順序或插入
    // NOTE: Combine的Type從1024開始
    public enum ButtonType
    {
        NONE = 0,
        DIRECTION,
        CAMERA,


        // 用於紀錄戰鬥用起始枚舉數值
        // BATTLE_START = LIGHT_ATTACK,
        LIGHT_ATTACK,
        HEAVY_ATTACK,
        DODGE,
        LOCK_CAMERA,  // REVERT 改為 復位與鎖定共用鍵
        MONITOR,
        UP_ARROW,
        DWON_ARROW,
        LEFT_ARROW,
        RIGHT_ARROW,
        SWITCH_AREMD_STATE,
        RESTART,
        CHANGE_LOCK_CAMERA_TARGET,
        CHANGE_LOCK_CAMERA_PART,
        JUMP,
        CLIMB,
        SPECIAL_ATTACK,
        REVIVE,
        ACTIVE_PROP,
        CHANGE_PROP,
        KALLA_SKILL_A,
        KALLA_SKILL_B,
        GATHER_HERB,              // 采集草药按钮
        GATHER_ORE,               // 采集矿石按钮
        CANCEL_LIGHT_ATTACK,      // 取消蓄力
        SWITCH_WEAPON,            // 切換武器

        // 用於紀錄戰鬥用起始結束數值，若之後有增加，則此項更改
        // BATTLE_END = SWITCH_WEAPON,
        COMBINE_LIGHT_HEAVY_ATTACK = 1024,
        DRAG_FROM_HEAVY_ATTACK_TO_LIGHT_ATTACK = 1025,
    }
    
    // NOTE: 這邊的Enum只能往下加不要改順序或插入
    public enum PressType
    {
        NONE = 0,
        PRESS = 1,
        CLICK = 2,
        HOLD = 3,
        // HOLD_AND_RELEASE = 4,
        PRESS_OR_CLICK = 5,
        ROCKER = 6,
        COMBINE = 7,
        DROP = 8,
        SHORT_HOLD = 9,
        DRAG = 10,
        SHORT_HOLD_AND_RELEASE = 11,
        LONG_HOLD_AND_RELEASE = 12,
        CHARGE_HOLD = 13,
        RELEASE = 14
    }
    
}

