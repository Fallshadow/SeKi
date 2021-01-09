using System;
using System.Collections.Generic;

namespace ASeKi.AssetBundleCore
{
    public interface ISyncProxy
    {
        int GetHash();
        int GetABHash();
        int GetTypeHash();
        void Addreference();
        void DefReference();
        void Destroy();
        void ZeroReference();
    }

    public class SyncAssetProxy<T> : ISyncProxy where T : UnityEngine.Object  //主资源
    {
        protected AssetInfoSimplify assetInfo;
        public int AssetHash => assetInfo.hash;

        protected T resultObj;
        
        AssetBundleLoaderManager manager;

        int typeHash;
        protected int reference;
        public Action<ISyncProxy> OnDestroy;

        public virtual void Init(AssetBundleLoaderManager manager, ref AssetInfoSimplify assetInfo, int typeHash)
        {
            resetData();
            this.assetInfo = assetInfo;
            this.manager = manager;
            this.typeHash = typeHash;
        }

        void resetData()
        {
            resultObj = null;
            reference = 1;
            OnDestroy = null;
            manager = null;
        }

        public virtual void Addreference()
        {
            // 卸载后，AB没被卸载，但又重新引用它,重新加回自己包的引用
            if(reference == 0)
            {
                ASeKi.debug.PrintSystem.Log($"Add reference asset {AssetHash} ab {assetInfo.ownerBundleHash}", debug.PrintSystem.PrintBy.sunshuchao);
                var myProxy = getDepProxy(assetInfo.ownerBundleHash);
                myProxy.AddAssetsReference();
            }

            reference++;
        }

        AssetBundleProxy getDepProxy(int abHash)
        {
            return manager.GetABProxy(abHash);
        }

        public void DefReference()
        {
            throw new NotImplementedException();
        }

        public void Destroy()
        {
            throw new NotImplementedException();
        }

        public int GetABHash()
        {
            throw new NotImplementedException();
        }

        public T GetAsset()
        {
            return resultObj;
        }

        public int GetHash()
        {
            throw new NotImplementedException();
        }

        public int GetTypeHash()
        {
            throw new NotImplementedException();
        }

        public void ZeroReference()
        {
            throw new NotImplementedException();
        }

        public virtual void Begin()
        {
            List<AssetDependenceInfo> depABs;

            if(!manager.assetDependInfos.TryGetValue(assetInfo.hash, out depABs))
            {
                debug.PrintSystem.Log($"Current AssetInfo{assetInfo.asset} don't in the assetDepend List!");
                return;
            }

            var abProxys = manager.abProxys;

            foreach(var dep in depABs)
            {
                AssetBundleProxy proxy;

                if(!abProxys.TryGetValue(dep.abHash, out proxy))
                {
                    int abHash = dep.abHash;

                    AssetBundleProxy newAbProxy = manager.NewABProxy(abHash);
                    newAbProxy.Init(manager, abHash, dep.ab, dep.offset);
                    newAbProxy.LoadSync();
                }
                else
                {
                    //                    if (proxy.IsLoading)
                    //                    {
                    //                        throw new Exception(string.Format($"Async hash {assetInfo.hash} Proxy is Processing it:{dep.ab}, offset:{dep.offset}"));
                    //                    }
                }
            }

            loadAsset();
        }


        void loadAsset()
        {
            //所有包(Proxy)都加上对于Asset的reference
            var abProxys = manager.abProxys;
            var depABs = manager.assetDependInfos[assetInfo.hash];
            foreach(var dep in depABs)
            {
                var proxy = abProxys[dep.abHash];
                if(proxy != null)
                {
                    proxy.AddAssetsReference();
                }
            }

            var myProxy = getDepProxy(assetInfo.ownerBundleHash);
            resultObj = myProxy.LoadAsset<T>(assetInfo.asset);
        }
    }
}
