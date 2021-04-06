using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.AnimGraphs
{
    /// <summary>
    /// 缓存例子
    /// 可以将其向下扩展成每个职业或者分类一个缓存池
    /// </summary>
    public class AnimGraphCache : System.IDisposable
    {
        private static AnimGraphCache instance;

        public static AnimGraphCache Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AnimGraphCache();
                }
                return instance;
            }
        }

        private AssetCache cache;

        private AnimGraphCache()
        {
            // TODO: 这里是池深度表达，可以根据实际情况调整
            const int MAX = 20;
            cache = new AssetCache(MAX,(int resourceID) => {
                // TODO:这里调用底层资源系统API，进行资源回收，传入的参数就是资源id
                // TODO:这里是Rex自己的资源读取脚本，需要搬过来
                // act.AssetBundleCore.ResourceLoaderProxy.instance.UnloadInSync(resourceID);
            });
            AnimGraphLoader.LoadAnimationClip = LoadAnimationClip;
            AnimGraphLoader.LoadAvatarMask = LoadAvatarMask;
            AnimGraphLoader.UnloadAnimationClip = UnloadAnimationClip;
            AnimGraphLoader.UnloadAvatarMask = UnloadAvatarMask;
        }

        private int GetAnimationAssetID(int asset, int overrideType)
        {
            return asset;
        }

        private int GetAvatarMaskAssetID(int asset,int overrideType)
        {
            return asset;
        }

        private AnimationClip LoadAnimationClip(int asset,int overrideType)
        {
            var resourceID = GetAnimationAssetID(asset, overrideType);
            if (cache.ContainsKey(resourceID))
            {
                return cache.New<AnimationClip>(resourceID);
            }
            // TODO：这里 AssetMapping.GetAnimationClip 替换成项目资源加载API
            // TODO:这里是Rex自己的资源读取脚本，需要搬过来
            // cache.Register(resourceID, act.AssetBundleCore.ResourceLoaderProxy.instance.LoadAsset<AnimationClip>(resourceID));
            return cache.New<AnimationClip>(resourceID);
        }

        private AvatarMask LoadAvatarMask(int asset, int overrideType)
        {
            var resourceID = GetAvatarMaskAssetID(asset, overrideType);
            if (cache.ContainsKey(resourceID))
            {
                return cache.New<AvatarMask>(resourceID);
            }
            // TODO：这里 AssetMapping.GetAvatarMask 替换成项目资源加载API
            // TODO:这里是Rex自己的资源读取脚本，需要搬过来
            // cache.Register(resourceID, act.AssetBundleCore.ResourceLoaderProxy.instance.LoadAsset<AvatarMask>(resourceID));
            return cache.New<AvatarMask>(resourceID);
        }

        private void UnloadAnimationClip(int asset,int overrideType)
        {
            var resourceID = GetAnimationAssetID(asset, overrideType);
            cache.Recycling(resourceID);
        }

        private void UnloadAvatarMask(int asset, int overrideType)
        {
            var resourceID = GetAvatarMaskAssetID(asset, overrideType);
            cache.Recycling(resourceID);
        }

        public void Update()
        {
            cache.DestroyStep();
        }

        public void Dispose()
        {
            cache.Dispose();
        }
    }
}
