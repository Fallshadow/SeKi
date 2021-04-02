using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Framework.AnimGraphs
{
    public unsafe struct TransitionAsset 
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public bool hasExitTime;
            public int destinationState;
            public uint exitTime;
            public uint duration;
            public float offset;
        }

        private byte* dynamicPtr;
        private _Data* data;

        public bool isNull => data == null;

        public TransitionAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }

        public static int SizeOf => UnsafeUtility.SizeOf<_Data>();

        public bool hasExitTime => data->hasExitTime;
        public int destinationState => data->destinationState;
        public uint exitTime => data->exitTime;
        public uint duration => data->duration;
        public float offset => data->offset;
    }
}