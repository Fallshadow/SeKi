using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SekiInfiniteScroll<TData, TItem> where TItem : Component
{
    public float Spacing = 10;

    private readonly RectTransform view = null;
    private readonly RectTransform content = null;
    private readonly TItem prefab = null;
    private readonly Action<TItem> onItemCreate = null;
    private readonly Action<TData, TItem> onItemUpdate = null;
    private List<TItem> itemList = new List<TItem>();

    public SekiInfiniteScroll(RectTransform view, RectTransform content, TItem prefab, Action<TItem> onItemCreate, Action<TData, TItem> onItemUpdate)
    {
        this.prefab = prefab;
        this.view = view;
        this.content = content;
        this.onItemCreate = onItemCreate;
        this.onItemUpdate = onItemUpdate;
        prefab.SetActive(false);
    }

    private void init()
    {
        prefab.SetActive(false);
    }

    public void UpdateData(IList<TData> datas)
    {
        init();
        int maxItemNum = caleMaxShowNum(view.sizeDelta.y, (prefab.transform as RectTransform).sizeDelta.y);
        for(int i = 0; i < maxItemNum; ++i)
        {
            TItem item = UnityEngine.Object.Instantiate(prefab, content.transform);
            onItemCreate?.Invoke(item);
            item.SetActive(true);
            onItemUpdate?.Invoke(datas[i], item);
            item.transform.localPosition = new Vector3(0,-(i * Spacing + (item.transform as RectTransform).sizeDelta.y * i),0);
            itemList.Add(item);
        }
        content.sizeDelta = new Vector2(content.sizeDelta.x, datas.Count * Spacing + (prefab.transform as RectTransform).sizeDelta.y * datas.Count);
    }

    private int caleMaxShowNum(float viewHeight, float uiitemHeight)
    {
        return Mathf.CeilToInt(viewHeight / uiitemHeight) + 1;
    }

    private void MoveItemDown()
    {

    }
}
