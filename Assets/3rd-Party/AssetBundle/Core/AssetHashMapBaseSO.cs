using UnityEngine;

namespace act.AssetBundleCore
{
    public class AssetHashMapBaseSO : ScriptableObject
    {
        public virtual void Initialize() { }
        
#if UNITY_EDITOR
        public virtual void Automatic() { }
#endif
    }
}