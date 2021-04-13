using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ASeKi.action
{
    [System.Serializable]
    public class ConditionConfigData
    {
        public string Name = "";
        public int HashName = -1;
        public ConditionValueType ValueType; 
        public ConditionValue Value = default;
    }
    
    public enum ConditionValueType : ushort
    {
        Float = 0,
        Int = 1,
        Bool = 2,
        //Trigger = 3
    }
    
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public unsafe struct ConditionValue
    {
        [FieldOffset(0)] private float f;
        [FieldOffset(0)] private int i;
        [FieldOffset(0)] private bool b;

        public float FloatValue => f;
        public int IntValue => i;
        public bool BoolValue => b;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ConditionValue(float v)
        {
            i = default;
            b = default;
            f = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ConditionValue(bool v)
        {
            i = default;
            f = default;
            b = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ConditionValue(int v)
        {
            f = default;
            b = default;
            i = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ConditionValue(float v)
        {
            return new ConditionValue(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ConditionValue(int v)
        {
            return new ConditionValue(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator ConditionValue(bool v)
        {
            return new ConditionValue(v);
        }
    }
    
    [Serializable]
    public class ActionConditionConfigSO : ScriptableObject
    {
        public List<ConditionConfigData> Data = new List<ConditionConfigData>();
    }
}