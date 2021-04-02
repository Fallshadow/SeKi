using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Framework.AnimGraphs
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct AnimGraphHead
    {
        public int version;
        public int dynamicOffset;
        public int nodeMappingOffset;
        public int parametersOffset;
        public int layersOffset;
    }
}