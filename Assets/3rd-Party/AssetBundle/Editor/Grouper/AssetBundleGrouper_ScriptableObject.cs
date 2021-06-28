using System;
using System.Collections.Generic;
using System.IO;
using act.Resource;
using UnityEditor;
using UnityEngine;

namespace act.AssetBundleUtility
{
    public class AssetBundleGrouper_ScriptableObject : AssetBundleGrouper
    {
        public override List<AssetGroup> Work(BuildTarget buildTarget)
        {
            this.buildTarget = buildTarget;
            string resPath = $"{Application.dataPath}/{constants.ResourcesPathSetting.RESOURCES_FOLDER}/{constants.ResourcesPathSetting.SO_FOLDER}";
            if (!Directory.Exists(resPath))
            {
                return new List<AssetGroup>();
            }

            FileInfo[] assets = new DirectoryInfo(resPath).GetFiles(constants.ResourcesPathSetting.SO_PATTERN, SearchOption.AllDirectories);
            List<AssetGroup> retGroups = new List<AssetGroup>(assets.Length);
            for (int i = 0; i < assets.Length; ++i)
            {
                retGroups.Add(createAssetGroup(assets[i], false));
            }
            return retGroups;
        }

        public override List<AssetGroup> WorkForEditor()
        {
            string[] resFolders = new string[] { constants.ResourcesPathSetting.RESOURCES_FOLDER, constants.ResourcesPathSetting.RESOURCES_PC_FOLDER, constants.ResourcesPathSetting.RESOURCES_ANDROID_FOLDER, constants.ResourcesPathSetting.RESOURCES_IOS_FOLDER };
            List<AssetGroup> assetGroups = new List<AssetGroup>(256);
            foreach (string folder in resFolders)
            {
                string resPath = $"{Application.dataPath}/{folder}{constants.ResourcesPathSetting.SO_FOLDER}";
                if (!Directory.Exists(resPath))
                {
                    continue;
                }

                FileInfo[] assets = new DirectoryInfo(resPath).GetFiles(constants.ResourcesPathSetting.SO_PATTERN, SearchOption.AllDirectories);
                for (int i = 0; i < assets.Length; ++i)
                {
                    assetGroups.Add(createAssetGroup(assets[i], true));
                }
            }
            return assetGroups;
        }

        private AssetGroup createAssetGroup(FileInfo fileInfo, bool isForEditor)
        {
            AssetItem item = new AssetItem
            {
                assetPath = isForEditor ? GetAssetPath(fileInfo.FullName) : GetAssetPath(fileInfo.FullName, buildTarget),
                importance = DEFAULT_SHADER_IMPORTANCE,
                subLevelNames = null
            };
            
            AssetGroup group = new AssetGroup
            {
                GroupName = $"{System.IO.Path.GetFileNameWithoutExtension(item.assetPath)}_{ResourceUtility.GetHashCodeByAssetPath(item.assetPath).ToString()}" 
            };
            
            group.Assets.Add(item);
            return group;
        }
    }
}