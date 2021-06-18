#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace act.AssetBundleUtility
{
    public class AssetBundleGrouper
    {
        public const int DEFAULT_SHADER_IMPORTANCE = 4;

        private static int localDataPathLength = -1;

        public static int LocalDataPathLength
        {
            get
            {
                if (localDataPathLength == -1)
                {
                    localDataPathLength = Application.dataPath.Length - "Assets".Length;
                }
                return localDataPathLength;
            }
        }

        public static string GetAssetPath(string fullPath)
        {
            return fullPath.Replace('\\', '/').Remove(0, LocalDataPathLength);
        }

        // NOTE: 如果路徑中有和預設資料夾名稱重複會錯誤
        public static string GetAssetPath(string fullPath, BuildTarget buildTarget)
        {
            fullPath = fullPath.Replace('\\', '/');
            string specPath = fullPath.Replace(constants.ResourcesPathSetting.RESOURCES_FOLDER, GetSpecificPlatformFolder(buildTarget));
            if (File.Exists(specPath))
            {
                return specPath.Remove(0, LocalDataPathLength);
            }
            return fullPath.Remove(0, LocalDataPathLength);
        }

        // 获取各个平台的文件夹位置
        public static string GetSpecificPlatformFolder(BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                case BuildTarget.StandaloneWindows64:
                    return constants.ResourcesPathSetting.RESOURCES_PC_FOLDER;
                case BuildTarget.Android:
                    return constants.ResourcesPathSetting.RESOURCES_ANDROID_FOLDER;
                case BuildTarget.iOS:
                    return constants.ResourcesPathSetting.RESOURCES_IOS_FOLDER;
                default:
                    return constants.ResourcesPathSetting.RESOURCES_FOLDER;
            }
        }

        protected BuildTarget buildTarget;

        public virtual void Init() { }

        public virtual List<AssetGroup> Work(BuildTarget buildTarget = BuildTarget.Android)
        {
            this.buildTarget = buildTarget;
            return null;
        }

        public virtual List<AssetGroup> WorkForEditor()
        {
#if UNITY_ANDROID
            return Work(BuildTarget.Android);
#elif UNITY_IPHONE
            return Work(BuildTarget.iOS);
#elif UNITY_STANDALONE_WIN
            return Work(BuildTarget.StandaloneWindows64);
#else
            return Work();
#endif
        }
    }
}
#endif