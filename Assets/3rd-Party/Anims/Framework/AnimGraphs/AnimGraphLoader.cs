using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.AnimGraphs
{
    public static class AnimGraphLoader
    {
        public delegate TResult DelegateFunc<T0,T1, TResult>(T0 ctx0,T1 ctx1);
        public delegate void DelegateAction<T0, T1>(T0 ctx0, T1 ctx1);

        public static DelegateFunc<int,int,AnimationClip> LoadAnimationClip;
        public static DelegateFunc<int,int, AvatarMask> LoadAvatarMask;
        public static DelegateAction<int, int> UnloadAnimationClip;
        public static DelegateAction<int, int> UnloadAvatarMask;
    }
}