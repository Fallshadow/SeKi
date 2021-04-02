using System.Runtime.InteropServices;

namespace Framework.AnimGraphs
{

    public unsafe struct BlendTreeAsset 
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public MotionType motionType;
            public bool isLooping;
            public uint avgDuration;
            public BlendTreeType blendType;
            public sbyte blendParameterX;
            public sbyte blendParameterY;
            public int childMotionsOffset;
        }

        private byte* dynamicPtr;
        private _Data* data;

        public BlendTreeAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }
        
        public MotionType motionType => data->motionType;
        public bool isLooping => data->isLooping;
        public uint avgDuration => data->avgDuration;
        public BlendTreeType blendType => data->blendType;
        public sbyte blendParameterX => data->blendParameterX;
        public sbyte blendParameterY => data->blendParameterY;
        public NArray<ChildMotionAsset> childMotions => data->childMotionsOffset == -1 ? default : new NArray<ChildMotionAsset>(dynamicPtr + data->childMotionsOffset, dynamicPtr, ChildMotionAsset.SizeOf);
    }
}
