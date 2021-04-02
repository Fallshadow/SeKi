using Unity.Burst;
using Unity.Collections;
using UnityEngine.Animations;

namespace Framework.AnimGraphs
{
    public sealed class MotionAnimatedBool : MotionAnimatedProperty<MotionAnimatedBool.Job, bool>
    {
        public MotionAnimatedBool(AnimGraphPlayer graphPlayer, int propertyCount,
            NativeArrayOptions options = NativeArrayOptions.ClearMemory)
            : base(graphPlayer, propertyCount, options)
        {
        }

        public MotionAnimatedBool(AnimGraphPlayer graphPlayer, string propertyName)
            : base(graphPlayer, propertyName)
        {
        }

        public MotionAnimatedBool(AnimGraphPlayer graphPlayer, params string[] propertyNames)
            : base(graphPlayer, propertyNames)
        {
        }

        protected override void createJob()
        {
            job = new Job() {properties = properties, values = values};
        }

        [BurstCompile]
        public struct Job : IAnimationJob
        {
            public NativeArray<PropertyStreamHandle> properties;
            public NativeArray<bool> values;

            public void ProcessRootMotion(AnimationStream stream)
            {
            }

            public void ProcessAnimation(AnimationStream stream)
            {
                for (int i = properties.Length - 1; i >= 0; i--)
                    if (properties[i].IsValid(stream))
                        values[i] = properties[i].GetBool(stream);
            }
        }
    }
}