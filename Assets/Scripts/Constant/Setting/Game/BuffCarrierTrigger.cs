using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace constants
{
    public enum BuffCarrierTrigger : int
    {
        // 挂载时
        ATTACH = 0,
        // 部位破坏
        DESTORY_PART = 1,
        // 命中时
        SKILL_HITTED = 2,
        // 技能释放时
        SKILL_CAST = 3,
        // 满足条件时
        SATISFY_CARRIOR_CONDITION = 7,
        // 指定时间未受伤（在主城）
        LIFE_MAINTAIN = 13,
        // 满足StateFlag条件时
        SATISFY_FLAG_CONDITION = 19,
        // 间隔触发
        INTERVAL = 100,
    }
}
