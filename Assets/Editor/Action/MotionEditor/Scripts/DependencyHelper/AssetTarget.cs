using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using Object = UnityEngine.Object;
using act.Resource;

namespace ASeKi.action
{
    public class AssetTarget
    {
        public readonly HashSet<AssetTarget> DependParentSet = new HashSet<AssetTarget>();    // 我要依赖的项
        public readonly HashSet<AssetTarget> DependChildrenSet = new HashSet<AssetTarget>();    // 依赖我的项
        public readonly Object asset;
        public readonly string AssetPath;
        public readonly int hash;
        
        public readonly bool bMulitipleAssetRoot;
        public readonly List<Object> rootAssets;
        private readonly HashSet<Object> hashSetObjs;
        private readonly HashSet<string> hashSetPaths;
        public AssetDepTree depTree;
        bool _isAnalyzed = false;
        
        public bool bAtlas = false;
        private Texture2D[] atlasTextures;
        
        public AssetTarget(Object o, string assetPath)
        {
            asset = o;
            assetPath = assetPath.Replace('\\', '/');
            hash = ResourceUtility.GetHashCodeByAssetPath(assetPath, false);
            AssetPath = assetPath;

            var atlas = o as SpriteAtlas;
            if (atlas != null)
            {
                bAtlas = true;

                var sprites = loadAltasInfo(o);
                var arrarySize = sprites.arraySize;

                atlasTextures = new Texture2D[arrarySize];

                for (int i = 0; i < arrarySize; i++)
                {
                    var sp = sprites.GetArrayElementAtIndex(i).objectReferenceValue as Sprite;
                    if (sp == null || sp.texture == null)
                    {
                        throw new Exception($"Sprite is null{assetPath}");
                    }

                    atlasTextures[i] = sp.texture;
                }
            }
        }
        
        SerializedProperty loadAltasInfo(Object o)
        {
            SerializedObject altasSerializedObject = new SerializedObject(o);
            var sprites = altasSerializedObject.FindProperty("m_PackedSprites");
            return sprites;
        }
        
        public void Analyze()
        {
            if (_isAnalyzed) return;
            _isAnalyzed = true;

            Object[] deps = null;
            if (bMulitipleAssetRoot)
            {
                var lDeps = new List<Object>();
                var allDeps = EditorUtility.CollectDependencies(rootAssets.ToArray());
                foreach (var dep in allDeps)
                {
                    if (!hashSetObjs.Contains(dep))
                    {
                        lDeps.Add(dep);
                    }
                }

                deps = lDeps.ToArray();
            }
            else
            {
                deps = EditorUtility.CollectDependencies(new Object[] {asset});
            }

            List<Object> depList = new List<Object>();

            for (int i = 0; i < deps.Length; i++)
            {
                Object o = deps[i];

                //不包含脚本对象
                //不包含LightingDataAsset对象
                if (o is MonoScript)
                    continue;

                //不包含builtin对象
                string path = AssetDatabase.GetAssetPath(o);

                if (String.IsNullOrEmpty(path))
                {
                    Debug.LogWarning("invalid:" + o);
                    continue;
                }

                if (path.StartsWith("Resources") || path.Contains("unity default resources"))
                {
                    continue;
                }

                if (bMulitipleAssetRoot)
                {
                    //排除自己依赖自己
                    if (hashSetPaths.Contains(path))
                    {
                        continue;
                    }
                }

                depList.Add(o);
            }

            var res = from s in depList
                let obj = AssetDatabase.GetAssetPath(s)
                select obj;
            var paths = res.Distinct().ToArray();

            for (int i = 0; i < paths.Length; i++)
            {
                var path = paths[i];
                if (File.Exists(path) == false)
                {
                    Debug.LogWarning("invalid:" + paths[i]);
                    continue;
                }

                FileInfo fi = new FileInfo(path);
                AssetTarget target = depTree.GetOrNew(fi, depTree);
                if (target == null)
                    continue;

                this.addDependParent(target);

                target.Analyze();
            }
        }
        
        // 保证引用链为一层层的，非跨层引用
        public void Merge()
        {
            if (DependChildrenSet.Count > 1)
            {
                var children = new List<AssetTarget>(DependChildrenSet);
                removeDependChildren();
                foreach (AssetTarget child in children)
                {
                    child.addDependParent(this);
                }
            }
        }
        
        private void removeDependChildren()
        {
            var all = new List<AssetTarget>(DependChildrenSet);
            DependChildrenSet.Clear();
            foreach (AssetTarget child in all)
            {
                child.DependParentSet.Remove(this);
            }
        }

        //自己依赖于target，_dependParentSet存放依赖的对象
        private void addDependParent(AssetTarget target)
        {
            if (target == this || containsDepend(target))
                return;

            DependParentSet.Add(target);
            target.addDependChild(this);
            this.clearParentDepend(target);
        }
        
        //检查自己或者祖先（recursive = true）是否已经依赖于target
        private bool containsDepend(AssetTarget target, bool recursive = true)
        {
            if (DependParentSet.Contains(target))
                return true;
            if (recursive)
            {
                var e = DependParentSet.GetEnumerator();
                while (e.MoveNext())
                {
                    if (e.Current.containsDepend(target, true))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
        //添加自己的孩子
        private void addDependChild(AssetTarget parent)
        {
            DependChildrenSet.Add(parent);
        }
        
        // 我依赖了这个项，那么依赖我的项不需要直接依赖这个项了
        private void clearParentDepend(AssetTarget target = null)
        {
            IEnumerable<AssetTarget> cols = DependParentSet;
            if (target != null) cols = new AssetTarget[] {target};
            foreach (AssetTarget at in cols)
            {
                var e = DependChildrenSet.GetEnumerator();
                while (e.MoveNext())
                {
                    AssetTarget dc = e.Current;
                    dc.removeDependParent(at);
                }
            }
        }
        
        // 移除依赖项,包括子孙都不依赖该项
        private void removeDependParent(AssetTarget target, bool recursive = true)
        {
            DependParentSet.Remove(target);
            target.DependChildrenSet.Remove(this);

            //recursive
            var dcc = new HashSet<AssetTarget>(DependChildrenSet);
            var e = dcc.GetEnumerator();
            while (e.MoveNext())
            {
                AssetTarget dc = e.Current;
                dc.removeDependParent(target);
            }
        }
        
        public bool CheckTextureInAltas(Texture2D texure)
        {
            foreach (var tex in atlasTextures)
            {
                if (tex == texure)
                {
                    return true;
                }
            }
            return false;
        }
    }
}