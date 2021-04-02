using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Framework.AnimGraphs
{
    public unsafe struct LayerAsset
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public int layer;
            public int avatarMaskAsset;
            public LayerBlendingMode blendingMode;
            public int layerHash;
        }

        private byte* dynamicPtr;
        private _Data* data;

        public bool isNull => data == null;

        public static int SizeOf => UnsafeUtility.SizeOf<_Data>();

        public LayerAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }

        public LayerBlendingMode blendingMode => data->blendingMode;
        public int layer => data->layer;
        public AvatarMask avatarMask (int overrideType)
        {
            if (AnimGraphLoader.LoadAvatarMask == null) return null;
            
            if (data->avatarMaskAsset != 0)
            {
                return AnimGraphLoader.LoadAvatarMask(data->avatarMaskAsset, overrideType);
            }
            
            Debug.Log("Current Avatar has no Avatar mask!");
            
            return null;
        } 
        public int layerHash => data->layerHash;

        public int avatarMaskHash => data->avatarMaskAsset;
    }
}