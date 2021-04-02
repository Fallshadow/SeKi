using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Framework.AnimGraphs
{

    public enum LayerBlendingMode : ushort
    {
        Override = 0,
        Additive = 1
    }
}