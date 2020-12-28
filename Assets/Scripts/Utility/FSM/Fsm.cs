using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.fsm
{
    // 某个系统的状态机，是这个系统与外界联系的接口
    public class Fsm<T>
    {
        public T Owner { get; protected set; }      // 设置自身，主要方便state使用自身参数

        private Machine<T> m_machine;
        private Dictionary<int, State<T>> m_stateMap = new Dictionary<int, State<T>>(); // 存储着所有的状态

        public void Initialize(T owner)
        {
            Owner = owner;
            m_machine = new Machine<T>();
        }

        public void Finalize()
        {
            m_stateMap.Clear();
        }

        // 更新状态（而且深度监视了性能）
        public void Update()
        {
            UnityEngine.Profiling.Profiler.BeginSample("[ScriptProfiler]FSM.Update");
            State<T> s = m_machine.GetCurrentState();
            if(s != null)
            {
                s.Update();
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void FixedUpdate()
        {
            State<T> s = m_machine.GetCurrentState();
            if(s != null)
            {
                s.FixedUpdate();
            }
        }

        public void LateUpdate()
        {
            State<T> s = m_machine.GetCurrentState();
            if(s != null)
            {
                s.LateUpdate();
            }
        }

        public State<T> GetCurrentState()
        {
            if(m_machine == null)
            {
                return null;
            }
            return m_machine.GetCurrentState();
        }

        public int GetCurrentStateEnum()
        {
            return GetStateEnum(GetCurrentState());
        }

        public int GetStateEnum(State<T> s)
        {
            if(m_stateMap.ContainsValue(s))
            {
                Dictionary<int, State<T>>.Enumerator iter = m_stateMap.GetEnumerator();
                while(iter.MoveNext())
                {
                    if(iter.Current.Value == s)
                    {
                        return iter.Current.Key;
                    }
                }
            }

            return s.StateEnum();
        }

        public void AddState(int stateEnum, State<T> state)
        {
            state.Init(this);
            if(!m_stateMap.ContainsKey(stateEnum))
            {
                m_stateMap[stateEnum] = state;
            }
        }

        public State<T> GetState(int stateEnum)
        {
            m_stateMap.TryGetValue(stateEnum, out State<T> s);
            return s;
        }

        // 如果强行进行转换，即使当前的状态已经是了还是会执行转换
        public void SwitchToState(int stateEnum, bool isForce = false)
        {
            State<T> s = GetState(stateEnum);
            debug.PrintSystem.Assert(s != null);
            if(isForce || s != m_machine.GetCurrentState())
            {
                m_machine.SwitchToState(s);
            }
        }

        public void SwitchToState(State<T> s)
        {
            s.Init(this);
            if(s != m_machine.GetCurrentState())
            {
                m_machine.SwitchToState(s);
            }
        }

        public State<T> GetPreviousState()
        {
            return m_machine.GetPreviousState();
        }

        public Dictionary<int, State<T>>.Enumerator GetStateEnumerator()
        {
            return m_stateMap.GetEnumerator();
        }
    }
}