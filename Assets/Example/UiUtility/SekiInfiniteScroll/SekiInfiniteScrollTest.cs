using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SekiInfiniteScrollTest : MonoBehaviour
{
    [SerializeField] private int uiItemCount = 100;
    [SerializeField] private RectTransform view = null;
    [SerializeField] private RectTransform content = null;
    [SerializeField] private WeaponUiItem weaponPrefab = null;
    private SekiInfiniteScroll<WeaponData, WeaponUiItem> sekiInfiniteScroll = null;
    private List<WeaponData> weaponDatas = new List<WeaponData>();

    private void Awake()
    {
        Init();
    }

    private void OnDestroy()
    {
        Release();
    }

    public void Init()
    {
        prepareDatas();
        sekiInfiniteScroll = new SekiInfiniteScroll<WeaponData, WeaponUiItem>(view, content, weaponPrefab, weaponCreatCall, weaponUpdateCall);
    }

    public void Release()
    {

    }

    public void Retest()
    {
        Init();
    }

    public void InputDatas()
    {
        sekiInfiniteScroll.UpdateData(weaponDatas);
    }

    private void prepareDatas()
    {
        weaponDatas.Clear();
        for(int index = 0; index < uiItemCount; index++)
        {
            WeaponData data = new WeaponData();
            data.name = "逆刃刀";
            weaponDatas.Add(data);
        }
    }

    private void weaponCreatCall(WeaponUiItem uiItem)
    {
        // 可以设置一些回调
    }

    private void weaponUpdateCall(WeaponData data, WeaponUiItem uiItem)
    {
        uiItem.UpdateData(data);
    }
}
