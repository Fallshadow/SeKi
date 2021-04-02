using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {
        private void InitLayer(AnimGraphPlayerConfig data)
        {
            const int NODE_COUNT = 10;

            var layerAssets = controller.GetLayerAssets();

            bodyLayer = AnimationLayerMixerPlayable.Create(m_Graph, layerAssets.Length);
            m_Graph.Connect(bodyLayer, 0, controlPlayable, 0);
            controlPlayable.SetInputWeight(0, 1);

            layers = new Layer[layerAssets.Length];
            for (int i = 0; i < layerAssets.Length; i++)
            {
                LayerAsset layerAsset = layerAssets[i];
                ref var layer = ref layers[i];

                layer.LayerWeight = 1f;
                layer.InputWeight = 1f;
                
                var mixerPlayable = AnimationMixerPlayable.Create(m_Graph, NODE_COUNT);
                m_Graph.Connect(mixerPlayable, 0, bodyLayer, i);
                bodyLayer.SetInputWeight(i, 0);
                if (layerAsset.avatarMask(OverrideType) != null)
                {
                    bodyLayer.SetLayerMaskFromAvatarMask((uint)i, layerAsset.avatarMask(OverrideType));
                }
                switch (layerAsset.blendingMode)
                {
                    case LayerBlendingMode.Override:
                        bodyLayer.SetLayerAdditive((uint)i, false);
                        break;
                    case LayerBlendingMode.Additive:
                        bodyLayer.SetLayerAdditive((uint)i, true);
                        break;
                }
                if (layerAsset.layer != i)
                    Debug.LogError($"层级Index错误");
                layer.layerIndex = i;
                layer.layerAsset = layerAsset;
                layer.mixerPlayable = mixerPlayable;
                layer.readyPlayQueue = new NativeList<PlayCommand>(10, Allocator.Persistent);
                layer.nodes = new NativeList<System.Runtime.InteropServices.GCHandle>(10, Allocator.Persistent);
                layer.handles = System.Runtime.InteropServices.GCHandle.Alloc(new Dictionary<int, NodeHandle>(10));
            }
        }

        private void AddLayerFadeInTransition(ref Layer layer, LayerTransitionAsset fadeInTtransLayerAsset)
        {
            if (currentTimeLayerTransitionNodes.Count > 0 && currentTimeLayerTransitionNodes[currentTimeLayerTransitionNodes.Count - 1].transitionType == LayerTransitionType.FadeOut)
                goto Trans;

            var layerWeight = bodyLayer.GetInputWeight(layer.layerIndex);
            if (layerWeight != 1)
                goto Trans;
            return;

        Trans:
            if (fadeInTtransLayerAsset.isNull)
                AddRuntimeLayerTransitionNode(bodyLayer, localTime, 0, LayerTransitionType.FadeIn, layer.layerIndex);
            else
                AddRuntimeLayerTransitionNode(bodyLayer, localTime, fadeInTtransLayerAsset.duration, LayerTransitionType.FadeIn, layer.layerIndex);
        }

        private void AddLayerFadeOutTransition(ref Layer layer, LayerTransitionAsset fadeOutTransLayerAsset, RuntimePlayNode playNode)
        {
            if (layer.layerIndex == 0)
                return;

            if (playNode.isLooping)
                return;

            if (fadeOutTransLayerAsset.isNull)
            {
                AddRuntimeLayerTransitionNode(bodyLayer, playNode.intervalEnd, 0, LayerTransitionType.FadeOut, layer.layerIndex);
            }
            else
            {
                var startTime = playNode.intervalEnd - fadeOutTransLayerAsset.duration;
                AddRuntimeLayerTransitionNode(bodyLayer, startTime, fadeOutTransLayerAsset.duration, LayerTransitionType.FadeOut, layer.layerIndex);
            }
        }
    }
}