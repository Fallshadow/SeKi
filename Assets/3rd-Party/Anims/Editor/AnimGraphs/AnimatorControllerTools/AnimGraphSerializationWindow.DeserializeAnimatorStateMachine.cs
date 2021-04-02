using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;


namespace Framework.AnimGraphs
{

    public partial class AnimGraphSerializationWindow
    {
        private Dictionary<int, AnimatorStateTransition> anyTransitionMapping = new Dictionary<int, AnimatorStateTransition>(1024);

        private Dictionary<int, float> layerFadeinCacheMap = new Dictionary<int, float>();

        private void DeserializeAnimatorStateMachine(AnimatorStateMachine stateMachine,int layer)
        {
            //stateMachine.anyStateTransitions;
            Debug.Log($"AnimatorStateMachine Name：{stateMachine.name}");
            if (stateMachine.stateMachines.Length > 0)
            {
                string childAnimatorStateMachineName = "";

                for (int i = 0; i < stateMachine.stateMachines.Length; i++)
                {
                    childAnimatorStateMachineName += stateMachine.stateMachines[i].stateMachine.name + " ";
                }

                Debug.Log($"ChildAnimatorStateMachines Name：{childAnimatorStateMachineName}");
            }

            if(stateMachine.anyStateTransitions != null)
            {
                for (int i = 0; i < stateMachine.anyStateTransitions.Length; i++)
                {
                    var any = stateMachine.anyStateTransitions[i];
                    if (anyTransitionMapping.ContainsKey(any.destinationState.name.HashCode()))
                    {
                        Debug.LogError($"不应该出现重复的AnyTransition配置 {any.destinationState.name}");
                        continue;
                    }
                    anyTransitionMapping.Add(any.destinationState.name.HashCode(), any);
                }
            }

            for (int i = 0; i < stateMachine.stateMachines.Length; i++)
            {
                DeserializeChildAnimatorStateMachine(stateMachine.stateMachines[i],layer);
            }

            if (stateMachine.states.Length > 0)
            {
                string statesName = "";
                for (int i = 0; i < stateMachine.states.Length; i++)
                {
                    var state = stateMachine.states[i].state;
                    statesName += state.name + " ";
                }
                Debug.Log($"ChildAnimatorStates Name：{statesName}");

                var defaultState = stateMachine.defaultState;
                if (defaultState.motion == null)
                {
                    cacheEmptyDefaultMotionBlend(defaultState);
                }

                for (int i = 0; i < stateMachine.states.Length; i++)
                {
                    var state = stateMachine.states[i].state;
                    DeserializeAnimatorState(state,layer);
                }
            }
        }

        private void cacheEmptyDefaultMotionBlend(AnimatorState animatorState)
        {
            if (animatorState.motion != null)
            {
                return;
            }

            int transitionCount = animatorState.transitions.Length;

            if (transitionCount == 0)
            {
                return;
            }

            for (int i = 0; i < transitionCount; i++)
            {
                var transition = animatorState.transitions[i];

                Debug.LogError($"自身为空motion {animatorState.name}  默认认为是LayerTransition FadeIn Duration:{ transition.duration} 目标是 { transition.destinationState.name} ");
                if (!layerFadeinCacheMap.ContainsKey(transition.destinationState.name.HashCode()))
                {
                    layerFadeinCacheMap.Add(transition.destinationState.name.HashCode(), transition.duration);
                }
            }
        }

        private void DeserializeChildAnimatorStateMachine(ChildAnimatorStateMachine stateMachine, int layer)
        {
            DeserializeAnimatorStateMachine(stateMachine.stateMachine,layer);
        }
    }
}