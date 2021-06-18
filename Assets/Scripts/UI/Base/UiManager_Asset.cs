using UnityEngine;
using System;
using System.Collections.Generic;
using act.UIRes;

namespace ASeKi.ui
{
    // UI管理器的动态加载部分,日后填充
    public partial class UiManager : SingletonMonoBehavior<UiManager>
    {
        public Sprite DefaultSprite { get; private set; }
        private List<int> keepHashcode = new List<int>();   // 不释放的资源，用于切场景不卸载的资源。
        private List<int> assetHashCodes = new List<int>(); // 通过GetUiAsset()加载的资源hashcode

        private void unloadAsset(act.UIRes.UiAssetIndex uiAssetIndex)
        {
            int hashCode = act.UIRes.UIResMgr.instance.GetUiAssetInfoViaIndex(uiAssetIndex);
            if (keepHashcode.Contains(hashCode))
            {
                return;
            }
            AssetBundleCore.ResourceLoaderProxy.instance.Unload(hashCode);
        }

        private void unloadAllAssets()
        {
            foreach (int hashCode in assetHashCodes)
            {
                if (keepHashcode.Contains(hashCode))
                {
                    continue;
                }
                AssetBundleCore.ResourceLoaderProxy.instance.Unload(hashCode);
            }
            assetHashCodes.Clear();
            assetHashCodes.AddRange(keepHashcode);
        }

        /// <summary>
        /// 调用此接口加载的资源，框架没有进行管理，需要调用下面UnloadAsset自行卸载资源；如果没有卸载，会在切场景的时候由unloadAllAssets统一卸载。
        /// </summary>
        /// <param name="assetNm"></param>
        /// <returns></returns>
        public T GetUiAsset<T>(UiAssetIndex index) where T : UnityEngine.Object
        {
            int hashCode = UIResMgr.instance.GetUiAssetInfoViaIndex(index);
            if (hashCode == -1)
            {
                debug.PrintSystem.LogError($"[UiManager] Load AB failed. Asset Index:{index}; Can`t find hashInfo!");
                return null;
            }

            T asset = AssetBundleCore.ResourceLoaderProxy.instance.LoadAsset<T>(hashCode);
            if (asset == null)
            {
                debug.PrintSystem.LogError($"[UiManager] Load AB failed. Asset Index:{index}; Asset HashCode: {hashCode}; Can`t find assetbundle!");
                return null;
            }

            if (!assetHashCodes.Contains(hashCode))
            {
                assetHashCodes.Add(hashCode);
            }
            return asset;
        }
        
        /// <summary>
        /// 与“GetUiAsset（）”配套使用
        /// </summary>
        /// <param name="assetName"></param>
        public void UnloadAsset(UiAssetIndex index, UiAssetType type)
        {
            int hashCode = UIResMgr.instance.GetUiAssetInfoViaIndex(index);
            if (type != UiAssetType.UAT_ATLAS && !assetHashCodes.Contains(hashCode)) //　图集特殊处理，因为图集有些是直接引用到的
            {
                return;
            }

            if (hashCode != -1)
            {
                assetHashCodes.Remove(hashCode);
                AssetBundleCore.ResourceLoaderProxy.instance.Unload(hashCode);
                debug.PrintSystem.Log($"[UiManager] UnloadAsset Index:{index}; type: {type.ToString()}", Color.green);
            }
        }
        
        private void getUiViaType<T>(Type type, out UiBase ui) where T : UiBase
        {
            debug.PrintSystem.Log($"[UI Open] 获取UI资源中......");
            Type attrType = typeof(BindingResourceAttribute);
            BindingResourceAttribute attr = Attribute.GetCustomAttribute(type, attrType) as BindingResourceAttribute;
            int hashCode = UIResMgr.instance.GetUiAssetInfoViaIndex(attr.AssetId);
            if (hashCode == -1)
            {
                ui = null;
                debug.PrintSystem.LogError($"[UI Open] Load AB failed. Asset:{attr.AssetId}---- Can`t find hashCode!");
                return;
            }
            debug.PrintSystem.Log($"[UI Open] 成功获取Hash ---- Asset:{attr.AssetId}  HashCode:{hashCode}");
            GameObject prefab = AssetBundleCore.ResourceLoaderProxy.instance.LoadAsset<GameObject>(hashCode);
            if (prefab == null)
            {
                debug.PrintSystem.LogError($"[UI Open] Load AB failed. Asset HashCode: {hashCode}");
                ui = null;
                return;
            }
            debug.PrintSystem.Log($"[UI Open] 成功加载预制体 ---- Asset:{attr.AssetId}  HashCode:{hashCode}");
            ui = prefab.GetComponent<T>();
            if (ui.IsDontDestroy)
            {
                if (!keepHashcode.Contains(hashCode))
                {
                    debug.PrintSystem.Log("[UI Open] 加入不销毁UI组");
                    keepHashcode.Add(hashCode);
                }
            }
        }
    }
}