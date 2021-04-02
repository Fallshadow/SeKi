using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct AnimGraphHashEntry
    {
        public int id;
        public int value;
        public int next;
    }
}