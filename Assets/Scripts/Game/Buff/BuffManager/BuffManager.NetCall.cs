using System.Collections.Generic;

namespace ASeKi.battle
{
    // 用来放置由服务端发起调用的BUFF逻辑
    public partial class BuffManager : Singleton<BuffManager>
    {
        // buff生效 逻辑
        public void OnRecvBuffUseEvent(ulong carrierId, ulong dynamicId, int index, uint type, int value, IEnumerable<uint> ids)
        {
            Carrier carrier = getCarrier(carrierId);
            uint et = type & 1;
            uint detailType = type >> 1;
            bool succ = carrier.OnRecvBuffUseEvent(dynamicId, index, (int)detailType, value, ids);
            if(!succ)
            {
                //未找到buff的情况，自行处理一些逻辑
                if(et == 1)
                {
                    //if(detailType == (int)battle.ActionStatusType.HEAL_HP)
                    //{
                    //    foreach(uint id in ids)
                    //    {
                    //        battle.Hero hero = battle.BattleActorManager.instance.GetHeroById(id);
                    //        hero.HeroLogic.HeroAttrSystem.Heal(value);
                    //    }
                    //}
                }
            }
        }

        // buff生效 表现
        public void EffectBuffInternal(ulong carrierId, int buffId)
        {
            Carrier carrier = this.getCarrier(carrierId);
            carrier.EffectBuffInternal(buffId);
        }

    }
}
