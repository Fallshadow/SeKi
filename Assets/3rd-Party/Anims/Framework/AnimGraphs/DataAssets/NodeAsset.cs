using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Framework.AnimGraphs
{
    public unsafe struct NodeAsset
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public int nodeName;
            public int motionStateAsset;
        }

        private byte* dynamicPtr;
        private _Data* data;

        public bool isNull => data == null;

        public static int SizeOf => UnsafeUtility.SizeOf<_Data>();

        public NodeAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }

        public int nodeName => data->nodeName;
        public MotionStateAsset motionStateAsset => data->motionStateAsset == -1 ? default : new MotionStateAsset(dynamicPtr + data->motionStateAsset, dynamicPtr);
    }
}