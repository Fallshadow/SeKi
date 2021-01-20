using System;
using System.Collections.Generic;

namespace ASeKi.battle
{
    public abstract class BuffTriggerEvent : BuffTrigger
    {
        public override void Start()
        {
            registerEvent();
        }

        public override void Release()
        {
            base.Release();
            unRegisterEvent();
        }

        protected abstract bool CheckTriggerEvent(ulong param1, ulong param2, int param3, int param4, int param5);

        protected abstract evt.EventGroup EventGroupId();

        protected abstract short EventId();

        private void registerEvent()
        {
            evt.EventManager.instance.Register<ulong, ulong, int, int, int>(EventGroupId(), (short)EventId(), receiveEvent);
        }

        private void unRegisterEvent()
        {
            evt.EventManager.instance.Unregister<ulong, ulong, int, int, int>(EventGroupId(), (short)EventId(), receiveEvent);
        }

        private void receiveEvent(ulong param1, ulong param2, int param3, int param4, int param5)
        {
            if(buff.CarrierID != param1)
            {
                return;
            }

            if(!CheckTriggerEvent(param1, param2, param3, param4, param5))
            {
                return;
            }

            if(!CheckTriggerCondition())
            {
                return;
            }

            buff.TriggerWithNet(param2);
        }
    }
}