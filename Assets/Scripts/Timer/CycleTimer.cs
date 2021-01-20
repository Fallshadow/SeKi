using UnityEngine;

namespace ASeKi.time
{
    [System.Serializable]
    public class CycleTimer : Timer
    {
        public float Cycle = 1;
        public float CycleElasedTime = 0f;
        public delegate void OnCycleDelegate(float cycle, Timer obj = null);

        public OnCycleDelegate onCycleDelegate;

        public CycleTimer(string name, bool isUnique, float cycle, OnCycleDelegate onCycleDelegate, object obj = null) : base(name, isUnique, obj)
        {
            Cycle = cycle;
            this.onCycleDelegate = onCycleDelegate;
        }

        public override void Process(float deltaTime)
        {
            if(pause)
                return;

            base.Process(deltaTime);

            CycleElasedTime += getTotalTimeScaleDeltaTime(deltaTime);

            if(Mathf.Abs(CycleElasedTime) >= Cycle)
            {
                if(CycleElasedTime > 0)
                {
                    //for reverse time
                    onCycleDelegate?.Invoke(Cycle, this);
                    CycleElasedTime -= Cycle;
                }
                else
                {
                    onCycleDelegate?.Invoke(-Cycle, this);
                    CycleElasedTime += Cycle;
                }
            }
        }

        public override void Reset()
        {
            base.Reset();
            CycleElasedTime = 0;
        }
    }
}
