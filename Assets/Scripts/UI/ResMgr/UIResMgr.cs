using System.Collections;
using act.AssetBundleCore;
using ASeKi.AssetBundleCore;
using ASeKi.data;

namespace act.UIRes
{
    public class UIResMgr : SingletonMonoBehaviorNoDestroy<UIResMgr>
    {
        private AssetHashMap_UI uiSO;
        public bool IsInitialized { get; private set; }
        
        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(LoadSceneMappingInfo());
        }
        
        private IEnumerator LoadSceneMappingInfo()
        {
            while (!ResourceLoaderProxy.instance.IsOK)
            {
                yield return null;
            }
            InitLoader();
        }
        
        private void InitLoader()
        {
            if (IsInitialized)
            {
                return;
            }

            int assetMapId = AssetHashDefine.UI_ASSET_HASH_MAP;
            uiSO = ResourceLoaderProxy.instance.LoadAsset<AssetHashMap_UI>(assetMapId);
            if (uiSO == null)
            {
                ASeKi.debug.PrintSystem.LogError(" [Asset Load][UIResMgr] Asset Mapping Info Load Failed");
            }

            uiSO.Initialize();
            IsInitialized = true;
        }
        
        // 通过UI枚举获取UI的HashCode
        public int GetUiAssetInfoViaIndex(UIRes.UiAssetIndex index)
        {
            if (!IsInitialized)
            {
                InitLoader();
            }

            foreach (UiAssetInfo info in uiSO.UiAssetInfos)
            {
                if (info.Index == (int)index)
                {
                    return info.HashCode;
                }
            }

            ASeKi.debug.PrintSystem.LogError($"[UI Open] Can`t find UiAssetInfo : {index}");
            return -1;
        }
    }
}