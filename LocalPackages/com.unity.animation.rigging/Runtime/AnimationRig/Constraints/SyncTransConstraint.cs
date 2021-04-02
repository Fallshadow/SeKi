using Unity.Collections;

namespace UnityEngine.Animations.Rigging
{
    public class SyncTransConstraint : RigConstraint<
        SyncTransConstraintJob,
        SyncTransConstraintData,
        SyncTransConstrainttDataJobBinder<SyncTransConstraintData>
        >
    {
        public void OnGraphChange()
        {
            data.IsGraphChange = true;
        }
        
        public void SetData(Animator animator, Transform cons, Transform source)
        {
            data.NeedRefresh = true;
            data.constraintObject = cons;
            data.sourceObject = source;
            data.animator = animator;
        }
    }

    public class SyncTransConstrainttDataJobBinder<T> : AnimationJobBinder<SyncTransConstraintJob, T>
        where T : struct, IAnimationJobData, ISyncTransConstrainttData
    {
        public override SyncTransConstraintJob Create(Animator animator, ref T data, Component component)
        {
            var job = new SyncTransConstraintJob();

            job.constraints = new NativeList<TransformStreamHandle>(Allocator.Persistent);
            job.sources = new NativeList<TransformSceneHandle>(Allocator.Persistent);
            job.isGraphChange = BoolProperty.Bind(animator, component, data.isGraphChangeBoolProperty);
            return job;
        }

        public override void Update(SyncTransConstraintJob job, ref T data)
        {
            base.Update(job, ref data);

            if (data.IsGraphChange)
            {
                data.IsGraphChange = false;
            }

            if (!data.NeedRefresh)
            {
                return;
            }

            data.NeedRefresh = false;

            var animator = data.TargetAnimator;
            
            job.constraints.Add(animator.BindStreamTransform(data.ConstraintObject));
            job.sources.Add(animator.BindSceneTransform(data.SourceObject));
        }

        public override void Destroy(SyncTransConstraintJob job)
        {
            job.constraints.Dispose();
            job.sources.Dispose();
        }
    }

    public interface ISyncTransConstrainttData
    {
        bool IsGraphChange { get; set; }
        bool NeedRefresh { get; set; }
        Transform ConstraintObject { get; }
        Transform SourceObject { get; }
        Animator TargetAnimator { get; }
        string isGraphChangeBoolProperty { get; }
    }
    
    [System.Serializable]
    public struct SyncTransConstraintData : IAnimationJobData, ISyncTransConstrainttData
    {
        public Animator animator;
        [SyncSceneToStream, SerializeField] public Transform constraintObject;
        [SyncSceneToStream, SerializeField] public Transform sourceObject;
        [SyncSceneToStream, SerializeField] private bool isGraphChange;

        public bool IsValid()
        {
            return true;
        }

        public void SetDefaultValues()
        {
            constraintObject = null;
            sourceObject = null;
        }

        public bool NeedRefresh { get; set; }

        
        public bool IsGraphChange
        {
            get => isGraphChange;
            set => isGraphChange = value;
        }

        public Transform ConstraintObject => constraintObject;
        public Transform SourceObject => sourceObject;
        public Animator TargetAnimator => animator;
        
        string ISyncTransConstrainttData.isGraphChangeBoolProperty => PropertyUtils.ConstructConstraintDataPropertyName(nameof(isGraphChange));

    }

    public struct SyncTransConstraintJob : IWeightedAnimationJob
    {
        public BoolProperty isGraphChange;
        
        public NativeList<TransformStreamHandle> constraints;
        public NativeList<TransformSceneHandle> sources;
        public FloatProperty jobWeight { get; set; }

        
        public void ProcessAnimation(AnimationStream stream)
        {
            float w = jobWeight.Get(stream);
            
            if (w <= 0f)
            {
//                for (var index = 0; index < constraints.Length; index++)
//                {
//                    var constraint = constraints[index];
//                    AnimationRuntimeUtils.PassThrough(stream, constraint);
//                }

                return;
            }

            if (isGraphChange.Get(stream))
            {
                return;
            }
            
            for (var index = 0; index < constraints.Length; index++)
            {
                var constraint = constraints[index];
                var source = sources[index];
                
                if (!constraint.IsValid(stream) || !source.IsValid(stream))
                {
                    continue;
                }
                
//                constraint.SetLocalPosition(stream, source.GetLocalPosition(stream));
//                constraint.SetLocalRotation(stream, source.GetLocalRotation(stream));
                constraint.SetLocalTRS(stream, Vector3.zero, Quaternion.identity, Vector3.one, false);
                constraint.SetPosition(stream, source.GetPosition(stream));
                constraint.SetRotation(stream, source.GetRotation(stream));
                //constraint.SetGlobalTR(stream, source.GetPosition(stream), source.GetRotation(stream), false);
            }
        }

        public void ProcessRootMotion(AnimationStream stream)
        {
        }
        

    }
}