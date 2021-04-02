using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Unity.Collections;
using UnityEngine.Animations;

namespace Framework.AnimGraphs
{
    public class InertialBlendModule : IDisposable
    {
        private AnimationScriptPlayable intertialPlayable;

        private bool blending;
        private float remainingTime;

        private Animator animator;
        private AvatarMask avatarMask;

        private Transform[] skeletons;

        //Job Data
        private NativeArray<TransformStreamHandle> targetAnimationTransforms;
        private NativeArray<TransformSceneHandle> currentAnimationTransforms;
        private NativeArray<TransformData> previousAnimationTransforms;

        private AnimGraphPlayer player;
        
        public ref AnimationScriptPlayable Initialize(AnimGraphPlayer player)
        {
            this.player = player;

            this.animator = player.GetAnimator();
            this.avatarMask = player.GetBodyLayerMask();
            
            CollectAndBindAnimationTransforms(player.GetHeirachyRoot().name);
            
            var inertializerJob = new InertializerJob()
            {
                BlendActive = false,
                DeltaTime = Time.deltaTime,
                RemainingTime = 0f,
                TargetAnimationTransforms = targetAnimationTransforms,
                CurrentAnimationTransforms = currentAnimationTransforms,
                PreviousAnimationTransforms = previousAnimationTransforms
            };

            intertialPlayable = player.InsertOutputJob(inertializerJob);
            
            return ref intertialPlayable;
        }
        
        public ref AnimationScriptPlayable Initialize(Animator animator, AvatarMask avatarMask,
            PlayableGraph playableGraph, AnimationLayerMixerPlayable layerMixer)
        {
            this.animator = animator;
            this.avatarMask = avatarMask;

            CollectAndBindAnimationTransforms(null);

            var inertializerJob = new InertializerJob()
            {
                BlendActive = false,
                DeltaTime = Time.deltaTime,
                RemainingTime = 0f,
                TargetAnimationTransforms = targetAnimationTransforms,
                CurrentAnimationTransforms = currentAnimationTransforms,
                PreviousAnimationTransforms = previousAnimationTransforms
            };

            intertialPlayable = AnimationScriptPlayable.Create(playableGraph, inertializerJob);
            intertialPlayable.SetTraversalMode(PlayableTraversalMode.Mix);
            intertialPlayable.SetInputCount(1);
            intertialPlayable.SetInputWeight(0, 1f);
            intertialPlayable.SetProcessInputs(false);

            intertialPlayable.ConnectInput(0, layerMixer, 0);

            return ref intertialPlayable;
        }

        public void UpdateTransition()
        {
            if (!blending) return;
            
            var inertializerJob = intertialPlayable.GetJobData<InertializerJob>();

            float deltaTime = animator.updateMode == AnimatorUpdateMode.AnimatePhysics
                ? Time.fixedDeltaTime
                : Time.deltaTime;
            
            remainingTime -= deltaTime;

            if (remainingTime <= 0f)
            {
                blending = false;
                inertializerJob.BlendActive = false;
            }
            else
            {
                inertializerJob.BlendActive = true;
                inertializerJob.DeltaTime = deltaTime;
                inertializerJob.RemainingTime = remainingTime;
            }

            intertialPlayable.SetJobData(inertializerJob);
        }

        public void BeginTransition(float a_blendDuration)
        {
            remainingTime = a_blendDuration;
            blending = true;
        }
        
        private void CollectAndBindAnimationTransforms(string rootName)
        {
            List<Transform> tempList = new List<Transform>(60);

            if (animator.isHuman)
            {
                for (int i = 0; i < (int) HumanBodyBones.LastBone; ++i)
                {
                    var boneTransform = animator.GetBoneTransform((HumanBodyBones) i);

                    if (boneTransform != null)
                    {
                        tempList.Add(boneTransform);
                    }
                }
            }

            if (avatarMask != null)
            {
                //All non human bones
                for (int i = 0; i < avatarMask.transformCount; ++i)
                {
                    var jointTransformPath = avatarMask.GetTransformPath(i);
                    var jointTransform = rootName != null 
                        ? animator.transform.Find($"{rootName}/{jointTransformPath}") 
                        : animator.transform.Find(jointTransformPath);

                    if (jointTransform != null)
                    {
                        tempList.Add(jointTransform);
                    }
                }
            }

            skeletons = tempList.ToArray();

            int length = skeletons.Length;
            
            targetAnimationTransforms = new NativeArray<TransformStreamHandle>(length, Allocator.Persistent);
            currentAnimationTransforms = new NativeArray<TransformSceneHandle>(length, Allocator.Persistent);
            previousAnimationTransforms = new NativeArray<TransformData>(length, Allocator.Persistent);

            for (int i = 0; i < length; i++)
            {
                Transform tform = skeletons[i];

                targetAnimationTransforms[i] = animator.BindStreamTransform(tform);
                currentAnimationTransforms[i] = animator.BindSceneTransform(tform);
                previousAnimationTransforms[i] = new TransformData(tform.position, tform.rotation);
            }
        }

        public void Dispose()
        {
            if (targetAnimationTransforms.IsCreated)
                targetAnimationTransforms.Dispose();

            if (currentAnimationTransforms.IsCreated)
                currentAnimationTransforms.Dispose();

            if (previousAnimationTransforms.IsCreated)
                previousAnimationTransforms.Dispose();

            if (player != null)
            {
                MotionJobUtil.RemovePlayable(intertialPlayable);
            }
        }
    }
}