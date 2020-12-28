using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.fsm
{
    // 为状态机提供状态转换的工具
    // 接口：获取当前状态、获取前一个状态、转换到X状态、转换到前一个状态
    public class Machine<T>
    {
        private State<T> m_currentState = null;
        private State<T> m_previousState = null;

        public State<T> GetCurrentState()
        {
            return m_currentState;
        }

        public State<T> GetPreviousState()
        {
            return m_previousState;
        }

        public void SwitchToState(State<T> newState)
        {
            if(m_currentState != null)
            {
                m_previousState = m_currentState;
                m_currentState.Exit();
            }

            if(newState != null)
            {
                m_currentState = newState;
                m_currentState.Enter();
            }
        }

        public void SwitchToPreviousState()
        {
            if(m_previousState != null)
            {
                SwitchToState(m_previousState);
            }
        }
    }
}
