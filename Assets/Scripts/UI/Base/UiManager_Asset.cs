using UnityEngine;
using System;
using System.Collections.Generic;

namespace ASeKi.ui
{
    // UI管理器的动态加载部分,日后填充
    public partial class UiManager : SingletonMonoBehavior<UiManager>
    {
        public Sprite DefaultSprite { get; private set; }
        private List<int> keepHashcode = new List<int>();   // 不释放的资源，用于切场景不卸载的资源。

        // 调用此接口加载的资源，框架进行没有管理，需要调用下面UnloadAsset自行卸载资源；如果没有卸载，会在切场景的时候由unloadAllAssets统一卸载。
        public T GetUiAsset<T>(string assetNm, data.UiAssetType assetType = data.UiAssetType.UAT_PREFAB) where T : UnityEngine.Object
        {
            int hashCode = data.UiAssetInfoSO.Instance.GetUiAssetHashCode(assetType, assetNm);
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

        // 卸载资源
        private void unloadAsset(UiBase ui)
        {
            Type uiType = ui.GetType();
            Type attrType = typeof(BindingResourceAttribute);
            BindingResourceAttribute attr = Attribute.GetCustomAttribute(uiType, attrType) as BindingResourceAttribute;
            int hashCode = ASeKi.data.UiAssetInfoSO.Instance.GetUiAssetHashCode(ASeKi.data.UiAssetType.UAT_PREFAB, attr.AssetId);
            if(keepHashcode.Contains(hashCode))
            {
                return;
            }
            // AssetBundleCore.ResourceLoaderProxy.instance.UnloadInSync(hashCode);
        }

        private void unloadAllAssets()
        {
            //foreach(int hashCode in assetHashCodes)
            //{
            //    if(keepHashcode.Contains(hashCode))
            //    {
            //        continue;
            //    }
            //    act.AssetBundleCore.ResourceLoaderProxy.instance.UnloadInSync(hashCode);
            //}
            //assetHashCodes.Clear();
            //assetHashCodes.AddRange(keepHashcode);
        }

        private void getUiAsset<T>(Type type, out UiBase ui) where T : UiBase
        {
            Type attrType = typeof(BindingResourceAttribute);
            BindingResourceAttribute attr = Attribute.GetCustomAttribute(type, attrType) as BindingResourceAttribute;
            int hashCode = ASeKi.data.UiAssetInfoSO.Instance.GetUiAssetHashCode(ASeKi.data.UiAssetType.UAT_PREFAB, attr.AssetId);
            if(hashCode != -1)
            {
                debug.PrintSystem.Log($"[UiManager_Asset] 加载资源{hashCode}", debug.PrintSystem.PrintBy.sunshuchao);
                ui = null;
                GameObject prefab = ASeKi.AssetBundleCore.ResourceLoaderProxy.instance.LoadAsset<GameObject>(hashCode);
                if(prefab == null)
                {
                    debug.PrintSystem.LogError($"[UiManager] Load AB failed. Asset HashCode: {hashCode}",debug.PrintSystem.PrintBy.sunshuchao);
                    return;
                }
                ui = prefab.GetComponent<T>();
                if(ui.IsDontDestroy)
                {
                    if(!keepHashcode.Contains(hashCode))
                    {
                        keepHashcode.Add(hashCode);
                    }
                }
            }
            else
            {
                ui = null;
                debug.PrintSystem.LogError($"[UiManager] Load AB failed. Asset:{attr.AssetId}---- Can`t find hashCode!",debug.PrintSystem.PrintBy.sunshuchao);
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