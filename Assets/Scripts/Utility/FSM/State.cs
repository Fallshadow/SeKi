using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.fsm
{
    // 以后有需要用到状态机的在这边添加枚举
    public enum GameFsmState
    {
        ENTRY = 0,
        RESET,
        LOAD_SCENE,
        MAIN_TOWN,
        DEBUG_ENTRY
    }

    // 状态本身需要处理逻辑的几个过程：
    // 进入、退出、同步
    public class State<T>
    {
        // 状态应该知道自己是属于哪个状态机
        protected Fsm<T> m_fsm;

        // 初始化的时候配置状态机
        public void Init(Fsm<T> f)
        {
            m_fsm = f;
            onInit();
        }

        protected virtual void onInit() { }

        public virtual void Enter() { }
        public virtual void Exit() { }

        public virtual void FixedUpdate() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }

        public virtual int StateEnum()
        {
            return -1;
        }
    }
}