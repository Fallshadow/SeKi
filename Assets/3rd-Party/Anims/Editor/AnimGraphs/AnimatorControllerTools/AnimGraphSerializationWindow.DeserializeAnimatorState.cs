using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

namespace Framework.AnimGraphs
{

    public partial class AnimGraphSerializationWindow
    {
        private void DeserializeAnimatorState(AnimatorState animatorState,int layer)
        {
            if (!animatorStateHash.Add(animatorState.name))
            {
                Debug.LogError($"重复状态名: {animatorState.name}");
                return;
            }

            AnimGraphStateScriptableObject animGraphStateScriptable = ScriptableObject.CreateInstance<AnimGraphStateScriptableObject>();
            animGraphStateScriptable.layer = layer;
            animGraphStateScriptable.stateName = animatorState.name;
            animGraphStateScriptable.iKOnFeet = animatorState.iKOnFeet;
            animGraphStateScriptable.writeDefaultValues = animatorState.writeDefaultValues;
            animGraphStateScriptable.transitions = new List<AnimGraphTransitionScriptableObject>();

            animGraphStateScriptable.speed = animatorState.speed;
            int hash = animatorState.name.HashCode();
            // AnyTransition
            animGraphStateScriptable.anyTransition = new AnimGraphTransitionScriptableObject();
            if (anyTransitionMapping.ContainsKey(hash))
            {
                var any = anyTransitionMapping[hash];
                animGraphStateScriptable.anyTransition.duration = any.duration;
                animGraphStateScriptable.anyTransition.destinationState = "Any";
                animGraphStateScriptable.anyTransition.exitTime = 0;
                animGraphStateScriptable.anyTransition.hasExitTime = false;
                animGraphStateScriptable.anyTransition.offset = 0;
            }
            else
            {
                animGraphStateScriptable.anyTransition.duration = 0;
                animGraphStateScriptable.anyTransition.destinationState = "Any";
                animGraphStateScriptable.anyTransition.exitTime = 0;
                animGraphStateScriptable.anyTransition.hasExitTime = false;
                animGraphStateScriptable.anyTransition.offset = 0;
            }

            if (animatorState.motion == null)
            {
                Debug.LogError($"AnimatorState {animatorState.name} Motion序列化丢失!");
            }

            DeserializeMotion(animatorState.name,animatorState.motion, animGraphStateScriptable);

            animGraphStateScriptable.fadeInLayerTransition = new AnimGraphLayerTransitionScriptableObject();
            //animGraphStateScriptable.fadeInLayerTransition.duration = 0;
            animGraphStateScriptable.fadeInLayerTransition.layerTransitionType = LayerTransitionType.FadeIn;

            animGraphStateScriptable.fadeOutLayerTransition = new AnimGraphLayerTransitionScriptableObject();
            //animGraphStateScriptable.fadeOutLayerTransition.duration = 0;
            animGraphStateScriptable.fadeOutLayerTransition.layerTransitionType = LayerTransitionType.FadeOut;

            if (layerFadeinCacheMap.ContainsKey(hash))
            {
                animGraphStateScriptable.fadeInLayerTransition.duration = layerFadeinCacheMap[hash];
            }
            else
            {
                animGraphStateScriptable.fadeInLayerTransition.duration = 0;
            }


            if (animatorState.transitions.Length > 0)
            {
                for (int i = 0; i < animatorState.transitions.Length; i++)
                {
                    var transition = animatorState.transitions[i];
                    if (!transition.hasFixedDuration)
                    {
                        Debug.LogError($"使用了非FixedDuration模式 animatorState {animatorState.name} transition {transition.name}");
                    }
                    if (transition.interruptionSource != TransitionInterruptionSource.None)
                    {
                        Debug.LogError($"使用中断设置 interruptionSource {transition.interruptionSource} transition {transition.name}");
                    }
                    DeserializeAnimatorStateTransition(transition, animGraphStateScriptable);
                }
            }
            string writeFilePath = Path.Combine(AssetDatabase.GetAssetPath(root), "Controller");
            var path = Path.Combine(writeFilePath, "State");
            Directory.CreateDirectory(path);
            AssetDatabase.CreateAsset(animGraphStateScriptable, Path.Combine(path, animatorState.name + ".asset"));
            EditorUtility.SetDirty(animGraphStateScriptable);

            AnimGraphNodeScriptableObject nodeScriptable = ScriptableObject.CreateInstance<AnimGraphNodeScriptableObject>();
            nodeScriptable.nodeName = animatorState.name;
            nodeScriptable.stateScriptable = animGraphStateScriptable;

            var nodePath = Path.Combine(writeFilePath);
            Directory.CreateDirectory(nodePath);
            AssetDatabase.CreateAsset(nodeScriptable, Path.Combine(nodePath, nodeScriptable.nodeName + ".asset"));
            EditorUtility.SetDirty(nodeScriptable);

        }
    }
}