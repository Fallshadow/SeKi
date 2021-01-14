using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASeKi.battle
{
    // 执行到具体实体上的BUFF执行者
    public abstract class BuffExecuter
    {
        protected Buff buff = null;
        protected ulong targetId = 0;
        protected List<ulong> units = new List<ulong>();
        private Utility.ObjectPool<object> pool = null;
        //protected act.time.Timer timerDelay = null;

        protected virtual bool ignoreFake
        {
            get
            {
                return false;
            }
        }

        public void Release()
        {
            if(buff.IsHostLogic || ignoreFake)
            {
                releaseWithNet();
            }
            else
            {
                releaseWithoutNet();
            }
            //if(timerDelay != null)
            //{
            //    act.time.TimeManager.instance.RemoveTimer(timerDelay);
            //    timerDelay = null;
            //}
            pool.Release(this);
            pool = null;
        }

        protected void delayDo(object obj = null)
        {
            if(!buff.IsHostLogic && !ignoreFake)
            {
                return;
            }
            executeInternal();
        }

        protected virtual void executeInternal()
        {

        }

        public abstract void Execute();

        protected virtual void releaseWithoutNet()
        {

        }
        protected virtual void releaseWithNet()
        {

        }

    }
}
