using UnityEngine;

namespace Framework
{
    public class PostInertializerTransitionProfile : ScriptableObject
    {
        public struct TransformConfig {
            public PostInertializer.EvaluateSpace Space;
            public float BlendTime;
        }

        public AvatarMask AvatarMask;

        public TransformConfig[] TransformConfigs;

        private int[] IndexMapping;

    }
}
