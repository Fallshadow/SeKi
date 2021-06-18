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
            initState();
            m_fsm.SwitchToState((int)GameFsmState.DEBUG_ENTRY);
            SetManagersActive(true);
        }

        private void initState()
        {
            m_fsm.Initialize(this);
            m_fsm.AddState((int)GameFsmState.ENTRY, new Entry());
            m_fsm.AddState((int)GameFsmState.RESET, new Reset());
#if UNITY_EDITOR
            m_fsm.AddState((int)GameFsmState.DEBUG_ENTRY, new DebugEntry());
#endif
            
        }

        // NOTE: 初始化完成前先停止GameControll上所有MonoBehaviour的Update。
        // TODO: 所有全域的Update應該都要統一放在GameControll裡。
        private void SetManagersActive(bool isActive)
        {
            MonoBehaviour[] managers = GetComponents<MonoBehaviour>();
            for(int i = 0; i < managers.Length; ++i)
            {
                managers[i].enabled = isActive;
            }
        }
        
        private void FixedUpdate()
        {
            m_fsm.FixedUpdate();
        }

        private void Update()
        {
            m_fsm.Update();
        }
        
        private void LateUpdate()
        {
            m_fsm.LateUpdate();
        }

        private void OnDestroy()
        {
            m_fsm.Finalize();
        }
    }
}
