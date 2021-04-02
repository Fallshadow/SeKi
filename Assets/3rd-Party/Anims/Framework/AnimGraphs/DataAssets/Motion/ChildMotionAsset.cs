using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Framework.AnimGraphs
{
    public unsafe struct ChildMotionAsset
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public float2 postion;
            public int motionOffset;
            public float timeScale;
        }

        private byte* dynamicPtr;
        private _Data* data;

        public ChildMotionAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }

        public static int SizeOf => UnsafeUtility.SizeOf<_Data>();

        public float2 position => data->postion;
        public MotionAsset motion => data->motionOffset == -1 ? default : new MotionAsset(data->motionOffset + dynamicPtr, dynamicPtr);
        public float timeScale => data->timeScale;
    }
}
