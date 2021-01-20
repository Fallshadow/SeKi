namespace network.model
{
    public class BattleModel : Singleton<BattleModel>
    {
        public ASeKi.battle.BattleInfo m_BattleInfo = new ASeKi.battle.BattleInfo();
        public bool isHost = false;

    }
}
