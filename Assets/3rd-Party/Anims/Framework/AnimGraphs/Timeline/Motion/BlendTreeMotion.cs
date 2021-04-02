using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    /// <summary>
    /// BlendTree类型的动画单元
    /// </summary>
    public unsafe sealed partial class BlendTreeMotion : Motion
    {
        private BlendTreeAsset motion;
        private NArray<ChildMotionAsset> motionChilds;

        public override bool isLooping => motion.isLooping;
        public override uint duration { get; protected set; }
        private float defaultDuration;

        private AnimationMixerPlayable blend;
        private NativeArray<GCHandle> motions;

        private int2 paramIndex;
        private float2 blendParams;
        private float timeScale;

        public override void Preparatory(ref MotionAsset motionAsset,float timeScale)
        {
            motion = *motionAsset.CastStruct<BlendTreeAsset>();
            this.timeScale = timeScale;

            switch (motion.blendType)
            {
                case BlendTreeType.Simple1D:
                    paramIndex.x = motion.blendParameterX;
                    break;
                case BlendTreeType.FreeformCartesian2D:
                    paramIndex.x = motion.blendParameterX;
                    paramIndex.y = motion.blendParameterY;
                    break;
                default:
                    throw new System.Exception(string.Format("BlendTreeMotion中的{0}未开发!", motion.blendType));
            }

            motionChilds = motion.childMotions;
            motions = new NativeArray<GCHandle>(motionChilds.Length, Allocator.Persistent);

            for (int i = 0; i < motionChilds.Length; i++)
            {
                var child = motionChilds[i];
                var motionChild = child.motion;
                if (motionChild.isNull)
                {
                    Debug.LogError($"motionChild 配置为null");
                    motions[i] = GCHandle.Alloc(null);
                    continue;
                }
                Motion m = null;
                switch (motionChild.motionType)
                {
                    case MotionType.AnimationClip:
                        m = new AnimationClipMotion();
                        break;
                    case MotionType.BlendTree:
                        m = new BlendTreeMotion();
                        break;
                }
                motions[i] = GCHandle.Alloc(m);
                m.Preparatory(ref motionChild, child.timeScale * timeScale);
            }
        }

        public override void SetFootIK(bool useFootIK)
        {
            for (int i = 0; i < motions.Length; i++)
            {
                Motion m = motions[i].Target as Motion;
                if (m == null)
                    continue;
                m.SetFootIK(useFootIK);
            }
        }

        public override void Connect()
        {
            PlayableGraph m_Graph = parent.GetGraph();

            blend = AnimationMixerPlayable.Create(m_Graph, motions.Length);

            for (int i = 0; i < motions.Length; i++)
            {
                Motion m = motions[i].Target as Motion;
                if (m == null)
                    continue;
                m.Connecting(player, blend, i);
            }
            parent.ConnectInput(inputPort, blend, 0);

            BlendUpdate();
            DurationUpdate();
        }

        public override void Disconnected()
        {
            if (blend.IsValid())
            {
                if (motions.IsCreated)
                {
                    for (int i = 0; i < motions.Length; i++)
                    {
                        var handle = motions[i];
                        if (handle.Target != null)
                        {
                            Motion m = handle.Target as Motion;
                            m.Disconnected();
                        }
                    }
                }
                if (parent.IsValid())
                    parent.DisconnectInput(inputPort);
                blend.Destroy();
            }
        }

        public override void Dispose()
        {
            Disconnected();

            if (motions.IsCreated)
            {
                for (int i = 0; i < motions.Length; i++)
                {
                    var handle = motions[i];
                    if (handle.Target != null)
                    {
                        Motion m = handle.Target as Motion;
                        m.Dispose();
                    }
                }
                motions.Dispose();
            }

            motionChilds = default;
        }

        public override void Update(float normalTime, int loopCount)
        {
            if (player == null)
                return;
            
            BlendUpdate();
            DurationUpdate();

            if (motions.IsCreated)
            {
                for (int i = 0; i < motions.Length; i++)
                {
                    var m = motions[i].Target as Motion;
                    if (m == null)
                        continue;
                    m.Update(normalTime, loopCount);
                }
            }
        }

        private void DurationUpdate()
        {
            if (motions.IsCreated)
            {
                float durationWeight = 0;
                float duration = 0;
                for (int i = 0; i < motions.Length; i++)
                {
                    var m = motions[i].Target as Motion;
                    if (m == null) continue;
                    duration += m.duration.ToFloatTime() * m.GetWeight();
                    durationWeight += m.GetWeight();
                }

                if(durationWeight != 0 && durationWeight < 1)
                {
                    duration = duration / durationWeight;
                }

                this.duration = duration.ToUintTime();
            }
        }

        private void BlendUpdate()
        {
            if (motions.IsCreated)
            {
                for (int i = 0; i < motions.Length; i++)
                {
                    var m = motions[i].Target as BlendTreeMotion;
                    if (m == null) continue;
                    m.BlendUpdate();
                }
            }

            switch (motion.blendType) {
                case BlendTreeType.Simple1D:
                    blendParams.x = player.GetFloat(paramIndex.x);
                    CalculateWeights1d();
                    break;
                case BlendTreeType.FreeformCartesian2D:
                    blendParams.x = player.GetFloat(paramIndex.x);
                    blendParams.y = player.GetFloat(paramIndex.y);
                    CalculateWeightsFreeformCartesian();
                    break;
                default:
                    throw new System.Exception(string.Format("BlendTreeMotion中的{0}未开发!", motion.blendType));
            }
        }

        public override void GetWeights(float parentWeight, int stateName, List<AnimationClipWeight> weights)
        {
            if (motions.IsCreated)
            {
                var weight = GetWeight();
                for (int i = 0; i < motions.Length; i++)
                {
                    var m = motions[i].Target as Motion;
                    if (m == null)
                        continue;
                    m.GetWeights(weight, stateName, weights);
                }
            }
        }

#if UNITY_EDITOR
        ~BlendTreeMotion()
        {
            if (motions.IsCreated)
                Debug.LogError("未正确回收Native内存");
        }
#endif
    }
}