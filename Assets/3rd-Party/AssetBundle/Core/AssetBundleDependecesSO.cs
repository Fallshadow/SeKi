using System.Collections.Generic;
using System;

namespace act.AssetBundleCore
{
    [System.Serializable]
    public class AssetInfo
    {
        public string addressName;
        public int hash;
        public int ownerBundleHash;
        public bool bRootAsset;
        public List<int> dependenceABsHash;
    }

    [System.Serializable]
    public class ABInfo
    {
        public string abName;
        public int hash;
        public int assetCount;
        public string ownerFileName;
        public ulong offset;
        public List<int> abAssets;
        public string abFileHash;
        public bool hasManyRootAsset;
    }

    public class AssetBundleDependecesSO
    {
        public List<ABInfo> abInfos = new List<ABInfo>();
        public List<AssetInfo> assetInfos = new List<AssetInfo>();

    }

#if UNITY_EDITOR
    [System.Serializable]
    public class AssetInfo_E
    {
        public string assetPath;

        public string addressName;

        // public string assetName;
        public int hash;
        public int ownerBundleHash;
#if DEBUG_ASSETBUNDLE
            public List<int> dependenceAssetsHash;
            public List<string> dependenceAssets;
#endif
        public bool bRootAsset;
        public List<int> dependenceABsHash;

        public AssetInfo GetRuntime()
        {
            AssetInfo assetInfo = new AssetInfo();
            assetInfo.addressName = addressName;
            assetInfo.hash = hash;
            assetInfo.ownerBundleHash = ownerBundleHash;
            assetInfo.bRootAsset = bRootAsset;

            var deps = new List<int>(dependenceABsHash.Count);
            foreach (var dep in dependenceABsHash)
            {
                deps.Add(dep);
            }

            assetInfo.dependenceABsHash = deps;
            return assetInfo;
        }
    }

    [System.Serializable]
    public class ABInfo_E
    {
        public string abName;
        public int hash;
        public int assetCount;
        public string ownerFileName;
        public ulong offset;
        public List<int> abAssets;
        public string abFileHash;
        public bool hasManyRootAsset;

        public ABInfo GetRunTime()
        {
            ABInfo abInfo = new ABInfo();
            abInfo.abName = abName;
            abInfo.hash = hash;
            abInfo.assetCount = assetCount;
            abInfo.ownerFileName = ownerFileName;

            abInfo.offset = offset;
            abInfo.abAssets = new List<int>(abAssets.Count);
            foreach (var abAsset in abAssets)
            {
                abInfo.abAssets.Add(abAsset);
            }

            abInfo.abFileHash = abFileHash;
            abInfo.hasManyRootAsset = hasManyRootAsset;
            return abInfo;
        }
    }

    public class AssetBundleDependecesSO_E
    {
        public List<ABInfo_E> abInfos = new List<ABInfo_E>();
        public List<AssetInfo_E> assetInfos = new List<AssetInfo_E>();

        public AssetBundleDependecesSO GetRuntime()
        {
            var so = new AssetBundleDependecesSO();
            so.abInfos = new List<ABInfo>(abInfos.Count);
            so.assetInfos = new List<AssetInfo>(assetInfos.Count);

            foreach (var abInfo in abInfos)
            {
                so.abInfos.Add(abInfo.GetRunTime());
            }

            foreach (var assetInfo in assetInfos)
            {
                so.assetInfos.Add(assetInfo.GetRuntime());
            }

            return so;
        }
    }
#endif
}