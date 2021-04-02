using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {
        private void InitPipeline()
        {
            controlPlayable = ScriptPlayable<PlayerControlPlayable>.Create(m_Graph, 1);
            controlPlayable.GetBehaviour().SetAnimGraphPlayer(this);
            controlPlayable.SetTraversalMode(PlayableTraversalMode.Passthrough);
        }

        private void PreProcessTimeline()
        {
            // 预处理当前时间片相关数据
            currentTimeNodes.Clear();
            currentTimePlayNodes.Clear();
            currentTimeTransitionNodes.Clear();
            currentTimeLayerTransitionNodes.Clear();

            // 当前时间是一个线段而不是一个点
            uint uintEndTime = localTime;
            uint uintStartTime = (uint)math.max((int)localTime - (int)deltaTime, 0);
            // 获得线段上的所有Node节点
            timeline.IntersectsWithRange(uintStartTime, uintEndTime, currentTimeNodes);

            if(currentTimeNodes.Count == 0)
            {
#if ANIMGRAPHS                
                Debug.LogWarning($"{Time.frameCount} 当前动画系统中无任何播放节点处于激活状态");
#endif
                // 确定历史是否有播放
                foreach (var item in currentRuntimePlayNodes.Values)
                {
                    var t = localTime - item.intervalStart;
                    item.InternalUpdate(t);
                    item.HandleUpdate(t);
                }
            }

            // 因为timeline里会排序
            // 所以实际上currentTime拿出来的数据是有序的
            // 因此二次分发依然有序
            for (int i = 0; i < currentTimeNodes.Count; i++)
            {
                var node = currentTimeNodes[i];
                if (node is RuntimePlayNode)
                {
                    currentTimePlayNodes.Add(node as RuntimePlayNode);
                    continue;
                }
                if (node is RuntimeTransitionNode)
                {
                    currentTimeTransitionNodes.Add(node as RuntimeTransitionNode);
                    continue;
                }
                if (node is RuntimeLayerTransitionNode)
                {
                    currentTimeLayerTransitionNodes.Add(node as RuntimeLayerTransitionNode);
                    continue;
                }
            }
        }

        private void PreProcessGraph()
        {
            for (int i = 0; i < currentTimePlayNodes.Count; i++)
            {
                var node = currentTimePlayNodes[i];
                
                if (!currentRuntimePlayNodes.ContainsKey(node.GetHashCode()))
                {
                    ConnectGraph(node);
                    currentRuntimePlayNodes.Add(node.GetHashCode(), node);
                }
            }
        }

        private void ProcessNode()
        {
            for (int i = 0; i < currentTimePlayNodes.Count; i++)
            {
                var node = currentTimePlayNodes[i];
                node.InternalUpdate(localTime - node.intervalStart);
            }

            if (currentTimeTransitionNodes.Count > 0)
            {
                bool* m_influences = stackalloc bool[layers.Length];
                for (int i = currentTimeTransitionNodes.Count - 1; i >= 0 ; i--)
                {
                    var node = currentTimeTransitionNodes[i];
                    if (m_influences[node.layerIndex])
                    {
                        node.isActive = false;
                        continue;
                    }
                    m_influences[node.layerIndex] = true;
                    ref var layer = ref layers[node.layerIndex];
                    node.InternalUpdate(ref layer, localTime - node.intervalStart);
                    
                    layer.isTransition = true;
                    layer.dstStateName = node.destination;

                    if(node.isActive == false)
                    {
                        layer.stateName = node.destination;
                        layer.isTransition = false;
                        layer.dstStateName = -1;
                    }
                }
            }

            if (currentTimeLayerTransitionNodes.Count > 0)
            {
                bool* m_influences = stackalloc bool[layers.Length];
                for (int i = currentTimeLayerTransitionNodes.Count - 1; i >= 0; i--) 
                {
                    var node = currentTimeLayerTransitionNodes[i];
                    if (m_influences[node.layerIndex])
                    {
                        node.isActive = false;
                        continue;
                    }
                    m_influences[node.layerIndex] = true;
                    ref var layer = ref layers[node.layerIndex];
                    node.InternalUpdate(ref layer,localTime - node.intervalStart);
                    layer.isTransition = true;
                    if (node.isActive == false)
                    {
                        layer.isTransition = false;
                    }
                }
            }

        }

        private void ProcessHandle()
        {
            foreach (var item in currentRuntimePlayNodes.Values)
            {
                item.HandleUpdate(localTime - item.intervalStart);
            }
        }

        private void PostProcessGraph()
        {
            currentRemovePlayNodes.Clear();
            // 计算所有在缓存中的Node哪些需要清理出内存
            foreach (var item in currentRuntimePlayNodes.Values)
            {
                if (item.isActive == false)
                {
                    currentRemovePlayNodes.Add(item);
                }
            }

            for (int i = 0; i < currentRemovePlayNodes.Count; i++)
            {
                var playNode = currentRemovePlayNodes[i];
                if (!currentRuntimePlayNodes.Remove(playNode.GetHashCode()))
                {
                    Debug.LogError($"删除RuntimeNode节点失败 {playNode.stateName} {playNode.GetHashCode()}");
                }
                DisconnectGraph(playNode);
            }

           
        }

        private void PostProcessTimeline()
        {
            currentTimeNodes.Clear();
            timeline.ClearTimeout(localTime, currentTimeNodes);
            for (int i = 0; i < currentTimeNodes.Count; i++)
            {
                var node = currentTimeNodes[i];
                if (node is RuntimePlayNode)
                {
                    ref var layer = ref layers[node.layerIndex];
                    var playNode = node as RuntimePlayNode;

                    layer.handleMapping.TryGetValue(playNode.stateName, out NodeHandle handle);
                    if (handle != null)
                    {
                        if (playNode.nodeHandle == handle)
                        {
                            layer.handleMapping.Remove(playNode.stateName);
                        }
                    }
                }
                node.Dispose();
            }
        }

        private void PostPrecessClean()
        {
            currentRemovePlayNodes.Clear();
            currentTimeNodes.Clear();
            currentTimePlayNodes.Clear();
            currentTimeTransitionNodes.Clear();
            currentTimeLayerTransitionNodes.Clear();
        }
    }
}