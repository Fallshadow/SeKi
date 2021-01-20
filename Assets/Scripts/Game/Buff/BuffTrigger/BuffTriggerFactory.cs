using System;
using System.Collections.Generic;
using ASeKi.Utility;
using constants;

namespace ASeKi.battle
{
    class BuffTriggerFactory
    {
        static Dictionary<BuffCarrierTrigger, Type> types = new Dictionary<BuffCarrierTrigger, Type>();
        static Dictionary<BuffCarrierTrigger, ObjectPool<object>> pools = new Dictionary<BuffCarrierTrigger, ObjectPool<object>>();

        static BuffTriggerFactory()
        {
            types.Add(BuffCarrierTrigger.ATTACH, typeof(BuffTriggerAttach));
            //types.Add(BuffCarrierTrigger.DESTORY_PART, typeof(BuffTriggerDestoryPart));
            //types.Add(BuffCarrierTrigger.INTERVAL, typeof(BuffTriggerInterval));
            //types.Add(BuffCarrierTrigger.SKILL_CAST, typeof(BuffTriggerSkillCast));
            //types.Add(BuffCarrierTrigger.SKILL_HITTED, typeof(BuffTriggerSkillHitted));
        }

        public static BuffTrigger Create(BuffCarrierTrigger triggerType, Buff buff, bool sendAttachRequest = false)
        {
            ObjectPool<object> pool = null;
            if(!pools.TryGetValue(triggerType,out pool))
            {
                pool = new ObjectPool<object>(() => 
                {
                    return Activator.CreateInstance(types[triggerType]);
                });
                pools.Add(triggerType, pool);
            }

            BuffTrigger buffTrigger = pool.Get() as BuffTrigger;
            buffTrigger?.Init(buff, pool);
            return buffTrigger;
        }
    }
}
