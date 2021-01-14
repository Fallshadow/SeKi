namespace ASeKi.battle
{
    public static class BuffSourceUtility
    {    
        // 用来判断是否由本机处理

        public static bool CheckNativeLogic(ulong entityId = 0)
        {
            //Hero mainPlayer = BattleActorManager.instance.MainPlayer;
            //if(mainPlayer != null && entityId == mainPlayer.ID)
            //{
            //    return true;
            //}

            //if(!Model.Battle.isHost)
            //{
            //    return false;
            //}

            //Entity entity = BattleActorManager.instance.GetActorById(entityId);
            //if(entity is Monster || entity is InteractObject)
            //{
            //    return true;
            //}
            //return false;
            return true;
        }
    }
}
