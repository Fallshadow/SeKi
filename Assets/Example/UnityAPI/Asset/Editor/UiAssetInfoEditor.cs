using UnityEngine;
using UnityEditor;
using ASeKi.data;
using System.IO;

class UiAssetInfoEditor
{
    [MenuItem("GameEditor/UI/RefreshUiAssetInfo")]
    public static void RefreshAssetInfo()
    {
        UiAssetInfoSO uiAsset = UiAssetInfoSO.Instance;
        uiAsset.UiAssetInfos.Clear();
        string uiFolderPath = $"{Application.dataPath}/ResourceAB/UI/GameLogic";
        if(Directory.Exists(uiFolderPath))
        {

        }
    }
}

