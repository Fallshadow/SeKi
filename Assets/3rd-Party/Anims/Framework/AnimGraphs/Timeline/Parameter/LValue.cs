using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public unsafe struct LValue
    {
        [FieldOffset(0)]
        private float f;
        [FieldOffset(0)]
        private int i;
        [FieldOffset(0)]
        private bool b;


        public float FloatValue => f;
        public int IntValue => i;
        public bool BoolValue => b;

        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LValue(float v)
        {
            i = default;
            b = default;
            f = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LValue(bool v)
        {
            i = default;
            f = default;
            b = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public LValue(int v)
        {
            f = default;
            b = default;
            i = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LValue(float v) { return new LValue(v); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LValue(int v) { return new LValue(v); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator LValue(bool v) { return new LValue(v); }

    }
}