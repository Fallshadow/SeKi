using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {
        private TransitionAsset GetAnyTransitionAsset(ref MotionStateAsset motionStateAsset)
        {
            return motionStateAsset.anyTransition;
        }

        private TransitionAsset GetTransitionAsset(RuntimePlayNode sourceNode, ref MotionStateAsset destStateAsset)
        {
            var sourceStateAsset = sourceNode.nodeAsset.motionStateAsset;
            var sourceMotionAsset = sourceStateAsset.motion;
            var sourceTransition = sourceStateAsset.transitions;
            var sourceTime = sourceNode.fixedTime;

            var transitionAsset = destStateAsset.anyTransition;

            if (sourceMotionAsset.isLooping)
            {
                for (int i = 0; i < sourceTransition.Length; i++)
                {
                    var t = sourceTransition[i];
                    if(t.destinationState == destStateAsset.stateName)
                    {
                        if(!transitionAsset.hasExitTime)
                        {
                            transitionAsset = t;
                            continue;
                        }
                        if (t.hasExitTime)
                        {
                            uint distanceA = GetTransitionDistance(transitionAsset.exitTime, sourceTime, sourceNode.duration);
                            uint distanceB = GetTransitionDistance(t.exitTime, sourceTime, sourceNode.duration);
                            if (distanceA > distanceB)
                                transitionAsset = t;
                            continue;
                        }
                        //TODO: 这里提示下忽略日志
                    }
                }
            }
            else
            {
                for (int i = 0; i < sourceTransition.Length; i++)
                {
                    var t = sourceTransition[i];
                    if(t.destinationState == destStateAsset.stateName)
                    {
                        if(!transitionAsset.hasExitTime && !t.hasExitTime)
                        {
                            transitionAsset = t;
                            continue;
                        }

                        if (!transitionAsset.hasExitTime && t.hasExitTime && t.exitTime >= sourceTime)
                        {
                            transitionAsset = t;
                            continue;
                        }
                        if (transitionAsset.hasExitTime && t.hasExitTime && t.exitTime >= sourceTime)
                        {
                            uint distanceA = transitionAsset.exitTime - sourceTime;
                            uint distanceB = t.exitTime - sourceTime;
                            if (distanceB < distanceA)
                                transitionAsset = t;
                            continue;
                        }
                        // TODO: 日志提示被抛弃流程
                    }
                }
            }

            return transitionAsset;
        }


        private uint GetTransitionDistance(uint exitTime,uint localTime,uint duration)
        {
            return exitTime >= localTime ? exitTime - localTime : duration - localTime + exitTime;
        }

    }
}