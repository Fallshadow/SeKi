using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;


namespace Framework.AnimGraphs
{

    public partial class AnimGraphSerializationWindow
    {
        private void DeserializeMotion(string blendTreeName, UnityEngine.Motion motion, AnimGraphStateScriptableObject animGraphState)
        {
            if (motion == null)
            {
                return;
            }
            if (motion is BlendTree)
            {
                animGraphState.motion = DeserializeBlendTree(blendTreeName,motion as BlendTree);
                return;
            }
            if (motion is AnimationClip)
            {
                animGraphState.motion = DeserializeAnimationClip(motion as AnimationClip);
                return;
            }
        }
    }
}
