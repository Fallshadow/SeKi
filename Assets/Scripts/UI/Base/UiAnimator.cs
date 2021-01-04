using System;
using UnityEngine;

namespace ASeKi.ui
{
    // 默认从动画的零层开始
    [RequireComponent(typeof(Animator))]
    public class UiAnimator : MonoBehaviour, IUiAnimation
    {
        [SerializeField] protected Animator animator = null;
        [SerializeField] protected string clipNamePrefix = null;

        protected void Reset()
        {
            animator = GetComponent<Animator>();
            animator.applyRootMotion = false;
            animator.updateMode = AnimatorUpdateMode.Normal;
            clipNamePrefix = name;
        }

        public void Initialize(Action<IUiAnimation> completeCb)
        {
            string showClipName = string.Format("{0}_{1}", clipNamePrefix, UiBase.ShowClipName);
            string hideClipName = string.Format("{0}_{1}", clipNamePrefix, UiBase.HideClipName);
            AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
            for(int i = 0; i < clips.Length; ++i)
            {
                if(clips[i].name.Contains(showClipName))
                {
                    setCompleteCb(clips[i]);
                    continue;
                }

                if(clips[i].name.Contains(hideClipName))
                {
                    setCompleteCb(clips[i]);
                    continue;
                }
            }
        }

        public bool HasClip(UiAnimationClipType clip)
        {
            switch(clip)
            {
                case UiAnimationClipType.UAC_SHOW:
                    return animator.HasState(0, Animator.StringToHash(UiBase.ShowClipName));
                case UiAnimationClipType.UAC_HIDE:
                    return animator.HasState(0, Animator.StringToHash(UiBase.HideClipName));
                default:
                    return false;
            }
        }

        public void Play(UiAnimationClipType clipType)
        {
            animator.enabled = true;
            switch(clipType)
            {
                case UiAnimationClipType.UAC_SHOW:
                    animator.Play(UiBase.ShowClipName, 0, 0f);
                    break;
                case UiAnimationClipType.UAC_HIDE:
                    animator.Play(UiBase.HideClipName, 0, 0f);
                    break;
                default:
                    debug.PrintSystem.LogError($"[UiAnimator] Unexpected clip: {clipType.ToString()}");
                    return;
            }
        }

        public void Stop()
        {
            animator.enabled = false;
        }

        // 在动画的最后添加事件
        protected void setCompleteCb(AnimationClip clip)
        {
            if(clip == null)
            {
                return;
            }

            AnimationEvent evt = new AnimationEvent
            {
                time = clip.length,
                objectReferenceParameter = this,
                functionName = UiBase.OnCompleteFunctionName
            };
            clip.AddEvent(evt);
        }
    }
}