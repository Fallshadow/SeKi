using System.Collections.Generic;

namespace ASeKi.battle 
{ 
    public abstract class BuffTriggerOnce : BuffTrigger
    {
        public override void FakeToReal()
        {
            //什么也不用做
        }
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
            if(!this.CheckTriggerCondition())
            {
                return;
            }

            buff.TriggerWithNet(0);
        }
    }
}
