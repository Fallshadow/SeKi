using System.Collections.Generic;
using System.Collections.Generic;
using System;

namespace ASeKi.battle
{
    public static class BuffExecuterFactory
    {
        static Dictionary<constants.BuffEffectType, Type> types = new Dictionary<constants.BuffEffectType, Type>();
        static Dictionary<constants.BuffEffectType, Utility.ObjectPool<object>> pools = new Dictionary<constants.BuffEffectType, Utility.ObjectPool<object>>();

        static BuffExecuterFactory()
        {
            types.Add(constants.BuffEffectType.NONE, typeof(AttrExecuter));
        }

        public static BuffExecuter GetExecuter(constants.BuffEffectType config)
        {
            Utility.ObjectPool<object> pool = null;
            if(!pools.TryGetValue(config, out pool))
            {
                pool = new Utility.ObjectPool<object>(() =>
                {
                    return Activator.CreateInstance(types[config]);
                });
                pools.Add(config, pool);
            }
            BuffExecuter executer = pool.Get() as BuffExecuter;
            executer.SetPool(pool);
            return executer;
        }

        public static BuffExecuter GetAttrExecuter()
        {
            return GetExecuter(constants.BuffEffectType.NONE);
        }

        public static void Clear()
        {
            pools.Clear();
        }
    }
}
