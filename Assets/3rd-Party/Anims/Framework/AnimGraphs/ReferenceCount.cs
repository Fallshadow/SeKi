using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;

namespace Framework.AnimGraphs
{
    public class ReferenceCount<T> : System.IDisposable where T : class
    {
#if UNITY_EDITOR
        ~ReferenceCount()
        {
            if (mapping.IsCreated)
            {
                Debug.LogError("Native内存未回收");
                Dispose();
            }
        }
#endif
        struct CountData
        {
            internal int count;
            internal GCHandle handle;
        }

        private NativeHashMap<int, CountData> mapping;

        public int Length
        {
            get
            {
                return mapping.Length;
            }
        }

        public ReferenceCount(int capacity = 64)
        {
            mapping = new NativeHashMap<int, CountData>(capacity, Allocator.Persistent);
        }

        public bool ContainsKey(int key)
        {
            return mapping.ContainsKey(key);
        }

        /// <summary>
        /// 添加一个对象的引用计数
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void Register(int key, T value)
        {
            CountData data;
            if (!mapping.TryGetValue(key, out data))
            {
                data = new CountData();
                data.count = 0;
                data.handle = GCHandle.Alloc(value);
                mapping.TryAdd(key, data);
            }
        }

        /// <summary>
        /// 不增加引用计数
        /// 如果存在该对象则返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get(int key)
        {
            CountData data;
            if (mapping.TryGetValue(key, out data))
            {
                return (T)data.handle.Target;
            }
            return null;
        }

        /// <summary>
        /// 调用该函数会增加引用计数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T New(int key)
        {
            CountData data;
            if (mapping.TryGetValue(key, out data))
            {
                data.count++;
                mapping[key] = data;
                return (T)data.handle.Target;
            }
            return null;
        }

        public T Remove(int key)
        {
            CountData data;
            if (mapping.TryGetValue(key, out data))
            {
                T value = data.handle.Target as T;
                mapping.Remove(key);
                data.handle.Free();
                return value;
            }
#if UNITY_EDITOR
            Debug.LogError($"要移除的Bundle {key} 不存在");
#endif
            return null;
        }

        /// <summary>
        /// 移除一次引用
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Recycling(int key)
        {
            CountData data;
            if (mapping.TryGetValue(key, out data))
            {
                T value = data.handle.Target as T;
                data.count--;
                if (data.count == 0)
                {
                    mapping.Remove(key);
                    data.handle.Free();
                }
                else
                {
                    mapping[key] = data;
                }
                return data.count <= 0 ? value : null;
            }
            return null;
        }

        public void Dispose()
        {
            mapping.Dispose();
        }
    }
}