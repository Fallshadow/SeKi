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
    [DisallowMultipleComponent]
    public abstract class UiBase : MonoBehaviour
    {
        public const string IdleClipName = "Idle";
        public const string ShowClipName = "Show";                              // 这个是UI的动画名
        public const string HideClipName = "Hide";                              // 这个是UI的动画名
        public const string OnCompleteFunctionName = "onUiAnimationComplete";   // 动画最后调用的本类的方法名
        public virtual UiOpenType OpenType { get => UiOpenType.UOT_COMMON; }    // 具体类型

        public UiState State { get; protected set; }                            // 具体状态

        public bool IsAlwaysVisible { get { return isAlwaysVisible; } }         // 提供给那些永不隐藏的UI
        [SerializeField] protected bool isAlwaysVisible = false;

        public bool IsDontDestroy { get { return isDontDestroy; } }             // 提供给那些永不销毁的UI
        [SerializeField] protected bool isDontDestroy = false;

        protected Action<UiBase> onCloseCompleteHandler = null;                 // 结束展示时的调用
        protected Action onAnimationCompleteCallback = null;                    // 用于各种动画的回调函数
        protected IUiAnimation[] uiAnimations = null;                           // 动画列表
        protected HashSet<IUiAnimation> playingAnimations = null;               // 动画列表（HashSet无序不重复）

        public virtual void OnCreate()
        {
            //setAnimations();
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

        protected virtual void onOpen() { }

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
            State = UiState.Show;
            SetVisible(true);
            onShow();
            evt.EventManager.instance.Send(evt.EventGroup.UI, (short)evt.UiEvent.UI_BASE_SHOW, GetType().Name, transform);
            playAnimations(UiAnimationClipType.UAC_SHOW);
        }

        protected virtual void onShow() { }

        public virtual void SetVisible(bool isVisible)
        {
            if(isVisible)
            {
                gameObject.SetActive(true);
                return;
            }

            if(!IsAlwaysVisible)
            {
                gameObject.SetActive(false);
            }
        }

        public abstract void Initialize();
        public abstract void Release();
        public abstract void Refresh();

        // Reset函数一般用来确定默认值
        protected virtual void Reset()
        {
            gameObject.layer = UiManager.VisibleUiLayer;
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

        protected virtual void onTransitionComplete()
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

            if(onAnimationCompleteCallback != null)
            {
                onAnimationCompleteCallback();
                onAnimationCompleteCallback = null;
            }
        }

        protected virtual void onShowComplete()
        {
            //LastCloseTime = null;
        }

        protected virtual void onHideComplete() { }
    }
}