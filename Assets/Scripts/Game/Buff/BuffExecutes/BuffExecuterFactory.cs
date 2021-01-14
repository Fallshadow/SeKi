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
    }
}
