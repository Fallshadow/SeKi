using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe class RuntimeTransitionNode : RuntimeNode
    {
        public int destination;
        public int dstNodeHash;
        
        public void Preparatory(int layerIndex, uint startTime, uint duration, int destination, int dstNodeHash)
        {
            mark = GenMark();
            this.layerIndex = layerIndex;
            intervalStart = startTime;
            intervalEnd = duration + intervalStart;
            this.destination = destination;
            this.dstNodeHash = dstNodeHash;
            
            isActive = true;
        }

        public override void Dispose()
        {
            isActive = false;
        }

        private bool checkNode(RuntimePlayNode node)
        {
            return node.GetHashCode() == dstNodeHash;
        }
        
        
        public void InternalUpdate(ref Layer layer, uint localTime)
        {
            uint duration = intervalEnd - intervalStart;

            if (localTime >= duration)
            {
                var count = layer.nodes.Length;
                // 权重标记为1
                for (int i = 0; i < count; i++)
                {
                    var node = layer.nodes[i].Target as RuntimePlayNode;
                    if (node == null)
                        continue;
                    if (checkNode(node))
                    {
                        node.SetWeight(1);
                    }
                    else
                    {
                        node.SetWeight(0);
                        node.isActive = false;
                    }
                }
                isActive = false;
                return;
            }

            if (duration != 0)
            {
                float t = math.clamp(localTime.ToFloatTime() / duration.ToFloatTime(), 0f, 1f);
                float totalWeight = 0;
                var count = layer.nodes.Length;
                for (int i = 0; i < count; i++)
                {
                    var node = layer.nodes[i].Target as RuntimePlayNode;
                    if (node == null)
                        continue;

                    if (!checkNode(node))
                    {
                        if(node.parentViald() == false)
                        {
                            Debug.LogError("错误");
                        }
                        totalWeight += node.GetWeight();
                    }
                }
                for (int i = 0; i < count; i++)
                {
                    var node = layer.nodes[i].Target as RuntimePlayNode;
                    if (node == null)
                        continue;
                    if (checkNode(node))
                    {
                        node.SetWeight(t);
                    }
                    else
                    {
                        // 单一节点权重/所有节点权重=比率 * 过渡系数
                        float otherWeight = math.clamp(node.GetWeight() / totalWeight * (1 - t), 0, 1);
                        node.SetWeight(otherWeight);
                    }
                }
            }
            else
            {
                // duration == 0表示为立即执行
                var count = layer.nodes.Length;
                // 权重标记为1
                for (int i = 0; i < count; i++)
                {
                    var node = layer.nodes[i].Target as RuntimePlayNode;
                    if (node == null)
                        continue;
                    if (checkNode(node))
                    {
                        node.SetWeight(1);
                    }
                    else
                    {
                        node.SetWeight(0);
                        node.isActive = false;
                    }
                }
                isActive = false;
            }

        }
    }
}