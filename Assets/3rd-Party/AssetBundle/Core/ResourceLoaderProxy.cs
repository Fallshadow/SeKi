using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using act.AssetBundleCore;

namespace ASeKi.AssetBundleCore
{
    // 资源加载的代理人，决定游戏中资源的加载方式和接口
    public class ResourceLoaderProxy : Singleton<ResourceLoaderProxy>
    {
        ResourceLoaderManager manager;
#if DEBUG_RESOURCE
        FastModeLoaderManager fastModeManager;
#endif
       

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
        public ResourceLoaderManagerInfo ResourceLoaderManagerInfo = null;
        
        // 根据设置设定游戏中加载资源的方式（也就是平时开发用的直接用HashCode取资源的方式或者测试AB包的方式）
        public void Init(bool bDisbaleAssetBundle)
        {
//#if UNITY_EDITOR
//            if (!Application.isPlaying)
//            {
//                manager = new EditorLoaderManager();
//                manager.Init(null);
//                return;
//            }
//#endif
            if (ResourceLoaderManagerInfo != null)
            {
                Debug.LogError("[Asset Load][ResourceLoaderProxy] Init muliply times");
                return;
            }
            
            this.bDisbaleAssetBundle = bDisbaleAssetBundle;
            ResourceLoaderManagerInfo = new GameObject("ResourceLoaderManager", typeof(ResourceLoaderManagerInfo)).GetComponent<ResourceLoaderManagerInfo>();
            ResourceLoaderManagerInfo.bDisbaleAssetbundle = bDisbaleAssetBundle;
            GameObject obj = new GameObject("ResourceLoaderManager");

            if (!bDisbaleAssetBundle)
            {
                manager = new AssetBundleLoaderManager();
#if UNITY_EDITOR && DEBUG_RESOURCE
                fastModeManager = new FastModeLoaderManager();
                fastModeManager.Init(ResourceLoaderManagerInfo);
#endif
            }
            else
            {
#if UNITY_EDITOR
                manager = new FastModeLoaderManager();
#endif
            }

            if (manager != null)
            {
                manager.Init(ResourceLoaderManagerInfo);
            }

            Object.DontDestroyOnLoad(ResourceLoaderManagerInfo.gameObject);
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

        // 通过hashcode同步载入ab
        public T LoadAsset<T>(int hashCode) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return manager.LoadAsset<T>(hashCode);
            }
#endif
#if UNITY_EDITOR && DEBUG_RESOURCE
            if (!bDisbaleAssetBundle && fastModeManager.ExistAsset(hashCode))
            {
                debug.PrintSystem.LogWarning(string.Format("[Asset Load] AssetBunlde can't found {0} LoadAsset by FastMode", hashCode));
                return fastModeManager.LoadAsset<T>(hashCode);
            }
#endif
            return manager.LoadAsset<T>(hashCode);
        }

        // 卸载调用Load加载的资源，即同步加载的资源
        public void Unload(int hashCode, bool bDebug = true)
        {
#if UNITY_EDITOR && DEBUG_RESOURCE
            if (!bDisbaleAssetBundle && !manager.ExistAsset(hashCode))
            {
                debug.PrintSystem.LogWarning(string.Format("[Asset Load] AssetBunlde can't found {0} UnloadInSync by FastMode", hashCode));
                fastModeManager.Unload(hashCode, bDebug);
                return;
            }
#endif
            manager.Unload(hashCode, bDebug);
        }
        
    }
}
