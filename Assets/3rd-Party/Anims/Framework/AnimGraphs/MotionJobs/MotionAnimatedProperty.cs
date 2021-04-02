using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.Animations;
using Unity.Collections;

namespace Framework.AnimGraphs
{
    public abstract class MotionAnimatedProperty<TJob, TValue> : MotionJob<TJob>, IDisposable
        where TJob : struct, IAnimationJob
        where TValue : struct
    {
        protected NativeArray<PropertyStreamHandle> properties;
        protected NativeArray<TValue> values;

        protected MotionAnimatedProperty(AnimGraphPlayer graphPlayer, int propertyCount,
            NativeArrayOptions options = NativeArrayOptions.ClearMemory)
        {
            properties = new NativeArray<PropertyStreamHandle>(propertyCount, Allocator.Persistent, options);
            values = new NativeArray<TValue>(propertyCount, Allocator.Persistent);
            
            createJob();
            createPlayable(graphPlayer);
            
            graphPlayer.Disposables.Enqueue(this);
        }

        protected MotionAnimatedProperty(AnimGraphPlayer graphPlayer, string propertyName)
            : this(graphPlayer, 1, NativeArrayOptions.UninitializedMemory)
        {
            var animator = graphPlayer.GetAnimator();
            properties[0] = animator.BindStreamProperty(animator.transform, typeof(Animator), propertyName);
        }

        protected MotionAnimatedProperty(AnimGraphPlayer graphPlayer, params string[] propertyNames)
            : this(graphPlayer, propertyNames.Length, NativeArrayOptions.UninitializedMemory)
        {
            var count = propertyNames.Length;

            var animator = graphPlayer.GetAnimator();
            var transform = graphPlayer.GetHeirachyRoot();

            for (int i = 0; i < count; i++)
                initialiseProperty(animator, i, transform, typeof(Animator), propertyNames[i]);
        }

        public void initialiseProperty(Animator animator, int index, string name)
            => initialiseProperty(animator, index, animator.transform, typeof(Animator), name);

        private void initialiseProperty(Animator animator, int index, Transform transform, Type type, string name)
            => properties[index] = animator.BindStreamProperty(transform, type, name);

        public bool IsValid => properties.IsCreated && values.IsCreated;

        protected abstract void createJob();

        public TValue Value => this[0];

        public static implicit operator TValue(MotionAnimatedProperty<TJob, TValue> properties) => properties[0];

        public TValue GetValue(int index) => values[index];

        public TValue this[int index] => values[index];

        public void GetValues(ref TValue[] values)
        {
            var count = this.values.Length;
            if (values == null || values.Length != count)
                values = new TValue[count];

            this.values.CopyTo(values);
        }

        public TValue[] GetValues()
        {
            var values = new TValue[this.values.Length];
            this.values.CopyTo(values);
            return values;
        }

        void IDisposable.Dispose() => Dispose();

        protected virtual void Dispose()
        {
            if (properties.IsCreated)
            {
                properties.Dispose();
                values.Dispose();
            }
        }

        public override void Destroy()
        {
            Dispose();
            base.Destroy();
        }
    }
}