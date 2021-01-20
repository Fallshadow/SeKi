using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigTable
{
    public class BuffData
    {
        public bool IsNull;
        // ID
        public int buff_id;
        // Buff名称
        public int title;
        // Buff描述
        public int desc;
        // Buff图标
        public int icon;
        // 是否显示Buff图标
        public int show_icon;
        // 文字提示
        public int message_id;
        // 种类
        public int buff_sort;
        // 类型
        public int buff_type;
        // 组别
        public int group_id;
        // 生命周期 格式是毫秒
        public int exist_time;
        // 生命周期计时方式
        public int countdown_type;
        // 冷却时间
        public int action_cd;
        // 触发事件
        public int event_config1;
        // 触发事件参数1
        public int event_config1_param1;
        // 触发事件参数2
        public int event_config1_param2;
        // 触发事件参数3
        public int event_config1_param3;
        // 事件交互目标状态条件
        public int event_target_condition;
        // 载体状态条件
        public string[] carrier_conditions;
        // 作用目标类型
        public int target_type;
        // 作用范围
        public int target_range;
        // 作用区域id
        public int target_area_id;
        // 目标状态条件
        public int target_conditions;
        // 最大作用数量
        public int target_maxnumber;
        // 目标优先级
        public int select_rule;
        // 作用效果生效几率
        public int effect_probability;
        // 属性效果
        public ConfigTable.BuffAttrConfig[] attr_effects;
        // 特殊作用效果目标状态条件
        public int effect_target_condition;
        // 特殊作用效果
        public BuffEffectConfig[] element_effects;
        // 移除作用效果
        public BuffEffectConfig[] remove_effects;
        // buff挂载时音效
        public int sound_done;
        // buff作用时音效
        public int sound_effect;
        // buff移除时音效
        public int sound_remove;
        // 同源组内冲突
        public int group_conflict;
        // 同源自身冲突
        public int self_conflict;
        // 不同源组内冲突
        public int diff_group_confict;
        // 不同源自身冲突
        public int diff_self_conflict;
        // 切换场景移除
        public int remove_rule1;
        // 载体死亡移除
        public int remove_rule2;
        // 完成关卡移除
        public int remove_rule3;
        // 更换装备移除
        public int remove_rule4;
        // 最大叠加层数
        public int max_addtive_layer;
        // 叠加额外效果层数
        public int addtive_effect_layer;
        // 叠加额外效果类型
        public ConfigTable.BuffEffectConfig addtive_effect;
        // 限定BuffFlag
        public string[] state_flags;
        // BuffFlag变更时移除
        public int stage_change_remove;
        // Buff提前结束类型
        public int buff_finish_type;
        // Buff提前结束参数1 代表BUFF提前结束所需次数
        public int buff_finish_param1;
        // Buff提前结束参数2
        public int buff_finish_param2;

        public BuffData(int ID)
        {
            buff_id = ID;
        }

        public static BuffData ByID(int configID)
        {
            BuffData buffData = new BuffData(0);
            return buffData;
        }
    }
}

