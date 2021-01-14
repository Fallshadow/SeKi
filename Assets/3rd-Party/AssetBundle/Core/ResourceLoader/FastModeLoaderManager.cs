using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEngine;

namespace ASeKi.AssetBundleCore
{
    public class SyncAssetProxy_Fast<T> : SyncAssetProxy<T> where T : UnityEngine.Object
    {
        public override void Addreference()
        {
            reference++;
        }

        public void SetInfoData(ref AssetInfoSimplify assetInfo)
        {
            this.assetInfo = assetInfo;
        }

        public override void Begin()
        {
            resultObj = AssetDatabase.LoadAssetAtPath<T>(assetInfo.asset);
        }
    }

    public class FastModeLoaderManager : ResourceLoaderManager
    {
        Dictionary<int, AssetInfoSimplify> s_assetInfos = new Dictionary<int, AssetInfoSimplify>();
        HashSet<int> unloadList;

        public override bool ExistAsset(int hash)
        {
            return s_assetInfos.ContainsKey(hash);
        }

        private void Awake()
        {
            bInitOK = true;
            string soPath = AssetbundlePathSetting.GetFastModeDepTreeSOBasePath();
            List<AssetInfo> infos = new List<AssetInfo>();
            string[] files = Directory.GetFiles(soPath);
            foreach(var file in files)
            {
                var text = File.ReadAllText(file);
                AssetBundleDependecesSO so = JsonUtility.FromJson<AssetBundleDependecesSO>(text);
                infos.AddRange(so.assetInfos);
            }

            foreach(var info in infos)
            {
                AssetInfoSimplify assetInfoSimplify;
                assetInfoSimplify.hash = info.hash;
                assetInfoSimplify.ownerBundleHash = info.ownerBundleHash;
                assetInfoSimplify.asset = info.assetPath;

                s_assetInfos[info.hash] = assetInfoSimplify;
            }

            unloadList = new HashSet<int>();
        }

        // 拿着本地记录的哈希去资源中找，直接通过fast方式进行读取
        public override T LoadAsset<T>(int hash)
        {
            AssetInfoSimplify assetInfo;
            if(s_assetInfos.TryGetValue(hash, out assetInfo))
            {
                ISyncProxy syncloader;
                debug.PrintSystem.Log($"[FastModeLoaderManager] 通过SyncAssetProxy_Fast直接去加载资源{hash}", debug.PrintSystem.PrintBy.sunshuchao);
                if(this.syncProxys.TryGetValue(hash, out syncloader))
                {
                    syncloader.Addreference();
                    return ((SyncAssetProxy_Fast<T>)syncloader).GetAsset();
                }
                else
                {
                    SyncAssetProxy_Fast<T> proxyFast = new SyncAssetProxy_Fast<T>();
                    proxyFast.SetInfoData(ref assetInfo);
                    proxyFast.Begin();

                    T reasult = proxyFast.GetAsset();
                    this.syncProxys.Add(hash, proxyFast);

                    return reasult;
                }
            }

            return default;
        }
    }
}
