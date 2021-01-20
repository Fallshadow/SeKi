using UnityEngine;
using System.Collections.Generic;

namespace ASeKi.battle
{
    public abstract class BuffTriggerTimer : BuffTrigger
    {
        private time.Timer timerCycle;
        public override void FakeToReal()
        {
            //等待下次时间到才执行
            timerCycle = time.TimeManager.instance.AddCycleTimer(true, "Buff_Cycle_" + buff.Id, 1.0f * buff.Config.event_config1_param1 / 1000.0f, trigger);
        }
        public override void Start()
        {
            trigger();
            timerCycle = time.TimeManager.instance.AddCycleTimer(true, "Buff_Cycle_" + buff.Id, 1.0f * buff.Config.event_config1_param1 / 1000.0f, trigger);
        }

        public override void Release()
        {
            base.Release();
            if(timerCycle != null)
            {
                time.TimeManager.instance.RemoveTimer(timerCycle);
            }
        }

        private void trigger(float cycle = 0, object obj = null)
        {
            if(!CheckTriggerCondition())
            {
                return;
            }

            buff.TriggerWithNet(0, true, this.sendAttachRequest);
        }
    }
}
