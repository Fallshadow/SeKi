using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Framework.AnimGraphs
{
    public unsafe struct MotionAsset
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public MotionType motionType;
            public bool isLooping;
            public uint avgDuration;
        }

        private byte* dynamicPtr;
        private _Data* data;

        public MotionAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }

        public MotionType motionType => data->motionType;
        public bool isLooping => data->isLooping;
        public uint avgDuration => data->avgDuration;

        public bool isNull => data == null;

        public T* CastStruct<T>() where T : unmanaged
        {
#if UNITY_EDITOR
            switch (motionType)
            {
                case MotionType.None:
                    if (typeof(T) != typeof(MotionAsset))
                        throw new System.Exception($"类型转换错误 {typeof(T)} to {typeof(MotionAsset)}");
                    break;
                case MotionType.BlendTree:
                    if (typeof(T) != typeof(BlendTreeAsset))
                        throw new System.Exception($"类型转换错误 {typeof(T)} to {typeof(BlendTreeAsset)}");
                    break;
                case MotionType.AnimationClip:
                    if (typeof(T) != typeof(AnimationClipAsset))
                        throw new System.Exception($"类型转换错误 {typeof(T)} to {typeof(AnimationClipAsset)}");
                    break;
                default:
                    break;
            }
#endif
            return (T*)UnsafeUtility.AddressOf(ref this);
        }

    }
}
