using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Framework.AnimGraphs
{
    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct AnimGraphAssetIDMap
    {
#pragma warning disable CS0414,CS0649
        private readonly int slotCount;
#pragma warning restore CS0414,CS0649
        private AnimGraphHashEntry first;

        public int Find(int id)
        {
            var slotIndex = ((uint)id) % slotCount;
            AnimGraphHashEntry* slots = (AnimGraphHashEntry*)((byte*)UnsafeUtility.AddressOf(ref this) + UnsafeUtility.SizeOf<int>());
            AnimGraphHashEntry* entry;
            do
            {
                entry = slots + slotIndex;
                if (entry->id == id) return entry->value;
                slotIndex = entry->next;
            } while (slotIndex >= 0);
            return -1;
        }
    }

}