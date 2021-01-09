using System.Collections.Generic;

namespace ASeKi.AssetBundleCore
{
    public class AssetBundleLoaderManager : ResourceLoaderManager
    {            
        //存储资产依赖的ab包列表
        public Dictionary<int, List<AssetDependenceInfo>> assetDependInfos = new Dictionary<int, List<AssetDependenceInfo>>();

        Utility.ObjectPool<AssetBundleProxy> abProxyPool = new Utility.ObjectPool<AssetBundleProxy>(200);

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
    }
}
