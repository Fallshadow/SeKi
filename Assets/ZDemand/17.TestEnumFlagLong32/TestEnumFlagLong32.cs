using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 默认1是int再进行移位操作时返回的也是int下的数值，即 1 << 64  = 1 << 32 = 1 << 0 = 1
// 所以要先进行转换
[Flags]
enum EnumFlagLong64 : long
{
    TEST0 = 0,
    TEST31 = 1 << 31,
    TEST32 = (long)1 << 32,
    TEST33 = (long)1 << 33,
    TEST64 = (long)1 << 63,
}

[Flags]
enum EnumFlagULong64 : ulong
{
    TEST0 = 0,
    TEST1 = 1 << 1,
    TEST28 = 1 << 28,
    TEST31 = (ulong)1 << 31,
    TEST32 = (ulong)1 << 32,
    TEST33 = (ulong)1 << 33,
    TEST64 = (ulong)1 << 63,
}


public class TestEnumFlagLong32 : MonoBehaviour
{
    private void Start()
    {
        Debug.Log($"EnumFlagLong32.TEST0 :  {(long)EnumFlagLong64.TEST0}");
        Debug.Log($"EnumFlagLong32.TEST32 : {(long)EnumFlagLong64.TEST32}");
        Debug.Log($"EnumFlagLong32.TEST33 : {(long)EnumFlagLong64.TEST33}");
        Debug.Log($"EnumFlagLong32.TEST64 : {(long)EnumFlagLong64.TEST64}");
        
        Debug.Log($"EnumFlagULong64.TEST0 :  {(ulong)EnumFlagULong64.TEST0}");
        Debug.Log($"EnumFlagULong64.TEST28 : {(ulong)EnumFlagULong64.TEST28}");
        Debug.Log($"EnumFlagULong64.TEST32 : {(ulong)EnumFlagULong64.TEST32}");
        Debug.Log($"EnumFlagULong64.TEST33 : {(ulong)EnumFlagULong64.TEST33}");
        Debug.Log($"EnumFlagULong64.TEST64 : {(ulong)EnumFlagULong64.TEST64}");

    }
}