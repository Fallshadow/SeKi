using System;
using System.Collections.Generic;

namespace ASeKi.Utility
{
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        // 可以指定创建方法，否则会通过system直接创建
        private readonly Func<T> createFunc = null;

        public ObjectPool()
        {
            CountAll = 0;
        }

        public ObjectPool(Func<T> createFunc)
        {
            this.createFunc = createFunc;
            CountAll = 0;
        }

        public ObjectPool(int capacity)
        {
            CountAll = 0;
            m_Stack = new Stack<T>(capacity);

            List<T> listNodes = new List<T>(capacity);

            for(int i = 0; i < capacity; i++)
            {
                listNodes.Add(Get());
            }

            for(int i = 0; i < capacity; i++)
            {
                Release(listNodes[i]);
            }
        }

        public T Get()
        {
            T t;
            if(m_Stack.Count == 0)
            {
                t = createFunc == null ? Activator.CreateInstance<T>() : createFunc();
                CountAll++;
            }
            else
            {
                t = m_Stack.Pop();
            }

            return t;
        }

        public void Release(T element)
        {
            if(this.m_Stack.Count > 0 && object.ReferenceEquals(this.m_Stack.Peek(), element))
            {
                ASeKi.debug.PrintSystem.LogError("Internal error. Trying to destroy object that is already released to pool.");
            }
            this.m_Stack.Push(element);
        }

        public void Clear()
        {
            this.m_Stack.Clear();
            CountAll = 0;
        }

        public int CountAll
        {
            get;
            private set;
        }

        public int CountInactive
        {
            get
            {
                return this.m_Stack.Count;
            }
        }
    }
}
