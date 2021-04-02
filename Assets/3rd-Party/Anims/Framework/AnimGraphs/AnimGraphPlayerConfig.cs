using UnityEngine;

namespace Framework.AnimGraphs
{
    public unsafe sealed class AnimGraphPlayerConfig
    {
        // 输出动画的Animator
        public Animator animator;

        // 序列化数据的头指针
        public byte* assetPtr;
        public int overrideControllerType = 1;

        private Transform customRoot;
        
        public Transform GetHierachyRoot()
        {
            if (animator == null)
            {
                return null;
            }
            
            return customRoot == null ? animator.transform : customRoot;
        }

        public void SetHierachyRoot(Transform root)
        {
            this.customRoot = root;
        }
    }
}