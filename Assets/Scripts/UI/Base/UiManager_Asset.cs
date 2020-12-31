using UnityEngine.EventSystems;
using UnityEngine;

namespace ASeKi.ui
{
    // UI管理器的动态加载部分,日后填充
    public partial class UiManager : SingletonMonoBehavior<UiManager>
    {
        public Sprite DefaultSprite { get; private set; }


        // 调用此接口加载的资源，框架进行没有管理，需要调用下面UnloadAsset自行卸载资源；如果没有卸载，会在切场景的时候由unloadAllAssets统一卸载。
        public T GetUiAsset<T>(string assetNm, data.UiAssetType assetType = data.UiAssetType.UAT_PREFAB) where T : UnityEngine.Object
        {
            int hashCode = data.UiAssetInfoSO.Instance.GetUiAssetInfo(assetType, assetNm);
            if(hashCode != -1)
            {
                //T asset = act.AssetBundleCore.ResourceLoaderProxy.instance.LoadAsset<T>(hashCode);
                //if(asset == null)
                //{
                //    debug.PrintSystem.LogError($"[UiManager] Load AB failed. Asset name:{assetNm}; Asset HashCode: {hashCode}; Can`t find assetbundle!");
                //    return null;
                //}
                //if(!assetHashCodes.Contains(hashCode))
                //{
                //    assetHashCodes.Add(hashCode);
                //}
                //return asset;               
                return null;
            }
            else
            {
                debug.PrintSystem.LogError($"[UiManager] Load AB failed. Asset name:{assetNm}; Can`t find hashInfo!");
                return null;
            }
        }

        // 提供默认图，以提示资源丢失
        private Sprite getDefaultSprite()
        {
            if(DefaultSprite == null)
            {
                Texture2D tex = GetUiAsset<Texture>(constants.ResourcesPathSetting.DefaultSpriteUiTexture, data.UiAssetType.UAT_HIGH_DEFINITION_TEX) as Texture2D;
                DefaultSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
            }

            return DefaultSprite;
        }
    }
}