namespace ASeKi.time
{
    [System.Serializable]
    public class UpdateTimer : Timer
    {
        public delegate void OnUpdateDelegate(float delta, Timer obj = null);

        public OnUpdateDelegate onUpdateDelegate;

        public UpdateTimer(string name, bool isUnique, OnUpdateDelegate onUpdateDelegate, object obj = null) : base(name, isUnique, obj)
        {
            this.onUpdateDelegate += onUpdateDelegate;
        }

        public override void Process(float deltaTime)
        {
            if(pause)
                return;

            base.Process(deltaTime);

            onUpdateDelegate?.Invoke(getTotalTimeScaleDeltaTime(deltaTime), this);
        }
    }
}