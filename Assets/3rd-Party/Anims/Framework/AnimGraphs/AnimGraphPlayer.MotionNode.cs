using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {
        private void ConnectGraph(RuntimePlayNode node)
        {
#if ANIMGRAPHS            
            Debug.Log($"创建链接 {Time.frameCount} => {stateNameDict[node.stateName]} {node.GetHashCode()}");
#endif
            var layer = layers[node.layerIndex];
            var mixer = layer.mixerPlayable;
            var count = mixer.GetInputCount();
            for (int i = 0; i < count; i++)
            {
                if (mixer.GetInput(i).IsNull())
                {
                    if (builder != null)
                        builder.SyncSetting();
                    node.Connecting(this, mixer, i);
                    layer.nodes.Add(GCHandle.Alloc(node));
                    return;
                }
            }
        }

        private void DisconnectGraph(RuntimePlayNode node)
        {
#if ANIMGRAPHS            
            Debug.Log($"断开链接 {Time.frameCount} => {stateNameDict[node.stateName]} {node.GetHashCode()}");
#endif
            ref var layer = ref layers[node.layerIndex];
            var mixer = layer.mixerPlayable;
            mixer.DisconnectInput(node.inputPort);
            for (int i = layer.nodes.Length - 1; i >= 0 ; i--)
            {
                var n = layer.nodes[i].Target as RuntimePlayNode;
                if (n.stateName == node.stateName && n.GetHashCode() == node.GetHashCode()) 
                {
                    node.Disconnected();
                    // 清理Layer
                    layer.nodes[i].Free();
                    layer.nodes.RemoveAtSwapBack(i);
                    return;
                }
            }
            Debug.LogError($"AnimGraphPlayer断开链接失败 {node.stateName} {node.GetHashCode()}");
        }

    }
}