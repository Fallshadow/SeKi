using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ASeKi.AssetBundleCore;

namespace ASeKi.AssetBundleCore
{
    // 用来启动资源加载相关
    public class ResourceLoaderLauncher : SingletonMonoBehavior<ResourceLoaderLauncher>
    {
        protected override void Awake()
        {
            base.Awake();
            ResourceLoaderProxy.instance.Init(debug.DebugConfig.instance.DisableAssetBundle);
        }


    }

}
