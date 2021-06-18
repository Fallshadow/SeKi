using UnityEngine;

namespace act.AssetBundleCore
{
    public class AssetbundlePathSetting
    {
        public static string GetFastModeDepTreeSOBasePath()
        {
            return $"{Application.dataPath}/../ABDepInfo_DT/";
        }
        
        public static string GetFastModeDepTreeInfoFolderPath()
        {
            return $"{Application.dataPath}/../ABDepInfo_DT/";
        }
        
        public static string GetFastModeDepTreeInfoPath(string ext)
        {
            return $"{GetFastModeDepTreeInfoFolderPath()}{ext}.json";
        }
    }
}
