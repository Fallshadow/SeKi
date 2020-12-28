using System.Collections;
using ASeKi.fsm;
using UnityEngine;

namespace ASeKi.game
{
    public class GameController : SingletonMonoBehaviorNoDestroy<GameController>
    {
        private Fsm<GameController> m_fsm = new Fsm<GameController>();
        private IEnumerator Start()
        {
            SetManagersActive(false);
            yield return new WaitForEndOfFrame();   // TODO:这里应该读取本地表格数据
            SetManagersActive(true);
            initState();
            m_fsm.SwitchToState((int)GameFsmState.ENTRY);
        }

        private void initState()
        {
            m_fsm.Initialize(this);
            m_fsm.AddState((int)GameFsmState.ENTRY, new Entry());
        }

        // NOTE: 初始化完成前先停止GameControll上所有MonoBehaviour的Update。
        private void SetManagersActive(bool isActive)
        {
            MonoBehaviour[] managers = GetComponents<MonoBehaviour>();
            for(int i = 0; i < managers.Length; ++i)
            {
                managers[i].enabled = isActive;
            }
        }
    }
}
