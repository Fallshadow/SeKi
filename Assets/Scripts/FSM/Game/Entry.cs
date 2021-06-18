using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.fsm
{
    public class Entry : State<game.GameController>
    {
        public override void Enter()
        {
            initManagers();
            m_fsm.SwitchToState((int)GameFsmState.RESET);
        }

        private void initManagers()
        {
            // 在这里进行各个管理器的初始化
        }
    }
}