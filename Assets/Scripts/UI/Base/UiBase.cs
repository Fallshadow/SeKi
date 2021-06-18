using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.ui
{
    public enum UiState
    {
        None = 0,
        ShowingAnim = 1,
        Show = 2,
        HidingAnim = 3,
        Hide = 4
    }

    // 这是UI界面的基础类
    // 详解：创建之初设置未为Hide状态
    // 注意默认的UI无法交互，InteractableUi才是可交互UI
    // 不建议直接继承uibase，建议挑选或者增加子类再继承
    [DisallowMultipleComponent]
    public abstract class UiBase : MonoBehaviour
    {
        public const string IdleClipName = "Idle";
        public const string ShowClipName = "Show";                              // 这个是UI的动画名
        public const string HideClipName = "Hide";                              // 这个是UI的动画名
        public const string OnCompleteFunctionName = "onUiAnimationComplete";   // 动画最后调用的本类的方法名

        public virtual UiOpenType OpenType { get => UiOpenType.UOT_COMMON; }    // 具体类型

        public UiState State { get; protected set; }                            // 具体状态

        public virtual bool IsAlwaysVisible => false;                           // 提供给那些永不隐藏的UI

        public virtual bool bDynamicUnload => true;                             // 提供给那些参与动态加载的UI

        public bool IsDontDestroy => isDontDestroy;                             // 提供给那些永不销毁的UI
        [SerializeField] protected bool isDontDestroy = false;
        
        public bool IsOpen => onCloseCompleteHandler != null;                   // 逻辑上是否已经打开
        

        [NonSerialized] public float? LastCloseTime = null;                     // 窗口最后关闭的unity时间

        protected Action<UiBase> onCloseCompleteHandler = null;                 // 结束展示时的调用，如果UI创建出来了那一定是有的，UImanager会赋值
        protected Action onAnimationCompleteCallback = null;                    // 用于各种动画的回调函数
        protected IUiAnimation[] uiAnimations = null;                           // 动画列表
        protected HashSet<IUiAnimation> playingAnimations = null;               // 动画列表（HashSet无序不重复）

        public virtual void OnCreate()
        {
            setAnimations();
            State = UiState.Hide;
            SetVisible(false);
            Initialize();
            debug.PrintSystem.Log($"{gameObject.name} UiBase OnCreate 设置动画、设置状态Hide、设置不可见、初始化");
        }

        public void Open(Action<UiBase> closeCompleteCb, Action openCompleteCb = null)
        {
            if(onCloseCompleteHandler != null)
            {
                debug.PrintSystem.LogWarning($"[UiBase] Open 该UI已经打开了 Name: {gameObject.name}");
                openCompleteCb?.Invoke();
                return;
            }

            onCloseCompleteHandler = closeCompleteCb;
            onOpen();
            Show(openCompleteCb);
        }
        protected abstract void onOpen();

        public virtual void Show(Action showCompleteCb = null)
        {
            if(State == UiState.Show || State == UiState.ShowingAnim)
            {
                debug.PrintSystem.LogWarning($"[UiBase] Show 该UI处于错误状态(展示中/正在展示) Name: {gameObject.name}, State: {State}");
                showCompleteCb?.Invoke();
                return;
            }

            if(State == UiState.HidingAnim)
            {
                debug.PrintSystem.LogWarning($"[UiBase] Show 该UI处于正在展示关闭的状态，需要终止Hide展示并清除动画 Name: {gameObject.name}, State: {State}");
                onAnimationCompleteCallback = null;
                playingAnimations.Clear();
            }

            onAnimationCompleteCallback += showCompleteCb;
            SetVisible(true);
            onShow();
            evt.EventManager.instance.Send(evt.EventGroup.UI, (short)evt.UiEvent.UI_BASE_SHOW, GetType().Name, transform);
            playAnimations(UiAnimationClipType.UAC_SHOW);
        }

        protected abstract void onShow();

        public void Close(Action completeCb = null)
        {
            if(onCloseCompleteHandler == null)
            {
                debug.PrintSystem.LogWarning($"[UserInterfaceBase] UI has already closed. Name: {gameObject.name}");
                completeCb?.Invoke();
                return;
            }

            onClose();
            Hide(onCloseComplete + completeCb);
        }

        protected abstract void onClose();

        public void Hide(Action hideCompleteCb = null)
        {
            if(State == UiState.Hide || State == UiState.HidingAnim)
            {
                debug.PrintSystem.LogWarning($"[UserInterfaceBase] Hide at wrong state. Name: {gameObject.name}, State: {State}");
                hideCompleteCb?.Invoke();
                return;
            }

            if(State == UiState.ShowingAnim)
            {
                onAnimationCompleteCallback = null;
                playingAnimations.Clear();
            }

            onAnimationCompleteCallback += hideCompleteCb;
            onHide();
            evt.EventManager.instance.Send<string, Transform>(evt.EventGroup.UI, (short)evt.UiEvent.UI_BASE_HIDE, GetType().Name, transform);
            playAnimations(UiAnimationClipType.UAC_HIDE);
        }

        protected abstract void onHide();

        private void onCloseComplete()
        {
            LastCloseTime = UnityEngine.Time.time;
            onCloseCompleteHandler?.Invoke(this);
            onCloseCompleteHandler = null;
        }

        // 销毁UI 
        public void OnRuin()
        {
            if(State == UiState.None)
            {
                return;
            }

            if(onCloseCompleteHandler != null)
            {
                onClose();
                onCloseCompleteHandler = null;
            }

            if(State == UiState.Show || State == UiState.ShowingAnim)
            {
                onHide();
                onHideComplete();
            }
            Release();
        }

        #region UI动画
        protected void setAnimations()
        {
            uiAnimations = GetComponentsInChildren<IUiAnimation>();
            playingAnimations = new HashSet<IUiAnimation>();
            for(int i = 0; i < uiAnimations.Length; ++i)
            {
                uiAnimations[i].Initialize(onAnimationComplete);
            }
        }
        protected void onAnimationComplete(IUiAnimation uiAnimation)
        {
            playingAnimations.Remove(uiAnimation);
            if(playingAnimations.Count != 0)
            {
                return;
            }

            onTransitionComplete();
        }
        protected void playAnimations(UiAnimationClipType animationClip)
        {
            switch(animationClip)
            {
                case UiAnimationClipType.UAC_SHOW:
                    {
                        State = UiState.ShowingAnim;
                        break;
                    }
                case UiAnimationClipType.UAC_HIDE:
                    {
                        State = UiState.HidingAnim;
                        break;
                    }
                default:
                    {
                        debug.PrintSystem.LogError("[UiBase] 播的动画不正确");
                        return;
                    }
            }

            for(int i = 0; i < uiAnimations.Length; ++i)
            {
                if(uiAnimations[i].HasClip(animationClip))
                {
                    playingAnimations.Add(uiAnimations[i]);
                }
            }

            if(playingAnimations.Count == 0)
            {
                onTransitionComplete();
                return;
            }

            // Play clips.
            foreach(IUiAnimation clip in playingAnimations)
            {
                clip.Play(animationClip);
            }
        }

        protected void onTransitionComplete()
        {
            checkState();

            if(onAnimationCompleteCallback != null)
            {
                onAnimationCompleteCallback();
                onAnimationCompleteCallback = null;
            }
        }
        private void checkState()
        {
            switch(State)
            {
                case UiState.ShowingAnim:
                    {
                        State = UiState.Show;
                        onShowComplete();
                        break;
                    }
                case UiState.HidingAnim:
                    {
                        State = UiState.Hide;
                        SetVisible(false);
                        onHideComplete();
                        break;
                    }
            }
        }
        protected virtual void onShowComplete()
        {
            LastCloseTime = null;
        }

        protected virtual void onHideComplete() { }

        #endregion

        #region 立即展示、隐藏 跳过UI动画

        public virtual void ShowImmediate()
        {
            if(State == UiState.Show || State == UiState.ShowingAnim)
            {
                debug.PrintSystem.LogWarning($"[UserInterfaceBase] Show at wrong state. Name: {gameObject.name}, State: {State}");
                return;
            }

            if(State == UiState.HidingAnim)
            {
                StopAnimations();
            }

            onShow();
            State = UiState.ShowingAnim;
            SetVisible(true);
            evt.EventManager.instance.Send<string, Transform>(evt.EventGroup.UI, (short)evt.UiEvent.UI_BASE_SHOW, GetType().Name, transform);
            checkState();
        }

        public virtual void HideImmediate()
        {
            if(State == UiState.Hide || State == UiState.HidingAnim)
            {
                debug.PrintSystem.LogWarning($"[UserInterfaceBase] Hide at wrong state. Name: {gameObject.name}, State: {State}");
                return;
            }

            if(State == UiState.ShowingAnim)
            {
                StopAnimations();
            }

            onHide();
            State = UiState.HidingAnim;
            SetVisible(false);
            evt.EventManager.instance.Send<string, Transform>(evt.EventGroup.UI, (short)evt.UiEvent.UI_BASE_HIDE, GetType().Name, transform);
            checkState();
        }

        public void StopAnimations()
        {
            onAnimationCompleteCallback = null;
            foreach(IUiAnimation clip in playingAnimations)
            {
                clip.Stop();
            }
            playingAnimations.Clear();
        }
        #endregion

        public void SetVisible(bool isVisible)
        {
            if(isVisible)
            {
                if(OpenType == UiOpenType.UOT_PERSPECTIVE)
                {
                    gameObject.layer = UiManager.perspectiveLayer;
                }
                else
                {
                    gameObject.layer = UiManager.VisibleUiLayer;
                }
                return;
            }

            if(!IsAlwaysVisible)
            {
                gameObject.layer = UiManager.InvisibleUiLayer;
            }
        }

        // UI创建时初始化物件
        public abstract void Initialize();

        // UI销毁时释放资源
        public abstract void Release();

        // 统一一个刷新UI的接口
        public abstract void Refresh();

        // Reset函数一般用来确定默认值
        // 当用户在检查器的上下文菜单中点击Reset按钮或者第一次添加组件时，Reset被调用。此函数仅在编辑器模式下调用。Reset最常用来在检查器中给出良好的默认值。
        protected virtual void Reset()
        {
            gameObject.layer = UiManager.VisibleUiLayer;
        }
    }
}