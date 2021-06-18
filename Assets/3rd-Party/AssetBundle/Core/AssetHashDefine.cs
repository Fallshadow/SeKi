namespace act.AssetBundleCore
{
    public abstract class AssetHashDefine
    {
#if UNITY_STANDALONE_WIN
        // public const int UI_ASSET_HASH_MAP = 1053410750;
        public const int UI_ASSET_HASH_MAP = -2015699102;
#else
        public const int UI_ASSET_HASH_MAP = 505949548; //原始
#endif
    }
}