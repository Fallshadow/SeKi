using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Framework.AnimGraphs
{
    public unsafe struct ArrayNode
    {
        public byte* dynamicPtr;
        public void* data;
    }

    /// <summary>
    /// 通用数组
    /// </summary>
    public unsafe struct NArray<T> where T : unmanaged
    {
        // 数组元素个数
        public int Length { get; private set; }
        private byte* p;
        private byte* dynamicPtr;
        private int size;

        public NArray(byte* dataPtr, byte* dynPtr, int size)
        {
            Length = *(int*)dataPtr;
            p = dataPtr + UnsafeUtility.SizeOf<int>();
            dynamicPtr = dynPtr;
            this.size = size;
        }

        public bool isNull => p == null;

        public T this[int index]
        {
            get
            {
                ArrayNode node = new ArrayNode();
                ArrayNode* ptr = (ArrayNode*)UnsafeUtility.AddressOf<ArrayNode>(ref node);
                ptr->data = (p + size * index);
                ptr->dynamicPtr = dynamicPtr;
                return *(T*)ptr;
            }
        }
    }
}