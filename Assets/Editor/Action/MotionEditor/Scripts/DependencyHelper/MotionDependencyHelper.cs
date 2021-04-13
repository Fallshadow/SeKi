
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ASeKi.action
{
    public class MotionDependencyHelper
    {
        private readonly List<AssetTarget> assets;
        
        public MotionDependencyHelper(string assetPath)
        {
            assets = WorkForAssetInfos(assetPath);
        }
        
        // 获取此路径的依赖项目标列表
        public List<AssetTarget> WorkForAssetInfos(string assetPath)
        {
            string[] statesPaths = AssetDatabase.FindAssets("", new[] { assetPath }).Select(AssetDatabase.GUIDToAssetPath).ToArray();
            if (statesPaths == null || statesPaths.Length == 0)
            {
                Debug.LogError($"Current Folder {MotionDataCollection.Path} is no Asset!");
                return null;
            }
            AssetDepTree depTree = new AssetDepTree();
            foreach (var statePath in statesPaths)
            {
                depTree.GetOrNew(statePath, depTree);
            }
            List<AssetTarget> allTarget = depTree.Work();
            return allTarget;
        }
        
        // 根据此路径的依赖项目标列表找到所有的依赖于它的项的路径目录
        public string[] GetReferences(string assetPath)
        {
            if (assets.IsNullOrEmpty())
            {
                return null;
            }
            var ret = assets.Find(x => x.AssetPath == assetPath);
            return ret?.DependChildrenSet.Select(x=>x.AssetPath).ToArray();
        }
    }
}