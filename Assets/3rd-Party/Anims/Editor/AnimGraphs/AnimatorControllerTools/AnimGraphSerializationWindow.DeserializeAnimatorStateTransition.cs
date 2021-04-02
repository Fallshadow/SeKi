using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;


namespace Framework.AnimGraphs
{
    public partial class AnimGraphSerializationWindow
    {
        private HashSet<string> parameterSet = new HashSet<string>();
        private void DeserializeAnimatorStateTransition(AnimatorStateTransition transition, AnimGraphStateScriptableObject animGraphState)
        {
            AnimGraphTransitionScriptableObject transitionScriptable = new AnimGraphTransitionScriptableObject();/*ScriptableObject.CreateInstance<AnimGraphTransitionScriptableObject>();*/

            transitionScriptable.hasExitTime = transition.hasExitTime;
            transitionScriptable.exitTime = transition.exitTime;

            if(transition.destinationState == null)
            {
                animGraphState.fadeOutLayerTransition.duration = transition.duration;
                Debug.LogError($"目标过渡状态为空 {animGraphState.stateName}  默认认为是LayerTransition FadeOut Duration:{ transition.duration}");
                return;
            }

            transitionScriptable.destinationState = transition.destinationState.name;
            transitionScriptable.duration = transition.duration;
            transitionScriptable.offset = transition.offset;
            
            if (transition.conditions.Length > 0)
            {
                for (int i = 0; i < transition.conditions.Length; i++)
                {
                    var condition = transition.conditions[i];
                    DeserializeAnimatorCondition(condition);
                    parameterSet.Add(condition.parameter);
                    //Debug.Log($"destinationState {transitionScriptable.destinationState} parameter {condition.parameter} threshold {condition.threshold} mode {condition.mode}");
                }
            }

            animGraphState.transitions.Add(transitionScriptable);

            //transition.conditions
            //transition.destinationState
        }
    }
}