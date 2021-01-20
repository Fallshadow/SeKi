using System.Collections.Generic;

namespace ASeKi.battle
{
    class ImmuneEffectExecuter : BuffActionExecuter
    {
        protected override void executeInternal()
        {
            BuffManager.instance.SetImmuneEffect(units[0], Config.element_param1, true);
        }

        protected override void releaseWithoutNet()
        {
            BuffManager.instance.SetImmuneEffect(units[0], Config.element_param1, false);
        }
    }
}
