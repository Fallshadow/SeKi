#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using ASeKi.ui;
using UnityEngine;

namespace ASeKi.fsm
{
    public class DebugEntry : State<game.GameController>
    {
        public override void Enter()
        {
            initManagers();
            UiManager.instance.OpenUi<DebugMainCanvas>();
        }

        private void initManagers()
        {
            // 在这里进行各个管理器的初始化
        }
    }
}
#endif