using System.Collections.Generic;
using System.IO;
using act.AssetBundleCore;
using act.Resource;
using constants;
using UnityEditor;
using UnityEngine;

namespace act.UIRes
{
    [System.Serializable]
    public enum UiAssetType
    {
        UAT_NONE = -1,
        UAT_PREFAB = 0,                    // 预制体
        UAT_ATLAS = 1,                     // 图集
        UAT_HIGH_DEFINITION_TEX = 2,       // 高清图
        UAT_TMP_FONTASSET = 3,             // 字体资源
        UAT_MATERIAL = 4,                  // 材质
        UAT_ANIMATION = 5,                 // 动画
        UAT_TXT = 6,                       // 文本资源
    }
    
    [System.Serializable]
    public class UiAssetInfo
    {
        public UiAssetType AssetType;
        public string AssetName;
        public int HashCode;
        public int Index;
    }
    
    [CreateAssetMenu(fileName = "AssetHashMap_UI", menuName = "AssetHashMap/AssetHashMap_UI_SO")]
    public class AssetHashMap_UI : AssetHashMapBaseSO
    {
        public List<UiAssetInfo> UiAssetInfos = new List<UiAssetInfo>();
        
#if UNITY_EDITOR
        public override void Automatic()
        {
            // 清空资源信息
            if (UiAssetInfos == null)
            {
                ASeKi.debug.PrintSystem.LogError($"[Asset Load][AssetHashMap_UI] Editor init UiAssetInfoSO can`t find!");
                return;
            }
            UiAssetInfos.Clear();
            // 刷新资源信息
            RefreshBaseInfo();
            AssetDatabase.SaveAssets();
        }
        
        public virtual void RefreshBaseInfo()
        {
            CreateAssetInfo(ResourcesPathSetting.RESOURCES_FOLDER, true);
        }
        
