using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ASeKi.action
{
    public class AssetDepTree
    {
        public Dictionary<int, AssetTarget> targetContainer = new Dictionary<int, AssetTarget>();
        
        // 创建/获取依赖树（此时还没有依赖关系网）
        public AssetTarget GetOrNew(FileInfo file, AssetDepTree depTree)
        {
            AssetTarget target = null;
            string fullPath = file.FullName;
            int index = fullPath.IndexOf("Assets");
            if (index != -1)
            {
                string assetPath = fullPath.Substring(index);
                Object o = AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (o != null)
                {
                    int instanceId = o.GetInstanceID();

                    if (targetContainer.ContainsKey(instanceId))
                    {
                        target = targetContainer[instanceId];
                    }
                    else
                    {
                        //检测是否root里包含的asset
                        foreach (var item in targetContainer)
                        {
                            var assetTarget = item.Value;

                            if (assetTarget.bMulitipleAssetRoot)
                            {
                                var assets = assetTarget.rootAssets;
                                foreach (var asset in assets)
                                {
                                    if (asset == o)
                                    {
                                        return assetTarget;
                                    }
                                }
                            }
                        }

                        //检测图集包含该贴图
                        if (o is Texture2D)
                        {
                            foreach (var item in targetContainer)
                            {
                                var assetTarget = item.Value;

                                if (assetTarget.bAtlas && assetTarget.CheckTextureInAltas(o as Texture2D))
                                {
                                    return assetTarget;
                                }
                            }
                        }
                        target = new AssetTarget(o, assetPath);
                        target.depTree = depTree;
                        targetContainer[instanceId] = target;
                    }
                }
            }
            return target;
        }
        
        // 创建/获取依赖树（此时还没有依赖关系网）
        public AssetTarget GetOrNew(string assetPath, AssetDepTree depTree)
        {
            AssetTarget target = null;
            Object o = AssetDatabase.LoadMainAssetAtPath(assetPath);
            if (o != null)
            {
                int instanceId = o.GetInstanceID();

                if (targetContainer.ContainsKey(instanceId))
                {
                    target = targetContainer[instanceId];
                }
                else
                {
                    target = new AssetTarget(o, assetPath);
                    target.depTree = depTree;
                    targetContainer[instanceId] = target;
                }
            }
            else
            {
                debug.PrintSystem.LogError($"GetOrNew AssetTarget Fail: {assetPath}");
            }

            return target;
        }
        
        // 生成依赖关系，将树建立起来
        public List<AssetTarget> Work()
        {
            Analyze();
            Merge();
            return GetAll();
        }
        
        public void Analyze()
        {
            var assetTargets = GetAll();
            foreach (var item in assetTargets)
            {
                item.Analyze();
            }
        }
        
        public void Merge()
        {
            var assetTargets = GetAll();
            foreach (var item in assetTargets)
            {
                item.Merge();
            }
        }
        
        public List<AssetTarget> GetAll()
        {
            return new List<AssetTarget>(targetContainer.Values);
        }
    }
}