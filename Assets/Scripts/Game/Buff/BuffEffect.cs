using System.Collections.Generic;

namespace ASeKi.battle
{
    public class BuffEffect
    {
        const float PROB_RATIO = 100;
        Buff parent = null;
        List<BuffExecuter> buffExecuters = new List<BuffExecuter>();

        public void Effect(ulong targetId, bool satisfy = true)
        {
            //概率不通过
            if(UnityEngine.Random.Range(0, 100.0f) > 1.0f * parent.Config.effect_probability / PROB_RATIO)
            {
                return;
            }

            //满足条件则进行效果触发
            if(satisfy)
            {
                if(buffExecuters.Count > 0 && ((int)parent.Config.event_config1 == (int)constants.BuffCarrierTrigger.SATISFY_CARRIOR_CONDITION
                    || (int)parent.Config.event_config1 == (int)constants.BuffCarrierTrigger.LIFE_MAINTAIN))
                {
                    // 不是很懂
                }
                else
                {
                    initBaseAttr();
                    triggerAction(targetId);
                }

            }
            //否则将效果移除
            else
            {
                for(int i = 0; i < buffExecuters.Count; i++)
                {
                    buffExecuters[i].Release();
                }
                buffExecuters.Clear();
            }
        }

        // 这边看看BUFF是否有应用到实体属性上的效果，如果有的话就应用上
        void initBaseAttr()
        {
            // 先假定角色和怪物的等级都是1
            //Entity unit = battle.BattleActorManager.instance.GetActorById(parent.Carrier);

            //for(int i = 0; i < parent.Config.attr_effects.Length; i++)
            //{
            //    if(parent.Config.attr_effects[i].attr_config <= 0)
            //    {
            //        continue;
            //    }

            //    AttrExecuter exer = BuffExecuterFactory.GetAttrExecuter() as AttrExecuter;
            //    exer.Init(parent.Config.attr_effects[i], parent);
            //    exer.index = buffExecuters.Count;
            //    exer.Execute();
            //    buffExecuters.Add(exer);
            //}
        }

        // 触发实体行为继承BuffActionExecuter以达到效果
        void triggerAction(ulong targetId)
        {
            //for(int i = 0; i < parent.Config.element_effects.Length; i++)
            //{
            //    checkElementEffect(parent.Config.element_effects[i], targetId);
            //}
        }

        //void checkElementEffect(Framework.StructElement config, ulong targetId)
        //{
        //    if(config.element_config > 0)
        //    {
        //        if(BuffManager.instance.CheckContainAntiEffect(parent.Carrier, (int)config.element_config))
        //        {
        //            return;
        //        }
        //        BuffActionExecuter exer = BuffExecuterFactory.GetExecuter(config.element_config) as BuffActionExecuter;
        //        exer.Init(config, parent, targetId);
        //        exer.index = buffExecuters.Count;
        //        exer.Execute();
        //        buffExecuters.Add(exer);

        //    }
        //}
    }
}