        // 收集资源
        protected void CreateAssetInfo(string folder, bool isFilter)
        {
            string uiFolderPath = $"{Application.dataPath}/{folder}UI/Logic";
            ASeKi.debug.PrintSystem.Log($"[Asset Load][AssetHashMap_UI] 开始收集此目录下的资源 {uiFolderPath}");
            if (Directory.Exists(uiFolderPath))
            {
                DirectoryInfo root = new DirectoryInfo(uiFolderPath);
                DirectoryInfo[] allDir = root.GetDirectories();
                for (int i = 0; i < allDir.Length; i++)
                {
                    string tmpPath = $"{allDir[i].FullName}/Prefabs/Main";
                    if (Directory.Exists(tmpPath))
                    {
                        AddAssetInfo(tmpPath, UiAssetType.UAT_PREFAB, isFilter);
                    }

                    tmpPath = $"{allDir[i].FullName}/Animations/Main";
                    if (Directory.Exists(tmpPath))
                    {
                        AddAssetInfo(tmpPath, UiAssetType.UAT_ANIMATION, isFilter);
                    }

                    tmpPath = $"{allDir[i].FullName}/Materials/Main";
                    if (Directory.Exists(tmpPath))
                    {
                        AddAssetInfo(tmpPath, UiAssetType.UAT_MATERIAL, isFilter);
                    }

                    tmpPath = $"{allDir[i].FullName}/Atlas";
                    if (Directory.Exists(tmpPath))
                    {
                        AddAssetInfo(tmpPath, UiAssetType.UAT_ATLAS, isFilter);
                    }
                }
            }
            
            uiFolderPath = $"{Application.dataPath}/{folder}UI/Common/Prefabs/Main";//通用文件夹内预制体
            if (Directory.Exists(uiFolderPath))
            {
                AddAssetInfo(uiFolderPath, UiAssetType.UAT_PREFAB, isFilter);
            }

            uiFolderPath = $"{Application.dataPath}/{folder}UI/Common/Atlas"; //通用文件夹图集
            if (Directory.Exists(uiFolderPath))
            {
                AddAssetInfo(uiFolderPath, UiAssetType.UAT_ATLAS, isFilter);
            }

            uiFolderPath = $"{Application.dataPath}/{folder}UI/HighDefinitionTex"; //高清图片
            if (Directory.Exists(uiFolderPath))
            {
                AddAssetInfo(uiFolderPath, UiAssetType.UAT_HIGH_DEFINITION_TEX, isFilter);
            }

            uiFolderPath = $"{Application.dataPath}/{folder}UI/Fonts"; //字体资源
            if (Directory.Exists(uiFolderPath))
            {
                AddAssetInfo(uiFolderPath, UiAssetType.UAT_TMP_FONTASSET, isFilter);
                AddAssetInfo(uiFolderPath, UiAssetType.UAT_MATERIAL, isFilter);
            }

            uiFolderPath = $"{Application.dataPath}/{folder}UI/Emoji"; //表情资源
            if (Directory.Exists(uiFolderPath))
            {
                AddAssetInfo(uiFolderPath, UiAssetType.UAT_TXT, isFilter);
            }

            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            Debug.Log("-----------已刷新UI资源Hash信息------------");
        }
#endif
        /// <summary>
        /// 加入资源信息
        /// </summary>
        /// <param name="assetFolderPath">资源路径</param>
        /// <param name="assetType">UI资源类型</param>
        /// <param name="isFilter">特殊的资源（独一无二，切平台也不变）不需要多次刷新</param>
        public void AddAssetInfo(string assetFolderPath, UiAssetType assetType, bool isFilter)
        {
            string suffix = "";
            if (assetType == UiAssetType.UAT_ATLAS)
            {
                suffix = ".spriteatlas";
            }
            else if (assetType == UiAssetType.UAT_PREFAB)
            {
                suffix = ".prefab";
            }
            else if (assetType == UiAssetType.UAT_TMP_FONTASSET)
            {
                suffix = ".asset";
            }
            else if (assetType == UiAssetType.UAT_MATERIAL)
            {
                suffix = ".mat";
            }
            else if (assetType == UiAssetType.UAT_TXT)
            {
                suffix = ".txt";
            }
            else if (assetType == UiAssetType.UAT_ANIMATION)
            {
                suffix = ".anim";
            }
            else if (assetType == UiAssetType.UAT_HIGH_DEFINITION_TEX) // 图片特殊处理
            {
                if (Directory.Exists(assetFolderPath))
                {
                    FileInfo[] files = new DirectoryInfo(assetFolderPath).GetFiles("*.*", SearchOption.AllDirectories);
                    int strLen = 4; //.png 和 .jpg的长度
                    foreach (FileInfo file in files)
                    {
                        if (file.Name.EndsWith(".jpg") || file.Name.EndsWith(".png"))
                        {
                            UiAssetInfo info = generateAssetInfo(file, assetType, strLen);
                            if (info == null)
                            {
                                continue;
                            }

//                            if (isFilter && uniqueAssets.Contains(info.AssetName)) //如果资源特殊化就不添加通用部分
//                            {
//                                continue;
//                            }
                            
                            UiAssetInfos.Add(info);
                        }
                    }
                };
                return;
            }
            
            if (Directory.Exists(assetFolderPath))
            {
                FileInfo[] files = new DirectoryInfo(assetFolderPath).GetFiles("*.*", SearchOption.AllDirectories);
                int strLen = suffix.Length;
                foreach (FileInfo file in files)
                {
                    if (file.Name.EndsWith(suffix))
                    {
                        UiAssetInfo info = generateAssetInfo(file, assetType, strLen);
                        if (info == null)
                        {
                            continue;
                        }

//                        if (isFilter && uniqueAssets.Contains(info.AssetName))
//                        {
//                            continue;
//                        }
                        UiAssetInfos.Add(info);
                    }
                }
            }
        }
        
        /// <summary>
        /// 生成Ui资源的AssetInfo
        /// </summary>
        /// <param name="file">用于记录资源名称、生成HashCode</param>
        /// <param name="assetType">UI资源类型</param>
        /// <param name="suffixLen">需要去除的路径后缀（图片不知道为什么要去除）</param>
        /// <returns></returns>
        private static UiAssetInfo generateAssetInfo(FileInfo file, UiAssetType assetType, int suffixLen)
        {
            UiAssetInfo info = new UiAssetInfo();
            info.AssetType = assetType;
            info.AssetName = file.Name.Substring(0, file.Name.Length - suffixLen);
            bool suc = System.Enum.TryParse(info.AssetName, false, out UiAssetIndex res);
            if (!suc)
            {
                Debug.LogError($"Can` find asset index: [ {info.AssetName} ], Please register AssetIndex In UiAssetEnum");
                return null;
            }
            info.Index = (int)res;
            string path = file.FullName.Substring(file.FullName.IndexOf("Assets"));
            path = path.Replace('\\', '/');
            info.HashCode = ResourceUtility.GetHashCodeByAssetPath(path);
            return info;
        }
    }
}