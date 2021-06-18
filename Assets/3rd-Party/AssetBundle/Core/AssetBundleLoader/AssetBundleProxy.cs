using UnityEngine;

namespace act.AssetBundleCore
{
    public class AssetBundleProxy
    {
        public bool IsLoading
        {
            get { return isLoading; }
            set { isLoading = value; }
        }

        protected AssetBundleLoaderManager manager; 
        protected AssetBundle ab;

        public int abHash;
        private string file;
        private ulong offset;
        private bool isLoading;
        private int refCount = 0;
        private bool isFinished;

        public virtual void AddAssetsReference()
        {
            refCount++;
        }

        public virtual void Init(AssetBundleLoaderManager manager, int abHash, string file, ulong offset)
        {
            this.manager = manager;
            this.abHash = abHash;
            this.file = file;
            this.offset = offset;
            isLoading = false;
            isFinished = false;
            ab = null;
            refCount = 0;
        }

        public virtual void destroySelf()
        {
            ab = null;
            this.manager = null;
            this.abHash = -1;
            this.file = null;
            this.offset = 0;
            isLoading = false;
            isFinished = false;
            refCount = 0;
        }

        public void LoadSync()
        {
            ab = AssetBundle.LoadFromFile(GetBundleSourceFile(file, false), 0, offset);
            isLoading = false;
        }

        public T LoadAsset<T>(string assetName) where T : UnityEngine.Object
        {
            var asset = ab.LoadAsset<T>(assetName);
            return asset;
        }

        public string GetBundleSourceFile(string path, bool forWWW = true)
        {
            string filePath = null;
#if UNITY_STANDALONE_WIN
#if UNITY_EDITOR
                if (forWWW)
                    filePath = string.Format("file://{0}/StreamingAssets/{1}/{2}", Application.dataPath, "AssetBundle/x64", path);
                else
                    filePath = string.Format("{0}/x64/Full/{1}", manager.BundlePrePath, path);
#else
                if (forWWW)
                    filePath = string.Format("file://{0}/StreamingAssets/{1}/{2}", Application.dataPath, "AssetBundle/x64", path);
                else
                    filePath = string.Format("{0}/StreamingAssets/{1}/{2}", Application.dataPath, "AssetBundle/x64", path);
#endif
#elif UNITY_ANDROID

#if UNITY_EDITOR
            if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D11
                || SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Direct3D12)
            {
                if(forWWW)
                    filePath = string.Format("file://{0}/StreamingAssets/{1}/{2}", Application.dataPath, "AssetBundle/x64", path);
                else
                    filePath = string.Format("{0}/x64/Full/{1}", manager.BundlePrePath, path);
            }
            else
            {
                if(forWWW)
                    filePath = string.Format("file://{0}/StreamingAssets/{1}/{2}", Application.dataPath, "AssetBundle/Android", path);
                else
                    filePath = string.Format("{0}/Android/Full/{1}", manager.BundlePrePath, path);
            }
#else
            if (forWWW)
                filePath = string.Format("jar:file://{0}!/assets/{1}/{2}", Application.dataPath, "AssetBundle/Android", path);
            else
                filePath = string.Format("{0}!assets/{1}/{2}", Application.dataPath, "AssetBundle/Android", path);
#endif
#elif UNITY_IOS
#if UNITY_EDITOR
            if (forWWW)
                filePath = string.Format("file://{0}/StreamingAssets/{1}/{2}", Application.dataPath, "AssetBundle/IOS", path);
            else
                filePath = string.Format("{0}/IOS/Full/{1}", manager.BundlePrePath, path);
#else
            if (forWWW)
                filePath = string.Format("file://{0}/Raw/{1}/{2}", Application.dataPath, "AssetBundle/IOS", path);
            else
                filePath = string.Format("{0}/Raw/{1}/{2}", Application.dataPath, "AssetBundle/IOS", path);
#endif
#else
            throw new System.NotImplementedException();
#endif
            return filePath;
        }

        public virtual bool RemoveAssetsReference()
        {
            refCount--;
            if (refCount == 0)
            {
#if DEBUG_ASSETBUNDLE
                    act.debug.PrintSystem.LogWarning(string.Format("RemoveAssetsReference with destroy ab:{0} file:{1}", abHash, file));
#endif
                ab.Unload(true);

                manager.DestroyProxy(this);
                destroySelf();
                return true;
            }
            return false;
        }
    }
}
