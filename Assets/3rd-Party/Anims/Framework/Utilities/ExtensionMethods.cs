using Framework.AnimGraphs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public unsafe static class ExtensionMethods
{
    internal static readonly uint IntPtrVersion;

    private static float UNIT = 10000f;
    static ExtensionMethods()
    {
        IntPtrVersion = (uint)(new DateTime(1900, 1, 1) - DateTime.UtcNow).TotalSeconds;
    }

    public static void WriteStruct<T>(this BinaryWriter writer,ref T value) where T:unmanaged
    {
        var size = UnsafeUtility.SizeOf<T>();
        var p = (byte*)UnsafeUtility.AddressOf(ref value);
        for (int i = 0; i < size; i++)
        {
            writer.Write(p[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T* static_cast<T>(this IntPtr ptr) where T : unmanaged
    {
        return (T*)ptr.ToPointer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IntPtrEx ToPtrEx(this IntPtr ptr)
    {
        return new IntPtrEx(ptr);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToUintTime(this float value)
    {
        return (uint)(value * UNIT);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToIntTime(this float value)
    {
        return (int)(value * UNIT);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToFloatTime(this uint value)
    {
        return (float)value / UNIT;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int HashCode(this string value)
    {
        //为了兼容controller 统一hash算法！
        return Animator.StringToHash(value);

//        int h = 0;
//        fixed(char* c = value)
//        {
//            for (int i = 0; i < value.Length; i++)
//            {
//                h = 31 * h + c[i];
//            }
//        }
//        return h;
    }

    public static string Dump(this List<RuntimeNode> array,uint localTime)
    {
        StringBuilder sb = new StringBuilder(256);
        sb.Append($"FrameCount {Time.frameCount} {localTime}\n");
        foreach (var item in array)
        {
            if(item is RuntimePlayNode)
            {
                RuntimePlayNode playNode = item as RuntimePlayNode;
                sb.Append($"RuntimePlayNode {playNode.stateName} {playNode.GetHashCode()} {playNode.intervalStart} {playNode.intervalEnd}\n");
                continue;
            }
            if(item is RuntimeTransitionNode)
            {
                RuntimeTransitionNode tNode = item as RuntimeTransitionNode;
                sb.Append($"RuntimeTransitionNode {tNode.destination} {tNode.GetHashCode()} {tNode.intervalStart} {tNode.intervalEnd}\n");
                continue;
            }
            if(item is RuntimeLayerTransitionNode)
            {
                RuntimeLayerTransitionNode ltNode = item as RuntimeLayerTransitionNode;
                sb.Append($"RuntimeTransitionNode {ltNode.layerIndex} {ltNode.GetHashCode()} {ltNode.intervalStart} {ltNode.intervalEnd}\n");
                continue;
            }
        }

        return sb.ToString() ;
    }
}
