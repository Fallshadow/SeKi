using System;
using UnityEngine;

namespace ASeKi.Utility
{
    public static class Common
    {
        public static void UnLoadAllUnusedAssets()
        {
            Resources.UnloadUnusedAssets();
        }
        
        public static void GcCollect()
        {
            GC.Collect();
        }
    }
}