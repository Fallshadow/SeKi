using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Framework.AnimGraphs
{
    public unsafe struct LayerTransitionAsset
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public uint duration;
            public LayerTransitionType type;
        }

        private byte* dynamicPtr;
        private _Data* data;

        public bool isNull => data == null;

        public static int SizeOf => UnsafeUtility.SizeOf<_Data>();

        public LayerTransitionAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }

        public uint duration => data->duration;
        public LayerTransitionType type => data->type;
    }
}