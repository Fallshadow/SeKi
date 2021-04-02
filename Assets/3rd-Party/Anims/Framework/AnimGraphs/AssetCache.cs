using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
namespace Framework.AnimGraphs
{
    public class AssetCache
    {
        public static bool useCache = true;

        private static ObjectPool<AssetData> pool = new ObjectPool<AssetData>(null,
            (obj) =>
            {
                obj.count = 0;
                obj.asset = null;
            });

        class AssetData
        {
            internal int count;
            internal UnityEngine.Object asset;
        }

        private Dictionary<int, AssetData> mapping;
        private Queue<int> recyclingQueue;
        private Queue<int> immediateRecyclingQueue;
        private int depth;
        private Action<int> unloadCallback;

        public AssetCache(int depth, Action<int> unloadCallback, int capacity = 64)
        {
            this.depth = depth;
            mapping = new Dictionary<int, AssetData>(capacity);
            recyclingQueue = new Queue<int>(capacity);
            immediateRecyclingQueue = new Queue<int>(5);
            this.unloadCallback = unloadCallback;
        }

        public bool ContainsKey(int key)
        {
            return mapping.ContainsKey(key);
        }

        public UnityEngine.Object New(int key)
        {
            AssetData data;
            if (mapping.TryGetValue(key, out data))
            {
                data.count++;
                return data.asset;
            }
#if DEBUG
            Debug.LogError($"想要获得的资源不存在与缓存池 key => {key}");
#endif
            return null;
        }

        public T New<T>(int key) where T : UnityEngine.Object
        {
            AssetData data;
            if (mapping.TryGetValue(key, out data))
            {
                data.count++;
                return (T)data.asset;
            }
#if DEBUG
            Debug.LogError($"想要获得的资源不存在与缓存池 key => {key}");
#endif
            return null;
        }

        public void Recycling(int key)
        {
            AssetData data;
            if (mapping.TryGetValue(key, out data))
            {
                data.count--;
                if (data.count <= 0)
                {
                    if (useCache)
                        recyclingQueue.Enqueue(key);
                    else
                        immediateRecyclingQueue.Enqueue(key);
                }
            }
        }

        public void Register(int key, UnityEngine.Object value)
        {
            if (key == 0)
            {
                Debug.LogError($"The Current Asset Key is 0");
            }
            
            AssetData data;
            if (!mapping.TryGetValue(key, out data))
            {
                data = pool.Get();
                data.count = 0;
                data.asset = value;
                mapping.Add(key, data);
            }
        }

        public void DestroyStep()
        {
            while (immediateRecyclingQueue.Count > 0)
            {
                var id = immediateRecyclingQueue.Dequeue();
                if (mapping.TryGetValue(id, out AssetData data))
                {
                    if (data.count <= 0)
                    {
                        mapping.Remove(id);
                        pool.Release(data);
                        if (unloadCallback != null)
                            unloadCallback(id);
                    }
                }
            }

            while (mapping.Count > depth && recyclingQueue.Count > 0)
            {
                var id = recyclingQueue.Dequeue();
                if (mapping.TryGetValue(id, out AssetData data))
                {
                    if (data.count <= 0)
                    {
                        mapping.Remove(id);
                        pool.Release(data);
                        if (unloadCallback != null)
                            unloadCallback(id);
                    }
                }
            }
        }

        public void Dispose()
        {
            mapping.Clear();
            recyclingQueue.Clear();
        }
    }
}