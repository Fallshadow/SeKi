using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Framework.AnimGraphs
{
    public unsafe struct ParameterAsset
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct _Data
        {
            public int nameHash;
            public ParameterType parameterType;
        }
        public static int SizeOf => UnsafeUtility.SizeOf<_Data>();

        private byte* dynamicPtr;
        private _Data* data;

        public ParameterAsset(byte* dataPtr, byte* dynPtr)
        {
            data = (_Data*)dataPtr;
            dynamicPtr = dynPtr;
        }

        public int nameHash => data->nameHash;
        public ParameterType parameterType => data->parameterType;
    }
}