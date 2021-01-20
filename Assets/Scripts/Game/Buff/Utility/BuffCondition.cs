namespace ASeKi.battle
{
    // 帮助判断游戏中的条件是否达成
    // TODO:这里应该用结构体代替string传递信息，日后请更正
    public static class BuffCondition
    {
        public static bool Valify(ulong entityId, string buffCondition)
        {
            Entity entity = battle.BattleActorManager.instance.GetActorById(entityId);
            return Valify(entity, buffCondition);
        }

        public static bool Valify(Entity entity, string buffCondition)
        {
            return true;
        }
    }
}
