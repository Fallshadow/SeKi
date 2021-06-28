using System.Collections.Generic;
using act.Resource;
using act.UIRes;
using UnityEditor;
using UnityEngine;

namespace act.AssetBundleUtility
{
    public partial class AssetbundleEditor
    {
        [MenuItem("Rex/AssetBundle_yfn/Utility/Log_Select_Hash")]
        public static void LogSelectHash()
        {
            Object selectObj = Selection.activeObject;
            string selectPath = AssetDatabase.GetAssetPath(selectObj);
            int selectHash = ResourceUtility.GetHashCodeByAssetPath(selectPath);
            Debug.Log($"name:{selectObj.name} ———— path:{selectPath} ———— hash:{selectHash}");
        }
            
        [MenuItem("Rex/AssetBundle_yfn/UI_System/RefresHash")]
        public static void RefreshUiAssetHash()
        {
            List<AssetHashMap_UI> hashSoLst = new List<AssetHashMap_UI>();
            string path = "Assets/ResourceSeki/ScriptableObject/UI/AssetHashMap/AssetHashMap_UI.asset";
            hashSoLst.Add(AssetDatabase.LoadAssetAtPath<AssetHashMap_UI>(path));
            foreach (var so in hashSoLst)
            {
                so.Automatic();
            }
            act.AssetBundleUtility.AssetbundleEditor.SerializeAssetDepenceInfoForFastMode_UI();
        }
        
        [MenuItem("Rex/AssetBundle_yfn/SerializeAssetDepenceInfoForFastMode/UI")]
        public static void SerializeAssetDepenceInfoForFastMode_UI()
        {
            UnityEditor.EditorUtility.DisplayProgressBar("弹窗", "正在快速生成依赖信息...UI", 0.25f);
            AssetBundlePackageManager.SerializeAssetDepenceInfoForFastMode(new AssetBundleGrouper_UI());
            UnityEditor.EditorUtility.DisplayProgressBar("弹窗", "依赖信息生成完毕！", 1f);
            UnityEditor.EditorUtility.ClearProgressBar();
        }
        
        [MenuItem("Rex/AssetBundle_yfn/SerializeAssetDepenceInfoForFastMode/ScriptableObject")]
        private static void SerializeAssetDepenceInfoForFastMode_ScriptableObject()
        {
            EditorUtility.DisplayProgressBar("弹窗", "正在快速生成依赖信息...SO", 0.25f);
            AssetBundlePackageManager.SerializeAssetDepenceInfoForFastMode(new AssetBundleGrouper_ScriptableObject());
            EditorUtility.DisplayProgressBar("弹窗", "依赖信息生成完毕！", 1f);
            EditorUtility.ClearProgressBar();
        }
    }
}