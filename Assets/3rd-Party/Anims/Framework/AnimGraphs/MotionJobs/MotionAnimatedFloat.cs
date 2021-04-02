using Unity.Burst;
using UnityEngine.Animations;
using Unity.Collections;

namespace Framework.AnimGraphs
{
    public sealed class MotionAnimatedFloat : MotionAnimatedProperty<MotionAnimatedFloat.Job, float>
    {
        public MotionAnimatedFloat(AnimGraphPlayer graphPlayer, int propertyCount,
            NativeArrayOptions options = NativeArrayOptions.ClearMemory)
            : base(graphPlayer, propertyCount, options)
        {
        }

        public MotionAnimatedFloat(AnimGraphPlayer graphPlayer, string propertyName)
            : base(graphPlayer, propertyName)
        {
        }

        public MotionAnimatedFloat(AnimGraphPlayer graphPlayer, params string[] propertyNames)
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
            public NativeArray<float> values;

            public void ProcessRootMotion(AnimationStream stream)
            {
            }

            public void ProcessAnimation(AnimationStream stream)
            {
                for (int i = properties.Length - 1; i >= 0; i--)
                    if (properties[i].IsValid(stream))
                        values[i] = properties[i].GetFloat(stream);
            }
        }
    }
}