using System.Runtime.InteropServices;
using UnityEngine;

namespace Framework.AnimGraphs
{

    public unsafe struct AnimationClipAsset
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public MotionType motionType;
            public bool isLooping;
            public uint avgDuration;
            public int asset;
            public int nameHash;
        }

        private byte* dynamicPtr;
        private _Data* data;

        public AnimationClipAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }

        public bool IsNull => data == null;
        
        public MotionType motionType => data->motionType;
        public bool isLooping => data->isLooping;
        public uint avgDuration => data->avgDuration;
        public AnimationClip asset(int overrideType) {
            if (AnimGraphLoader.LoadAnimationClip != null)
                return AnimGraphLoader.LoadAnimationClip(data->asset, overrideType);
            return null;
        }
        public int assetValue => data->asset;

        public int AnimationClipHash => data->asset;

        public int NameHash => data->nameHash;
    }
}
