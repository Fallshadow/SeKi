using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;


namespace Framework.AnimGraphs
{

    public partial class AnimGraphSerializationWindow
    {
        private void DeserializeChildMotion(string blendTreeName,ChildMotion childMotion, AnimGraphBlendTreeScriptableObject blendTreeScriptable, BlendTreeType blendType, int deep)
        {
            AnimGraphChildMotionScriptableObject childMotionScriptable = new AnimGraphChildMotionScriptableObject();/*ScriptableObject.CreateInstance<AnimGraphChildMotionScriptableObject>();*/

            switch (blendType)
            {
                case BlendTreeType.Simple1D:
                    childMotionScriptable.position = new Vector2(childMotion.threshold, 0);
                    break;
                case BlendTreeType.FreeformCartesian2D:
                    childMotionScriptable.position = new Vector2(childMotion.position.x, childMotion.position.y);
                    break;
                default:
                    throw new System.Exception($"未实现的分类 {blendType}");
            }

            childMotionScriptable.timeScale = childMotion.timeScale;

            if (childMotion.motion == null)
            {
                Debug.LogError($"BlendTree {blendTreeName} ChildMotion序列化丢失!");
            }

            if (childMotion.motion is BlendTree)
            {
                deep++;
                Debug.LogError($"BlendTree {blendTreeName} ChildMotion 嵌套BlendTree模式!");
                childMotionScriptable.motion = DeserializeBlendTree(blendTreeName,childMotion.motion as BlendTree, deep);
            }
            if (childMotion.motion is AnimationClip)
            {
                childMotionScriptable.motion = DeserializeAnimationClip(childMotion.motion as AnimationClip);
            }
            blendTreeScriptable.childMotions.Add(childMotionScriptable);
        }
    }
}
