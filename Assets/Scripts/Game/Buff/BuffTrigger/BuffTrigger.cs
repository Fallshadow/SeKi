using System.Collections.Generic;

namespace ASeKi.battle
{
    public abstract class BuffTrigger
    {
        protected Buff buff; 
        Utility.ObjectPool<object> pool = null;
        HashSet<string> stateFlags = new HashSet<string>();
        protected bool sendAttachRequest;

        public abstract void Start();

        public virtual void FakeToReal()
        {
            Start();
        }

        public void Init(Buff buff, Utility.ObjectPool<object> pool)
        {
            this.buff = buff;
            this.pool = pool;
        }

        public virtual void Release()
        {
            if(pool != null)
            {
                pool.Release(this);
                pool = null;
            }
            else
            {
                ASeKi.debug.PrintSystem.LogWarning("[buff] BuffTrigger Release twice!");
            }
        }

        protected bool CheckTriggerCondition()
        {
            for(int index = 0; index < buff.Config.carrier_conditions.Length; index++)
            {
                // 条件不满足
                if(!BuffCondition.Valify(buff.CarrierID, buff.Config.carrier_conditions[index]))
                {
                    return false;
                }
            }

            // state Flag检测不通过
            if(!checkStateFlags())
            {
                return false;
            }
            return true;
        }

        private bool checkStateFlags()
        {
            //处于某种状态才可以触发
            stateFlags.Clear();
            ASeKi.action.ActionManager.instance.GetCurrentStateFlags(buff.CarrierID, stateFlags);

            //判定carrier是否满足stateFlag
            bool containContent = false;
            bool emptyContent = true;
            for(int i = 0; i < buff.Config.state_flags.Length; i++)
            {
                if(!string.IsNullOrEmpty(buff.Config.state_flags[i]) && buff.Config.state_flags[i] != "0")
                {
                    if(stateFlags.Contains(buff.Config.state_flags[i]))
                    {
                        containContent = true;
                        break;
                    }
                    emptyContent = false;
                }
            }
            if(!containContent && !emptyContent)
            {
                return false;
            }
            return true;
        }
    }
}
