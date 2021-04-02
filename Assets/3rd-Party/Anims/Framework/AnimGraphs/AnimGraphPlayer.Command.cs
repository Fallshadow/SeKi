using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {

        private void PreProcessCommand()
        {
            for (int i = 0; i < layers.Length; i++)
            {
                ref var layer = ref layers[i];
                PreProcessLayerQueue(ref layer);
            }
        }

        private void PreProcessLayerQueue(ref Layer layer)
        {
            var queue = layer.readyPlayQueue;
            if (queue.Length == 0)
                return;

            var cmd = queue[queue.Length - 1];
            switch (cmd.cmdType)
            {
                case PlayCommandType.Play:
                    ExcutePlayCommand(ref layer, cmd);
                    break;
            }
            queue.Clear();
        }

        private void ExcutePlayCommand(ref Layer layer, PlayCommand playCmd)
        {
            #region 初始化数据
            // 准备需要的数据
            var nodeAsset = playCmd.nodeAsset;
            var motionStateAsset = nodeAsset.motionStateAsset;
            var motionAsset = motionStateAsset.motion;

            // 分拣节点类型
            currentTimeLayerTransitionNodes.Clear();
            currentTimeTransitionNodes.Clear();
            currentTimePlayNodes.Clear();
            currentTimeNodes.Clear();

            //每次进行Play调用都会清理Timeline
            timeline.ClearWithoutTime(motionStateAsset.layerIndex, localTime, currentTimeNodes);
#if ANIMGRAPHS
            Debug.Log(currentTimeNodes.Dump(localTime));
#endif
            foreach (var item in currentTimeNodes)
            {
                if (item is RuntimePlayNode)
                {
                    var node = item as RuntimePlayNode;
                    currentRuntimePlayNodes.Remove(node.GetHashCode());
                    layer.handleMapping.TryGetValue(node.stateName, out NodeHandle handle);
                    if(handle != null)
                    {
                        if(node.nodeHandle == handle)
                        {
                            layer.handleMapping.Remove(node.stateName);
                        }
                    }
                    if (node.isConnected)
                        DisconnectGraph(node);
                }
                item.Dispose();
            }

            currentTimeNodes.Clear();
            timeline.IntersectsWithTime(motionStateAsset.layerIndex, localTime, currentTimeNodes);

#if ANIMGRAPHS
            Debug.Log(currentTimeNodes.Dump(localTime));
#endif

            //bool isHasNode = false;
            for (int i = 0; i < currentTimeNodes.Count; i++)
            {
                var node = currentTimeNodes[i];

                if (node is RuntimePlayNode)
                {
                    var playNode = (RuntimePlayNode)node;
//                    if (playNode.stateName == motionStateAsset.stateName)
//                    {
//                        isHasNode = true;
//                    }
                    currentTimePlayNodes.Add(playNode);
                    continue;
                }
                if (node is RuntimeTransitionNode)
                {
                    currentTimeTransitionNodes.Add((RuntimeTransitionNode)node);
                    continue;
                }
                if (node is RuntimeLayerTransitionNode)
                {
                    currentTimeLayerTransitionNodes.Add((RuntimeLayerTransitionNode)node);
                    continue;
                }
            }
            #endregion

            #region 统一计算是否要进行层级淡入处理
            AddLayerFadeInTransition(ref layer, motionStateAsset.fadeInLayerTtransition);
            #endregion

//            #region 要播放的节点存在，并且为Looping模式
//            if (isHasNode && motionAsset.isLooping)
//            {
////                if (false == motionAsset.isLooping)
////                {
////                    Debug.LogWarning($"播放指令 stateName => {motionStateAsset.stateName} 在内存中存在，忽略本次播放操作 {Time.frameCount}");
////                    Debug.LogWarning($"内存状态 ==> Transition Count {currentTimeTransitionNodes.Count} playNode count {currentTimePlayNodes.Count}");
////                    return;
////                }
//                if(currentTimeTransitionNodes.Count > 0)
//                {
//                    // 有过渡而且目标已经是该状态，取消命令执行
//                    var transitionNode = currentTimeTransitionNodes[0];
//                    if(transitionNode.destination == motionStateAsset.stateName)
//                        return;
//                }
//                // 栈中只有一个动画，说明自己到自己无需播放
//                if (currentTimeNodes.Count == 1)
//                    return;
//                var transitionAsset = GetAnyTransitionAsset(ref motionStateAsset);
//                if (transitionAsset.isNull)
//                    AddRuntimeTransitionNode(layer.layerIndex, localTime, 0, motionStateAsset.stateName);
//                else
//                    AddRuntimeTransitionNode(layer.layerIndex, localTime, transitionAsset.duration, motionStateAsset.stateName);
//                return;
//            }
//            #endregion

            #region 当前层级未播放过任何节点
            if (currentTimePlayNodes.Count == 0)
            {
                var playNode = AddRuntimePlayNode(layer.layerIndex, localTime, ref nodeAsset, layer.handleMapping[motionStateAsset.stateName], 0);
                AddRuntimeTransitionNode(layer.layerIndex, localTime, 0, playNode.stateName, playNode.GetHashCode());
                //AddLayerFadeOutTransition(ref layer, motionStateAsset.fadeOutLayerTtransition, playNode);
                return;
            }
            #endregion

            if (currentTimeTransitionNodes.Count > 0)
            {
                // 中断过渡
                RuntimeTransitionNode transitionNode = null;
                var transitionAsset = GetAnyTransitionAsset(ref motionStateAsset);
                
                var playNode = AddRuntimePlayNode(layer.layerIndex, localTime, ref nodeAsset, layer.handleMapping[motionStateAsset.stateName], transitionAsset.offset);

                
                if (transitionAsset.isNull)
                    transitionNode = AddRuntimeTransitionNode(layer.layerIndex, localTime, 0, playNode.stateName, playNode.GetHashCode());
                else
                    transitionNode = AddRuntimeTransitionNode(layer.layerIndex, localTime, transitionAsset.duration, playNode.stateName, playNode.GetHashCode());
                return;
            }

            #region 当前节点无切换，直接运作过度算法
            if (currentTimeTransitionNodes.Count == 0)
            {
                if (currentTimePlayNodes.Count != 1)
                    Debug.LogWarning($"AnimGraphPlayer 命令解析时出现多个节点未断开状态 {currentTimePlayNodes.Count} Frame => {Time.frameCount}");
                var sourceNode = currentTimePlayNodes[0];
                var t = GetTransitionAsset(sourceNode, ref motionStateAsset);

                uint startTime = 0;
                
                RuntimeTransitionNode transitionNode = null;
                if (t.hasExitTime)
                    startTime = localTime + sourceNode.GetDeltaTime(t.exitTime);
                else
                    startTime = localTime;
                var playNode = AddRuntimePlayNode(layer.layerIndex, startTime, ref nodeAsset, layer.handleMapping[motionStateAsset.stateName], t.offset);
                transitionNode = AddRuntimeTransitionNode(layer.layerIndex, startTime, t.duration, playNode.stateName, playNode.GetHashCode());                
                //Debug.Log($"New Node {stateNameDict[playNode.stateName]} hashCode:{playNode.GetHashCode()}");
                //AddLayerFadeOutTransition(ref layer, motionStateAsset.fadeOutLayerTtransition, playNode);
                return;
            }
            #endregion

            #region 有过渡器，采用AnyTransition模式
            {
                var transitionAsset = GetAnyTransitionAsset(ref motionStateAsset);
                var playNode = AddRuntimePlayNode(layer.layerIndex, localTime, ref nodeAsset, layer.handleMapping[motionStateAsset.stateName], transitionAsset.offset);
                AddRuntimeTransitionNode(layer.layerIndex, localTime, transitionAsset.duration, playNode.stateName, playNode.GetHashCode());                
                return;
            }
            #endregion
        }

    }
}
