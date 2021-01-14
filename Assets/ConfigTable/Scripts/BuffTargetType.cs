namespace ConfigTable
{
    public enum BuffTargetType : int
    {
        // 无
        NONE = 0,
        // 自己
        SELF = 1,
        // 敌人
        ENEMY = 2,
        // 友军
        FRIENDS = 3,
        // 自己+友军
        SELF_FRIENDS = 4,
        // 当前焦点1
        FOCUS = 5,
        // 怪物
        MONSTER = 6,
        // 玩家
        HERO = 7,
    }
}
