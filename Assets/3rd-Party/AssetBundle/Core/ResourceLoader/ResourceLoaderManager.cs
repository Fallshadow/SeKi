using UnityEngine;
using System.Collections.Generic;

namespace act.AssetBundleCore
{
    public struct AssetInfoSimplify
    {
        public string asset;
        public int hash;
        public int ownerBundleHash;
    }

    public struct AssetDependenceInfo
    {
        public int abHash;
        public string ab;
        public ulong offset;
    }

    public abstract class ResourceLoaderManager
    {
        public MonoBehaviour MonoObj => monoObj;
        private MonoBehaviour monoObj; 
        
        const int AB_COUNT = 300;

        public const int DEFAULT_ASSET_LOADER_COUNT = 500;

        public bool bInitOK = false;

        protected Dictionary<int, ISyncProxy> syncProxys = new Dictionary<int, ISyncProxy>(DEFAULT_ASSET_LOADER_COUNT);
        public Dictionary<int, AssetBundleProxy> abProxys = new Dictionary<int, AssetBundleProxy>(AB_COUNT);
#if UNITY_EDITOR
        public string BundlePrePath = "";
#endif
        public virtual void Init(MonoBehaviour monoObj)
        {
            this.monoObj = monoObj;
        }

        public abstract bool ExistAsset(int hash);

        public virtual T LoadAsset<T>(int hash) where T : UnityEngine.Object
        {
            return default(T);
        }

        public AssetBundleProxy GetABProxy(int abHash)
        {
            return abProxys[abHash];
        }
        
        public virtual void Unload(int hash, bool bDebug = true)
        {
                
        }
    }
}
