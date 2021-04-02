using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {
        private RuntimeTransitionNode AddRuntimeTransitionNode(int layerIndex, uint startTime, uint duration, int destination, int nodeHash)
        {
            RuntimeTransitionNode tranistionNode = new RuntimeTransitionNode();
            tranistionNode.Preparatory(layerIndex, startTime, duration, destination, nodeHash);
            timeline.AddNode(tranistionNode);
            return tranistionNode;
        }

        private RuntimeLayerTransitionNode AddRuntimeLayerTransitionNode(AnimationLayerMixerPlayable layerMixer, uint startTime, uint duration,
            LayerTransitionType transitionType, int layer)
        {
            RuntimeLayerTransitionNode layerTransitionNode = new RuntimeLayerTransitionNode();
            layerTransitionNode.Preparatory(layerMixer, startTime, duration, transitionType, layer);
            timeline.AddNode(layerTransitionNode);
            return layerTransitionNode;
        }

        private RuntimePlayNode AddRuntimePlayNode(int layerIndex, uint startTime, ref NodeAsset nodeAsset, NodeHandle nodeHandle,float offset)
        {
            var playNode = new RuntimePlayNode();
            playNode.Preparatory(this, layerIndex, startTime, ref nodeAsset, nodeHandle, offset);
            timeline.AddNode(playNode);
            return playNode;
        }
    }
}