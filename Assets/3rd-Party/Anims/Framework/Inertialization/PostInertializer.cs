﻿using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Framework
{
    [RequireComponent(typeof(Animator))]
    public class PostInertializer : MonoBehaviour
    {
        public enum EvaluateSpace
        {
            Local,
            Character,
            World
        }


        struct TransformState
        {
            public float3 PrevPosition;
            public float3 Position;

            public quaternion PrevRotation;
            public quaternion Rotation;

            public EvaluateSpace Space;
            public float BeginTime;
            public float EndTime;
        }


        private Animator _Animator;

        public Animator Animator
        {
            get
            {
                if (_Animator == null)
                    _Animator = GetComponent<Animator>();
                return _Animator;
            }
        }


        public AvatarMask AvatarMask;

        public float BlendTime = 0.5f;


        #region AvatarMaskBodyPart Mapping

        public static readonly Dictionary<AvatarMaskBodyPart, HumanBodyBones[]> AvatarBodyPartMapping =
            new Dictionary<AvatarMaskBodyPart, HumanBodyBones[]>()
            {
                {
                    AvatarMaskBodyPart.Body, new HumanBodyBones[]
                    {
                        HumanBodyBones.Chest,
                        HumanBodyBones.Hips,
                        HumanBodyBones.Spine,
                        HumanBodyBones.UpperChest
                    }
                },
                {
                    AvatarMaskBodyPart.Head, new HumanBodyBones[]
                    {
                        HumanBodyBones.Head,
                        HumanBodyBones.Neck,
                        HumanBodyBones.LeftEye,
                        HumanBodyBones.RightEye,
                        HumanBodyBones.Jaw
                    }
                },
                {
                    AvatarMaskBodyPart.LeftArm, new HumanBodyBones[]
                    {
                        HumanBodyBones.LeftUpperArm,
                        HumanBodyBones.LeftLowerArm,
                        HumanBodyBones.LeftHand
                    }
                },
                {
                    AvatarMaskBodyPart.RightArm, new HumanBodyBones[]
                    {
                        HumanBodyBones.RightUpperArm,
                        HumanBodyBones.RightLowerArm,
                        HumanBodyBones.RightHand
                    }
                },
                {
                    AvatarMaskBodyPart.LeftLeg, new HumanBodyBones[]
                    {
                        HumanBodyBones.LeftUpperLeg,
                        HumanBodyBones.LeftLowerLeg,
                        HumanBodyBones.LeftFoot,
                        HumanBodyBones.LeftToes
                    }
                },
                {
                    AvatarMaskBodyPart.RightLeg, new HumanBodyBones[]
                    {
                        HumanBodyBones.RightUpperLeg,
                        HumanBodyBones.RightLowerLeg,
                        HumanBodyBones.RightFoot,
                        HumanBodyBones.RightToes
                    }
                },
                {
                    AvatarMaskBodyPart.LeftFingers, new HumanBodyBones[]
                    {
                        HumanBodyBones.LeftThumbProximal,
                        HumanBodyBones.LeftThumbIntermediate,
                        HumanBodyBones.LeftThumbDistal,
                        HumanBodyBones.LeftIndexProximal,
                        HumanBodyBones.LeftIndexIntermediate,
                        HumanBodyBones.LeftIndexDistal,
                        HumanBodyBones.LeftMiddleProximal,
                        HumanBodyBones.LeftMiddleIntermediate,
                        HumanBodyBones.LeftMiddleDistal,
                        HumanBodyBones.LeftRingProximal,
                        HumanBodyBones.LeftRingIntermediate,
                        HumanBodyBones.LeftRingDistal,
                        HumanBodyBones.LeftLittleProximal,
                        HumanBodyBones.LeftLittleIntermediate,
                        HumanBodyBones.LeftLittleDistal
                    }
                },
                {
                    AvatarMaskBodyPart.RightFingers, new HumanBodyBones[]
                    {
                        HumanBodyBones.RightThumbProximal,
                        HumanBodyBones.RightThumbIntermediate,
                        HumanBodyBones.RightThumbDistal,
                        HumanBodyBones.RightIndexProximal,
                        HumanBodyBones.RightIndexIntermediate,
                        HumanBodyBones.RightIndexDistal,
                        HumanBodyBones.RightMiddleProximal,
                        HumanBodyBones.RightMiddleIntermediate,
                        HumanBodyBones.RightMiddleDistal,
                        HumanBodyBones.RightRingProximal,
                        HumanBodyBones.RightRingIntermediate,
                        HumanBodyBones.RightRingDistal,
                        HumanBodyBones.RightLittleProximal,
                        HumanBodyBones.RightLittleIntermediate,
                        HumanBodyBones.RightLittleDistal
                    }
                },
            };

        #endregion

        private readonly string fixedMaskPath = "BaseCharacterModel(Clone)";

        Transform[] CollectTransforms()
        {
            List<Transform> tempTrans = new List<Transform>();

            if (AvatarMask != null)
            {
                for (int i = 0; i < AvatarMask.transformCount; ++i)
                {
                    string path = AvatarMask.GetTransformPath(i);
                    var trans = transform.Find($"{fixedMaskPath}/{path}");
                    if (trans != null)
                        tempTrans.Add(trans);
                }
            }

            for (int i = 0; i < (int) AvatarMaskBodyPart.LastBodyPart; ++i)
            {
                var part = (AvatarMaskBodyPart) i;

                bool active = AvatarMask == null || AvatarMask.GetHumanoidBodyPartActive(part);

                if (!AvatarBodyPartMapping.TryGetValue(part, out var hbbones))
                    continue;

                foreach (var hbb in hbbones)
                {
                    var tempTran = Animator.GetBoneTransform(hbb);
                    if (tempTran == null)
                        continue;

                    if (active)
                        tempTrans.Add(tempTran);
                    else
                        tempTrans.Remove(tempTran);
                }
            }

            tempTrans.Remove(transform);

            var orderMapping = transform.GetComponentsInChildren<Transform>();

            List<Transform> result = new List<Transform>();

            foreach (var trans in orderMapping)
            {
                if (tempTrans.Find(x => x == trans))
                {
                    result.Add(trans);
                }
            }

            return result.ToArray();
        }

        class InertiaState
        {
            public Transform[] Transforms;
            public TransformState[] States;

            public float CurrTime;
            public float DeltaTime;

            public void Trigger(float blendTime)
            {
                for (int i = 0; i < Transforms.Length; ++i)
                {
                    Trigger(i, EvaluateSpace.Local, blendTime);
                }
            }

            public void Trigger(int index, EvaluateSpace space, float blendTime)
            {
                States[index].BeginTime = Time.time;
                States[index].EndTime = States[index].BeginTime + blendTime;
                States[index].Space = EvaluateSpace.Local;
            }

            public void LateUpdate()
            {
                float dt = max(1 / 120.0f, DeltaTime);

                for (int i = 0; i < Transforms.Length; ++i)
                {
                    //not inertia Root
                    if (i == 0)
                    {
                        continue;
                    }
                    
                    TransformState state = States[i];

                    float3 targetPos = Transforms[i].localPosition;
                    quaternion targetRot = Transforms[i].localRotation;

                    float tf = max(0.0001f, state.EndTime - CurrTime);
                    float3 pos =
                        InertializeModel.InertializeMagnitude(state.PrevPosition, state.Position, targetPos, dt, tf,
                            dt);
                    quaternion rot = InertializeModel.InertializeMagnitude(state.PrevRotation, state.Rotation,
                        targetRot, dt, tf, dt);

                    state.PrevPosition = state.Position;
                    state.PrevRotation = state.Rotation;
                    state.Position = pos;
                    state.Rotation = rot;

                    States[i] = state;
                    
                    Transforms[i].localPosition = pos;
                    Transforms[i].localRotation = rot;
                }
            }
        }

        InertiaState PostInertia = null;

        void InitializePostProcess()
        {
            PostInertia = new InertiaState();
            PostInertia.Transforms = CollectTransforms();

            List<TransformState> list = new List<TransformState>();
            
            foreach (var pt in PostInertia.Transforms)
            {
                var localPosition = pt.localPosition;
                var localRotation = pt.localRotation;
                
                list.Add(new TransformState
                {
                    Position = localPosition,
                    Rotation = localRotation,
                    PrevPosition = localPosition,
                    PrevRotation = localRotation
                });
            }

            PostInertia.States = list.ToArray();
        }

        private void LateUpdate()
        {
            if (PostInertia == null)
                InitializePostProcess();

            if (PostInertia != null)
            {
                PostInertia.CurrTime = Time.time;
                PostInertia.DeltaTime = Time.deltaTime;
                PostInertia.LateUpdate();
            }
        }

        public void Trigger()
        {
            PostInertia?.Trigger(BlendTime);
        }

        public void Trigger(float blendTime)
        {
            PostInertia?.Trigger(blendTime);
        }
    }
}