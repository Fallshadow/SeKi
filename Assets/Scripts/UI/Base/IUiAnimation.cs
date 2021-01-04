using System;

namespace ASeKi.ui
{
    public enum UiAnimationClipType
    {
        UAC_CUSTOM = 0,
        UAC_SHOW = 1,
        UAC_HIDE = ~UAC_SHOW,
        UAC_IDLE = 2
    }

    public interface IUiAnimation
    {
        void Initialize(Action<IUiAnimation> completeCb);
        bool HasClip(UiAnimationClipType clipType);
        void Play(UiAnimationClipType clipType);
        void Stop();
    }
}    