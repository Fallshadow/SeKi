using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.data
{
    // 资源类型枚举
    [System.Serializable]
    public enum UiAssetType
    {
        UAT_NONE = -1,
        UAT_PREFAB = 0,         //预制体
        UAT_ATLAS = 1,          //图集
        UAT_HIGH_DEFINITION_TEX = 2,    //高清图
        UAT_TMP_FONTASSET = 3,  //字体资源
        UAT_MATERIAL = 4,       //材质
        UAT_ANIMATION = 5,      //动画
        UAT_TXT = 6,            //文本资源
    }

    [System.Serializable]
    public class UiAssetInfo
    {
        public UiAssetType AssetType;
        public string AssetName;
        public string AssetPath;
        public int HashCode;
    }

    public class UiAssetInfoSO : ScriptableObject
    {
        public List<UiAssetInfo> UiAssetInfos = new List<UiAssetInfo>();

        public static UiAssetInfoSO Instance 
        {
            get
            {
                if(instance == null)
                {
                    instance = Utility.LoadResources.LoadAsset<UiAssetInfoSO>(constants.ResourcesPathSetting.SO_FOLDER + "UiAssetInfoSO");
                }
                return instance;
            }
        }

        private static UiAssetInfoSO instance;

        public int GetUiAssetHashCode(UiAssetType type, string name)
        {
            for(int i = 0; i < UiAssetInfos.Count; i++)
            {
                if(UiAssetInfos[i].AssetType == type && UiAssetInfos[i].AssetName == name)
                {
                    return UiAssetInfos[i].HashCode;
                }
            }

            return -1;
        }
    }
}
