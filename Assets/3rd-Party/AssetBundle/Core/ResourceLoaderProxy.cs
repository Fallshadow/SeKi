using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.AssetBundleCore
{
    // 资源加载的代理人，决定游戏中资源的加载方式和接口
    public class ResourceLoaderProxy : Singleton<ResourceLoaderProxy>
    {
        ResourceLoaderManager manager;
        FastModeLoaderManager fastModeManager;

        public bool IsOK
        {
            get
            {
                if(manager == null)
                {
                    return false;
                }

                return manager.bInitOK;
            }
        }

        #region Init

        bool bDisbaleAssetBundle;       // 是否开启ab

        // 根据设置设定游戏中加载资源的方式（也就是平时开发用的直接用HashCode取资源的方式或者测试AB包的方式）
        public void SettingAB(bool bDisbaleAssetBundle)
        {
            this.bDisbaleAssetBundle = bDisbaleAssetBundle;

            GameObject obj = new GameObject("ResourceLoaderManager");

            if(!bDisbaleAssetBundle)
            {
                // 打包/测试AB包的时候走这边
                manager = obj.AddComponent<AssetBundleLoaderManager>();
            }
            else
            {
                manager = obj.AddComponent<FastModeLoaderManager>();
            }

            Object.DontDestroyOnLoad(obj);
#if UNITY_EDITOR
            // 此处还不解，是因为默认资源的AB包防止重复占用么？
            manager.StartCoroutine(KeepShaderAB(obj.transform));
#endif
        }

        IEnumerator KeepShaderAB(Transform tran)
        {
            while(!IsOK)
            {
                yield return null;
            }

            var prefab = LoadAsset<GameObject>(283900967);
            var obj = GameObject.Instantiate(prefab, tran);
        }

        #endregion

        // 通过hashcode异步载入ab
        public T LoadAsset<T>(int hashCode) where T : UnityEngine.Object
        {
#if UNITY_EDITOR && DEBUG_RESOURCE
            if(!bDisbaleAssetBundle && !manager.ExistAsset(hashCode))
            {
                ASeKi.debug.PrintSystem.LogWarning(string.Format("AssetBunlde can't found {0} LoadAsset by FastMode", hashCode));

                return fastModeManager.LoadAsset<T>(hashCode);
            }
#endif
            return manager.LoadAsset<T>(hashCode);

        }

    }
}
