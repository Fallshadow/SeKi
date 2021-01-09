using UnityEngine;

namespace ASeKi.AssetBundleCore
{
    public class AssetbundlePathSetting
    {
        public static string GetFastModeDepTreeSOBasePath()
        {
            return $"{Application.dataPath}/../ABDepInfo_DT/";
        }
    }
}
