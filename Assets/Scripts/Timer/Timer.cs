using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.time
{
    [System.Serializable]
    public class Timer : MonoBehaviour
    {
        public string Name = "";
        public bool pause = false;
        public object obj = null;               // 帶參數
        public float ElapsedTime = 0f;
        public float LocalTimeScale = 1f;

        public bool IsUnique { get; private set; }

        public Timer(string name, bool isUnique, object obj = null)
        {
            Name = name;
            this.obj = obj;
            IsUnique = isUnique;
        }

        public virtual void Pause()
        {
            pause = true;
        }

        public virtual void Play()
        {
            Reset();
            pause = false;
        }

        public virtual void Reset()
        {
            ElapsedTime = 0;
        }

        public virtual void Process(float deltaTime)
        {
            if(pause)
                return;
            ElapsedTime += getTotalTimeScaleDeltaTime(deltaTime);
        }

        protected float getTotalTimeScaleDeltaTime(float deltaTime)
        {
            float timeScale = deltaTime * LocalTimeScale;
            return timeScale;
        }
    }
}