using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public class RuntimeLayerTransitionNode : RuntimeNode
    {
        public LayerTransitionType transitionType { get; private set; }
        private AnimationLayerMixerPlayable layerMixer;


        public void Preparatory(AnimationLayerMixerPlayable layerMixer, uint startTime, uint duration,
            LayerTransitionType transitionType, int layer)
        {
            this.layerMixer = layerMixer;
            mark = GenMark();
            if(transitionType == LayerTransitionType.FadeOut)
            {
                intervalStart = startTime + duration;
                intervalEnd = intervalStart + duration;
            }
            else
            {
                intervalStart = startTime;
                intervalEnd = startTime + duration;
            }

            //Debug.LogError($"s start {intervalStart} intervalEnd {intervalEnd} layer {layer} type {transitionType}");
            this.transitionType = transitionType;
            layerIndex = layer;
            isActive = true;
        }

        public override void Dispose()
        {
            transitionType = default;
            layerMixer = default;
            isActive = false;
        }

        public void InternalUpdate(ref Layer layer, uint localTime)
        {
            float t = 1;
            uint duration = intervalEnd - intervalStart;
            if (localTime >= duration)
            {
                isActive = false;
                switch (transitionType)
                {
                    case LayerTransitionType.FadeIn:

                        break;
                    case LayerTransitionType.FadeOut:
                        t = 1 - t;
                        var count = layer.nodes.Length;
                        // 权重标记为1
                        for (int i = 0; i < count; i++)
                        {
                            var node = layer.nodes[i].Target as RuntimePlayNode;
                            if (node == null)
                                continue;
                            node.isActive = false;
                        }
                        break;
                }

                layer.InputWeight = t;
                layerMixer.SetInputWeight(layerIndex,  layer.InputWeight * layer.LayerWeight);

                //Debug.LogError($"e start {intervalStart} intervalEnd {intervalEnd} localTime {localTime} weight {t} layer {layerIndex} type {transitionType}");
                return;
            }

            if (duration != 0)
            {
                t = math.clamp(localTime.ToFloatTime() / duration.ToFloatTime(), 0f, 1f);
            }
            switch (transitionType)
            {
                case LayerTransitionType.FadeIn:

                    break;
                case LayerTransitionType.FadeOut:
                    t = 1 - t;
                    break;
            }
            
            layer.InputWeight = t;
            //Debug.LogError($"p start {intervalStart} intervalEnd {intervalEnd} localTime {localTime} weight {t} layer {layerIndex} type {transitionType}");
            layerMixer.SetInputWeight(layerIndex, layer.InputWeight * layer.LayerWeight);
        }
    }
}