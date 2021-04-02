using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.AnimGraphs
{

    public class TimelineContainer : System.IDisposable
    {
        private List<RuntimeNode> m_Nodes;
        private List<RuntimeNode> back;
        private bool isDirty;

        internal TimelineContainer()
        {
            const int MAX = 50;
            if (m_Nodes == null)
            {
                m_Nodes = new List<RuntimeNode>(MAX);
                back = new List<RuntimeNode>(MAX);
            }
            isDirty = false;
        }

        public void Dispose()
        {
            m_Nodes.Clear();
            back.Clear();
        }

        internal void AddNode(RuntimeNode node)
        {
            m_Nodes.Add(node);
            isDirty = true;
        }

        internal void RemoveOldLayerTransitionNode(int layerIndex)
        {
            for (int i = m_Nodes.Count - 1; i >= 0; i--)
            {
                if (!(m_Nodes[i] is RuntimeLayerTransitionNode node)) continue;
                
                if (node.layerIndex == layerIndex)
                {
                    m_Nodes.Remove(node);
                    node.Dispose();
                }
            }
        }
        
        internal RuntimeNode Peek(int layerIndex)
        {
            Sort();

            for (int i = m_Nodes.Count - 1; i >= 0; i--)
            {
                var node = m_Nodes[i];
                if (node != null && node.layerIndex == layerIndex && node is RuntimePlayNode)
                    return node;
            }
            return null;
        }

        internal void IntersectsWithTime(int layerIndex, uint time, List<RuntimeNode> currentTimeNodes)
        {
            Sort();
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                var node = m_Nodes[i];
                if (node.layerIndex == layerIndex)
                {
//                    // 已经过了有效时间的节点
//                    if (node.intervalEnd < time)
//                    {
//                        continue;
//                    }
                    // 还未开始的节点
                    if (node.intervalStart > time)
                    {
                        break;
                    }
                    // 剩下的就是当前时间片中使用到的节点
                    if (node.isActive)
                        currentTimeNodes.Add(node);
                }
            }
        }

        internal void ClearWithoutTime(int layer, uint time, List<RuntimeNode> clearNodes)
        {
            int count = 0;
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                var node = m_Nodes[i];
                if (node.layerIndex != layer)
                {
                    back.Add(node);
                    continue;
                }
                count++;
                if (node.intervalEnd < time)
                {
                    clearNodes.Add(node);
                    continue;
                }
                if (node.intervalStart > time)
                {
                    clearNodes.Add(node);
                    continue;
                }
                back.Add(node);
            }
            if (count == 1)
            {
                clearNodes.Clear();
                back.Clear();
                return;
            }
            var tmp = m_Nodes;
            m_Nodes = back;
            back = tmp;
            back.Clear();
        }

        internal void ClearTimeout(uint time, List<RuntimeNode> clearNodes)
        {
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                var node = m_Nodes[i];
                if (node.isActive == false)
                {
                    // 决定对象被回收
                    clearNodes.Add(node);
                    continue;
                }
                // 节点处于运作，或者时间未到达
                if (node.isActive || node.intervalEnd >= time)
                {
                    back.Add(node);
                }
                else
                {
                    // 决定对象被回收
                    clearNodes.Add(node);
                }
            }
            var tmp = m_Nodes;
            m_Nodes = back;
            back = tmp;
            back.Clear();
        }

        internal void IntersectsWithRange(uint start, uint end, List<RuntimeNode> results)
        {
            Sort();
            for (int i = 0; i < m_Nodes.Count; i++)
            {
                var node = m_Nodes[i];
                // 已经过了有效时间的节点
                if (node.intervalEnd < start)
                    continue;
                // 还未开始的节点，因为有排序，所以每次到未开始之后所有节点都将是未开始
                if (node.intervalStart > end)
                    break;
                // 剩下的就是当前时间片中使用到的节点
                results.Add(node);
            }
        }

        private void Sort()
        {
            if (isDirty)
            {
                m_Nodes.Sort((a, b) =>
                {
                    if (a.intervalStart > b.intervalStart)
                    {
                        return 1;
                    }
                    else if (a.intervalStart < b.intervalStart)
                    {
                        return -1;
                    }
                    else
                    {
                        if (a.mark < b.mark)
                            return -1;
                        else
                        {
                            return 1;
                        }
                    }
                });
                isDirty = false;
            }
        }
    }
}
