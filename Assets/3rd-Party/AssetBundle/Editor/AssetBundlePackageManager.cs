#if UNITY_EDITOR
using System;
using Debug = UnityEngine.Debug;
using act.AssetBundleCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using act.Resource;
using UnityEditor;
using UnityEngine;

namespace act.AssetBundleUtility
{
        public partial class AssetBundlePackageManager
        {
            
            public static void SerializeAssetDepenceInfoForFastMode(AssetBundleGrouper grouper)
            {
                AssetBundleDependecesSO_E soE = new AssetBundleDependecesSO_E();
                List<AssetInfo_E> infos = new List<AssetInfo_E>();
                grouper.Init();

                List<AssetGroup> assetItems = grouper.WorkForEditor();

                foreach (var group in assetItems)
                {
                    foreach (var asset in group.Assets)
                    {
                        asset.assetPath = asset.assetPath.Replace('\\', '/');

                        AssetInfo_E infoE = new AssetInfo_E();
                        infoE.hash = ResourceUtility.GetHashCodeByAssetPath(asset.assetPath, false);
                        infoE.assetPath = asset.assetPath;
                        infos.Add(infoE);
                    }
                }

                soE.assetInfos = infos;

                var module = grouper.GetType().Name.Substring("AssetBundleGrouper_".Length);

                var savePath = AssetbundlePathSetting.GetFastModeDepTreeInfoPath(module);

                var jsonInfo = JsonUtility.ToJson(soE);

                File.Delete(savePath);
                File.WriteAllText(savePath, jsonInfo);

                Debug.Log($"序列化依赖信息完成:{module}");
            }
        }
}
#endif
