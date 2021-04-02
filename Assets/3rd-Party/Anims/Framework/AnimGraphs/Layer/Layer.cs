using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    /// <summary>
    /// 身体层级数据
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Layer : System.IDisposable
    {
        public int layerIndex;
        public int stateName;
       
        public LayerAsset layerAsset;
        public AnimationMixerPlayable mixerPlayable;
        public GCHandle handles;
        public NativeList<PlayCommand> readyPlayQueue;
        public NativeList<GCHandle> nodes;
        
        public bool isTransition;
        //only in transition we has dst
        public int dstStateName;

        public float LayerWeight;
        public float InputWeight;
        
        public Dictionary<int, NodeHandle> handleMapping {
            get
            {
                return handles.Target as Dictionary<int, NodeHandle>;
            }
        }

        public void Dispose()
        {
            if (AnimGraphLoader.UnloadAvatarMask != null)
            {
                AnimGraphLoader.UnloadAvatarMask(layerAsset.avatarMaskHash, 0);
            }
            
            if (handles.IsAllocated)
            {
                handles.Free();
            }

            if (nodes.IsCreated)
            {
                for (int i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i].Target as RuntimePlayNode;
                    if (node != null)
                    {
                        node.Dispose();
                    }
                }
                nodes.Dispose();
            }
            if (readyPlayQueue.IsCreated)
                readyPlayQueue.Dispose();
            if (mixerPlayable.IsValid())
                mixerPlayable.Destroy();
        }
    }
}
