using UnityEngine;
using System.Collections.Generic;

namespace ASeKi.battle
{
    public abstract class BuffTriggerUpdateTimer : BuffTrigger
    {
        private time.Timer timerCycle;

        public override void Start()
        {
            timerCycle = ASeKi.time.TimeManager.instance.AddUpdateTimer(true, "Buff_Update_" + buff.Id, trigger);
        }

        public override void Release()
        {
            base.Release();
            if(timerCycle != null)
            {
                ASeKi.time.TimeManager.instance.RemoveTimer(timerCycle);
            }
        }

        private void trigger(float cycle = 0, object obj = null)
        {
            bool satisfy = CheckTriggerCondition();
            buff.TriggerWithNet(0, satisfy);
        }
    }
}
