using System.Collections.Generic;
using System;

namespace ASeKi.AssetBundleCore
{
    [Serializable]
    public class AssetInfo
    {
        public string assetPath;
        public int hash;
        public int ownerBundleHash;
#if DEBUG_ASSETBUNDLE
            public List<int> dependenceAssetsHash;
            public List<string> dependenceAssets;
#endif
        public bool bRootAsset;
        public List<int> dependenceABsHash;
    }

    [Serializable]
    public class ABInfo
    {
        public string abName;
        public int hash;
        public int assetCount;
        public string ownerFileName;
        public ulong offset;
        public List<int> abAssets;
        public string assetFileHash;
        public bool hasManyRootAsset;
    }

    public class AssetBundleDependecesSO
    {
        public List<ABInfo> abInfos = new List<ABInfo>();
        public List<AssetInfo> assetInfos = new List<AssetInfo>();

    }
}
