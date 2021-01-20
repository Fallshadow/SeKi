using System.Collections.Generic;

namespace ASeKi.battle
{
    // 执行到具体实体上的BUFF执行者
    public abstract class BuffExecuter
    {
        public int Index;

        private Utility.ObjectPool<object> pool = null;
        protected ASeKi.time.Timer timerDelay = null;

        protected virtual bool ignoreFake
        {
            get
            {
                return false;
            }
        }

        public void SetPool(Utility.ObjectPool<object> pool)
        {
            this.pool = pool;
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
            if(timerDelay != null)
            {
                ASeKi.time.TimeManager.instance.RemoveTimer(timerDelay);
                timerDelay = null;
            }
            pool.Release(this);
            pool = null;
        }

        protected virtual void releaseWithoutNet()
        {

        }

        protected virtual void releaseWithNet()
        {

        }

        public virtual void Addtive()
        {

        }

        public virtual void Resume(int layer)
        {

        }

        public void OnRecvBuffUseEvent(int type, int value, IEnumerable<uint> ids)
        {
            if(BuffSourceUtility.CheckNativeLogic(buff.CarrierID))
            {
                onRecvNative(type, value, ids);
            }
            else
            {
                onRecvRemote(type, value, ids);
            }
        }

        //收到本机发送的消息
        protected virtual void onRecvNative(int type, int value, IEnumerable<uint> ids)
        {

        }
        //收到其他端发送的消息
        protected virtual void onRecvRemote(int type, int value, IEnumerable<uint> ids)
        {

        }


        protected Buff buff = null;
        protected ulong targetId = 0;
        protected List<ulong> units = new List<ulong>();
        //protected act.time.Timer timerDelay = null;

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

    }
}
