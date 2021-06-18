using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace act.AssetBundleUtility
{
    public class AssetBundleGrouper_UI : AssetBundleGrouper
    {
        private const int DEFAULT_UI_IMPORTANCE = 4;

        private bool isCheckUnique = false;

        public override List<AssetGroup> Work(BuildTarget buildTarget)
        {
            this.buildTarget = buildTarget;
            isCheckUnique = constants.ResourcesPathSetting.RESOURCES_FOLDER != GetSpecificPlatformFolder(buildTarget); // 如果读取资源来自同一个文件不用检查特殊性。
            List<AssetGroup> retGroups = buildGroup();
            foreach (var item in retGroups)
            {
                foreach (var asset in item.Assets)
                {
                    asset.assetPath = asset.assetPath.Replace('\\', '/');
                }
            }
            return retGroups;
        }

        public override List<AssetGroup> WorkForEditor()
        {
#if UNITY_ANDROID
            buildTarget = BuildTarget.Android;
#elif UNITY_IPHONE
            buildTarget = BuildTarget.iOS;
#elif UNITY_STANDALONE_WIN
            buildTarget = BuildTarget.StandaloneWindows64;
#endif
            isCheckUnique = constants.ResourcesPathSetting.RESOURCES_FOLDER != GetSpecificPlatformFolder(buildTarget); // 如果读取资源来自同一个文件不用检查特殊性。
            List<AssetGroup> retGroups = buildGroup();
            return retGroups;
        }

        private List<AssetGroup> buildGroup()
        {
            List<AssetGroup> retGroups = new List<AssetGroup>();
            List<string> resPaths = new List<string>();
            string uiFolderPath = $"{Application.dataPath}/{constants.ResourcesPathSetting.RESOURCES_FOLDER}/UI/Logic";

            if (Directory.Exists(uiFolderPath))
            {
                DirectoryInfo root = new DirectoryInfo(uiFolderPath);
                DirectoryInfo[] allDir = root.GetDirectories();
                for (int i = 0; i < allDir.Length; i++)
                {
                    string tmpPath = $"{allDir[i].FullName}/Prefabs/Main";
                    resPaths.Add(tmpPath);

                    tmpPath = $"{allDir[i].FullName}/Animations/Main";
                    resPaths.Add(tmpPath);

                    tmpPath = $"{allDir[i].FullName}/Materials/Main";
                    resPaths.Add(tmpPath);
                }
            }
            
            foreach (string path in resPaths)
            {
                retGroups.Add(packOneGroup(path));
            }


            #region 清除掉无资源资源组

            int count = 0;
            List<AssetGroup> nullAssetGroups = new List<AssetGroup>();
            foreach (AssetGroup item in retGroups)
            {
                // Debug.LogFormat("groupname:{0}, isCommon:{1},importance:{2} asset Count:{3}", item.groupName, item.IsCommon, item.Importance, item.assets.Count);
                count += item.Assets.Count;
                if (item.Assets.Count == 0)
                {
                    nullAssetGroups.Add(item);
                }
            }

            for (int i = 0; i < nullAssetGroups.Count; i++)
            {
                retGroups.Remove(nullAssetGroups[i]);
            }
            
            #endregion
            
            return retGroups;
        }

        /// <summary>
        /// 一个功能打成一个包。如家园功能在 application.path + ResourceRex/UI/Logic/Home/Prefab/Main,需要打包资源都放在Main文件夹中
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        AssetGroup packOneGroup(string path)
        {
            string groupNm = "";
            if (!path.Contains("Logic"))
            {
                groupNm = "Common";
            }
            else
            {
                groupNm = path.Substring(path.IndexOf("Logic") + 6);//取路径代表功能字段，如家园Home/Prefab/Main, 6 = “Logic/”.length
                groupNm = groupNm.Split('/')[0]; //取到功能字段，如家园Home/Prefab/Main中Home
            }

            AssetGroup group = new AssetGroup
            {
                GroupName = groupNm
            };

            if (Directory.Exists(path))
            {
                FileInfo[] files = new DirectoryInfo(path).GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    // 能这么判断是在大家严格按照规定路径存放制定资源情况下
                    if (file.Name.EndsWith(".prefab") || file.Name.EndsWith(".png") || file.Name.EndsWith(".jpg") || file.Name.EndsWith(".anim") || file.Name.EndsWith(".mat"))
                    {
                        string filepath = file.FullName;
                        if (isCheckUnique && IsReplaceUniqueRes(filepath, out string uniqueRes))
                        {
                            filepath = uniqueRes; //用特有资源代替通用资源
                        }
                        string assetPath = filepath.Substring(filepath.IndexOf("Assets", StringComparison.Ordinal));
                        AssetItem item = new AssetItem
                        {
                            assetPath = assetPath,
                            importance = DEFAULT_UI_IMPORTANCE,
                            subLevelNames = null
                        };
                        group.Assets.Add(item);
                    }
                }
            }
            return group;
        }

        private bool IsReplaceUniqueRes(string fullName, out string uniqueRes)
        {
            fullName = fullName.Replace('\\', '/');
            uniqueRes = fullName.Replace(constants.ResourcesPathSetting.RESOURCES_FOLDER, GetSpecificPlatformFolder(buildTarget));
            return File.Exists(uniqueRes);
        }
        
        /// <summary>
        /// 获取该文件夹下所有的图集路径pathname
        /// </summary>
        /// <param name="folder">要查找的文件夹</param>
        /// <returns></returns>
        List<string> getAtlasFilesPath(string folder)
        {
            List<string> paths = new List<string>();
            if (Directory.Exists(folder))
            {
                FileInfo[] files = new DirectoryInfo(folder).GetFiles("*.*", SearchOption.AllDirectories);
                foreach (FileInfo file in files)
                {
                    if (file.Name.EndsWith(".spriteatlas"))
                    {
                        paths.Add(file.FullName);
                    }
                }
            }
            return paths;
        }
    }
}