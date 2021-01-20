namespace constants
{
    //会造成buff提前结束的因素
    public enum BuffFinishType : int
    {
        NONE = 1,                   // 无
        SET_DAMAGE_COUNT = 2,       // 造成X次伤害
        GET_DAMAGE_COUNT = 3,       // 受到X次伤害
        SKILL_COUNT = 4,            // 释放X次技能
    }
}
