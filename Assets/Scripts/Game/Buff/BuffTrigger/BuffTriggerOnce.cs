using System.Collections.Generic;

namespace ASeKi.battle 
{ 
    public abstract class BuffTriggerOnce : BuffTrigger
    {
        public override void Start()
        {
            trigger();
        }

        public override void Release()
        {
            base.Release();
        }

        private void trigger()
        {
            if(!CheckTriggerCondition())
            {
                return;
            }

            buff.TriggerWithoutNet(0);
        }
    }
}
