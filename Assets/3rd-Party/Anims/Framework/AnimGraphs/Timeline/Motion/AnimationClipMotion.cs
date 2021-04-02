using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed class AnimationClipMotion : Motion
    {
        private AnimationClipPlayable clipPlayable;
        private AnimationClipAsset motion;

        public override bool isLooping => motion.isLooping;
        public override uint duration { get; protected set; }

        private float defaultDuration;
        private float timeScale;

        public int AnimatinClipHash { get { return motion.AnimationClipHash; } }

        public override void Preparatory(ref MotionAsset motionAsset, float timeScale)
        {
            motion = *motionAsset.CastStruct<AnimationClipAsset>();
            this.timeScale = timeScale;
        }

        public override void Connect()
        {
            var animationClip = motion.asset(player.OverrideType);

            if (animationClip == null)
            {
                Debug.LogError($"Can't find Clip Hash {motion.AnimationClipHash}'");
                return;
            }
            
            // 算出动画时长,并记录
            defaultDuration = animationClip.length;
            duration = (animationClip.length * (1 / timeScale)).ToUintTime();

            clipPlayable = AnimationClipPlayable.Create(parent.GetGraph(),animationClip);
            clipPlayable.SetApplyPlayableIK(true);
            parent.ConnectInput(inputPort, clipPlayable, 0);
        }

        public override void Disconnected()
        {
            if (!clipPlayable.IsNull() && clipPlayable.IsValid())
            {
                if (!parent.IsNull() && parent.IsValid())
                    parent.DisconnectInput(inputPort);
                clipPlayable.Destroy();
                if(AnimGraphLoader.UnloadAnimationClip != null)
                {
                    AnimGraphLoader.UnloadAnimationClip(motion.assetValue, player.OverrideType);
                }
            }
        }

        public override void SetFootIK(bool useFootIK)
        {
            clipPlayable.SetApplyFootIK(useFootIK);
        }

        public override void Update(float normalTime,int loopCount)
        {
            if (player == null)
                return;
            // 播放速度要进行一次修正
            clipPlayable.SetTime(loopCount * defaultDuration + normalTime * defaultDuration);
        }

        public override void Dispose()
        {
            Disconnected();

            motion = default;
            clipPlayable = default;
            parent = default;
            inputPort = -1;
        }

        public override void GetWeights(float parentWeight, int stateName, List<AnimationClipWeight> weights)
        {
            AnimationClipWeight w = new AnimationClipWeight();
            w.animationClipHash = motion.NameHash;
            w.stateName = stateName;
            w.weight = parentWeight * GetWeight();
            weights.Add(w);
        }

        
    }
}