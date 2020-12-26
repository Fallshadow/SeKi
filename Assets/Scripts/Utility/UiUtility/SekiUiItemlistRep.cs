using UnityEngine;
using System.Collections.Generic;
using System;

namespace ASeKi.Ui
{
    /// <summary>
    /// 會將UI物件拓展至最大數量，避免一直新增刪除物件。
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public class SekiUiItemlistRep<TData, TItem> where TItem : Component
    {
        private readonly TItem prefab = null;
        private readonly Transform root = null;
        private readonly Action<TItem> onItemCreate = null;
        private readonly Action<int, TData, TItem> onItemUpdate = null;

        private readonly List<TItem> items = new List<TItem>(4);

        public TItem this[int i] { get { return items[i]; } }

        public int Capcity { get { return items.Count; } }

        public int Count
        {
            get
            {
                int activeCount = 0;
                for(int i = 0, count = items.Count; i < count; ++i)
                {
                    if(items[i].gameObject.activeSelf)
                    {
                        ++activeCount;
                    }
                }
                return activeCount;
            }
        }

        public SekiUiItemlistRep(TItem prefab, Transform root, Action<TItem> onItemCreate, Action<int, TData, TItem> onItemUpdate)
        {
            this.prefab = prefab;
            this.root = root;
            this.onItemCreate = onItemCreate;
            this.onItemUpdate = onItemUpdate;
            prefab.SetActive(false);
        }

        public void Clear()
        {
            for(int i = 0; i < items.Count; ++i)
            {
                UnityEngine.Object.Destroy(items[i].gameObject);
            }
            items.Clear();
        }

        public void UpdateItems(IList<TData> datas)
        {
            int dataCount = datas.Count;
            int itemCount = items.Count;
            for(int i = itemCount; i < dataCount; ++i)
            {
                TItem item = UnityEngine.Object.Instantiate(prefab, root);
                onItemCreate?.Invoke(item);
                items.Add(item);
            }

            for(int i = 0; i < dataCount; ++i)
            {
                onItemUpdate?.Invoke(i, datas[i], items[i]);
                items[i].gameObject.SetActive(true);
            }

            for(int i = dataCount; i < itemCount; ++i)
            {
                items[i].gameObject.SetActive(false);
            }
        }

        public TItem GetItem(Predicate<TItem> predicate)
        {
            List<TItem> activeitems = items.FindAll(i => i.gameObject.activeSelf);
            return activeitems.Find(predicate);
        }

        public int GetItemIndex(Predicate<TItem> predicate)
        {
            List<TItem> activeitems = items.FindAll(i => i.gameObject.activeSelf);
            return activeitems.FindIndex(predicate);
        }
    }
}