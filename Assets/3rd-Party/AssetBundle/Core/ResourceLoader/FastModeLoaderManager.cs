#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;
using ASeKi.AssetBundleCore;
using ASeKi.debug;

namespace act.AssetBundleCore
{
    public class SyncAssetProxy_Fast<T> : SyncAssetProxy<T> where T : Object
    {
        public override void Addreference()
        {
            reference++;
        }
        
        public override void DefReference()
        {
            reference--;
            if (reference == 0)
            {
                Release();
            }
        }

        public void SetInfoData(ref AssetInfoSimplify assetInfo)
        {
            reference = 1;
            this.assetInfo = assetInfo;
        }

        public override void Begin()
        {
            resultObj = AssetDatabase.LoadAssetAtPath<T>(assetInfo.asset);
        }
        
        public override void Release()
        {
            OnDestroy?.Invoke(this);
        }
    }
    
    /// <summary>
    /// 快速处置资源主要就是Hash和资源信息的字典对应
    /// 从从ABDepInfo_DT读取信息存储
    /// </summary>
    public class FastModeLoaderManager : ResourceLoaderManager
    {
        Dictionary<int, AssetInfoSimplify> s_assetInfos = new Dictionary<int, AssetInfoSimplify>();
        HashSet<int> unloadList;

        public override bool ExistAsset(int hash)
        {
            return s_assetInfos.ContainsKey(hash);
        }

        public override void Init(MonoBehaviour monoObj)
        {
            PrintSystem.Log("[Asset Load][FastModeLoaderManager] 初始化 ------ 开始");
            base.Init(monoObj);
            bInitOK = true;
            var soPath = AssetbundlePathSetting.GetFastModeDepTreeInfoFolderPath();

            List<AssetInfo_E> infos = new List<AssetInfo_E>();
            
            PrintSystem.Log("[Asset Load][FastModeLoaderManager] 初始化 从ABDepInfo_DT读取AssetBundleDependecesSO_E 和 AssetInfo_E");
            var files = Directory.GetFiles(soPath);
            foreach (var file in files)
            {
                var text = File.ReadAllText(file);
                AssetBundleDependecesSO_E soE = JsonUtility.FromJson<AssetBundleDependecesSO_E>(text);
                infos.AddRange(soE.assetInfos);
            }
            
            PrintSystem.Log("[Asset Load][FastModeLoaderManager] 初始化 构建 hash 和 资源资料AssetInfoSimplify 的字典");    
            foreach (var info in infos)
            {
                AssetInfoSimplify assetInfoSimplify;
                assetInfoSimplify.hash = info.hash;
                assetInfoSimplify.ownerBundleHash = info.ownerBundleHash;
                assetInfoSimplify.asset = info.assetPath;
                    
                s_assetInfos[info.hash] = assetInfoSimplify;
            }

            unloadList = new HashSet<int>();
            PrintSystem.Log("[Asset Load][FastModeLoaderManager] 初始化 ------ 结束");
        }

        void OnDesotrySyncAsset(ISyncProxy proxy)
        {
            syncProxys.Remove(proxy.GetHash());
        }
        
        // 拿着本地记录的哈希去资源中找，直接通过fast方式进行读取
        public override T LoadAsset<T>(int hash)
        {
            AssetInfoSimplify assetInfo;
            if (s_assetInfos.TryGetValue(hash, out assetInfo))
            {
                if (syncProxys.TryGetValue(hash, out ISyncProxy syncloader))
                {
                    PrintSystem.LogWarning($"[Asset Load][FastModeLoaderManager] 加载了 {hash} path:{assetInfo.asset} 多次，注意需要对应卸载多次");
                    syncloader.Addreference();
                        
                    var cachedLoader = syncloader as SyncAssetProxy_Fast<T>;
                    if(cachedLoader == null)
                    {
                        PrintSystem.LogError($"加载的类型出错 {hash} path:{assetInfo.asset}，存在该类型被加载为其他类型，请检查加载");
                        return default(T);
                    }
                        
                    return cachedLoader.GetAsset();
                }
                else
                {
                    SyncAssetProxy_Fast<T> proxyFast = new SyncAssetProxy_Fast<T>();
                    proxyFast.SetInfoData(ref assetInfo);
                    proxyFast.Begin();
                    proxyFast.OnDestroy = OnDesotrySyncAsset;
                    T reault = proxyFast.GetAsset();
                    syncProxys.Add(hash, proxyFast);

                    return reault;
                }
            }
            else
            {
                PrintSystem.LogError($"[Asset Load][FastModeLoaderManager] LoadAsset Can't' FindAssets {hash}");
            }

            return default;
        }

    }
}
#endif
