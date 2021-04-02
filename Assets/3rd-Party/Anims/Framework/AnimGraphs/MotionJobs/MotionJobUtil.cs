using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public static class MotionJobUtil
    {
        public static void RemovePlayable(Playable playable, bool destroy = true)
        {
            if (!playable.IsValid())
                return;

            Debug.Assert(playable.GetInputCount() == 1,
                $"{nameof(RemovePlayable)} can only be used on playables with 1 input.");
            Debug.Assert(playable.GetOutputCount() == 1,
                $"{nameof(RemovePlayable)} can only be used on playables with 1 output.");

            var input = playable.GetInput(0);
            if (!input.IsValid())
            {
                if (destroy)
                    playable.Destroy();
                return;
            }

            var graph = playable.GetGraph();
            var output = playable.GetOutput(0);

            if (output.IsValid()) // Connected to another Playable.
            {
                if (destroy)
                {
                    playable.Destroy();
                }
                else
                {
                    Debug.Assert(output.GetInputCount() == 1,
                        $"{nameof(RemovePlayable)} can only be used on playables connected to a playable with 1 input.");
                    graph.Disconnect(output, 0);
                    graph.Disconnect(playable, 0);
                }

                graph.Connect(input, 0, output, 0);
            }
            else // Connected to the graph output.
            {
                Debug.Assert(graph.GetOutput(0).GetSourcePlayable().Equals(playable),
                    $"{nameof(RemovePlayable)} can only be used on playables connected to another playable or to the graph output.");

                if (destroy)
                    playable.Destroy();
                else
                    graph.Disconnect(playable, 0);

                graph.GetOutput(0).SetSourcePlayable(input);
            }
        }

        public static object GetParameterValue(Animator animator, AnimatorControllerParameter parameter)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    return animator.GetFloat(parameter.nameHash);

                case AnimatorControllerParameterType.Int:
                    return animator.GetInteger(parameter.nameHash);

                case AnimatorControllerParameterType.Bool:
                case AnimatorControllerParameterType.Trigger:
                    return animator.GetBool(parameter.nameHash);

                default:
                    throw new ArgumentException($"Unsupported {nameof(AnimatorControllerParameterType)}: {parameter.type}");
            }
        }

        public static object GetParameterValue(AnimatorControllerPlayable playable,
            AnimatorControllerParameter parameter)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    return playable.GetFloat(parameter.nameHash);

                case AnimatorControllerParameterType.Int:
                    return playable.GetInteger(parameter.nameHash);

                case AnimatorControllerParameterType.Bool:
                case AnimatorControllerParameterType.Trigger:
                    return playable.GetBool(parameter.nameHash);

                default:
                    throw new ArgumentException($"Unsupported {nameof(AnimatorControllerParameterType)}: {parameter.type}");
            }
        }

        public static void SetParameterValue(Animator animator, AnimatorControllerParameter parameter, object value)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(parameter.nameHash, (float) value);
                    break;

                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(parameter.nameHash, (int) value);
                    break;

                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(parameter.nameHash, (bool) value);
                    break;

                case AnimatorControllerParameterType.Trigger:
                    if ((bool) value)
                        animator.SetTrigger(parameter.nameHash);
                    else
                        animator.ResetTrigger(parameter.nameHash);
                    break;

                default:
                    throw new ArgumentException($"Unsupported {nameof(AnimatorControllerParameterType)}: {parameter.type}");
            }
        }

        public static void SetParameterValue(AnimatorControllerPlayable playable, AnimatorControllerParameter parameter,
            object value)
        {
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Float:
                    playable.SetFloat(parameter.nameHash, (float) value);
                    break;

                case AnimatorControllerParameterType.Int:
                    playable.SetInteger(parameter.nameHash, (int) value);
                    break;

                case AnimatorControllerParameterType.Bool:
                    playable.SetBool(parameter.nameHash, (bool) value);
                    break;

                case AnimatorControllerParameterType.Trigger:
                    if ((bool) value)
                        playable.SetTrigger(parameter.nameHash);
                    else
                        playable.ResetTrigger(parameter.nameHash);
                    break;

                default:
                    throw new ArgumentException($"Unsupported {nameof(AnimatorControllerParameterType)}: {parameter.type}");
            }
        }
    }
}