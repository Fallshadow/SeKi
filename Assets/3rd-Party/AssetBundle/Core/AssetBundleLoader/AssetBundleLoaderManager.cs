using System.Collections.Generic;

namespace act.AssetBundleCore
{
    public interface IAsyncAssetHandler
    {
        bool IsCompleted();
        void Begin();
    }
    
    public class AssetBundleLoaderManager : ResourceLoaderManager
    {            
        // 存储资产依赖的ab包列表
        public Dictionary<int, List<AssetDependenceInfo>> assetDependInfos = new Dictionary<int, List<AssetDependenceInfo>>();
        public Dictionary<int, List<int>> rootABAssets = new Dictionary<int, List<int>>();
        public  Dictionary<int, IAsyncAssetProxy> asyncAssetProxys = new Dictionary<int, IAsyncAssetProxy>(DEFAULT_ASSET_LOADER_COUNT);
        
        ASeKi.Utility.ObjectPool<AssetBundleProxy> abProxyPool = new ASeKi.Utility.ObjectPool<AssetBundleProxy>(200);
        
        public AssetBundleProxy NewABProxy(int abHash)
        {
            var proxy = abProxyPool.Get();
            abProxys.Add(abHash, proxy);
            return proxy;
        }

        public override bool ExistAsset(int hash)
        {
            return true;
        }
        
        public void DestroyProxy(AssetBundleProxy proxy)
        {
            var abHash = proxy.abHash;
            DestroyAssetProxy(abHash);
            abProxys.Remove(abHash);
            abProxyPool.Release(proxy);
        }
        
        // RootAsset的AB被移除时，所有对应的主资源proxy需要被移除
        void DestroyAssetProxy(int abHash)
        {
#if DEBUG_ASSETBUNDLE
                act.debug.PrintSystem.Log($"DestroyAssetProxy ab {abHash}", PrintSystem.PrintBy.YinFuNing);
#endif
            List<int> abAssets;
            if (!rootABAssets.TryGetValue(abHash, out abAssets))
            {
                return;
            }

            foreach (var asset in abAssets)
            {
                IAsyncAssetProxy asyncProxy;
                    
                if (asyncAssetProxys.TryGetValue(asset, out asyncProxy))
                {
                    asyncProxy.Destroy();
                }

                ISyncProxy syncProxy;

                if (syncProxys.TryGetValue(asset, out syncProxy))
                {
                    syncProxy.Destroy();
                }
            }
        }

    }
}
