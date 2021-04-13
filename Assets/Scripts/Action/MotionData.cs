using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

// 临时存储的动作资源，之后要转化为游戏实际使用的RuntimeAction资源
namespace ASeKi.action
{
    [Serializable]
    public class MotionData
    {
        public int ActionNameHash;            // 动作哈希值
        public string ActionName = "";        
        public int ActionId;
        public int Layer;
        public int LayerBlendMode;
        public int TotalFrame;
        public float TotalTime ;
        public ActionType ActionType = ActionType.IDLE;
        public WrapMode Mode = WrapMode.Default;

        // Import
        public int ActionState = 0;

        // About Weapon
        public WeaponType WeaponType = WeaponType.None;

        // Time flag
        public List<ActionTimeStateSetting> StateFrameList = new List<ActionTimeStateSetting>();    // 动作帧当前阶段
        public List<ActionTimeStateSwitchSetting> ActionTimeStateSwitchList = new List<ActionTimeStateSwitchSetting>();    // 动作帧当前状态

        public List<string> ActionClipNameList = new List<string>();
        public List<int> ActionClipNameHash = new List<int>();

#if UNITY_EDITOR
        public List<AnimationClip> ActionClipList = new List<AnimationClip>();
        
        public void InitAnimationCurve()
        {
            //ResetAnimationCurve();
            int num = ActionClipList.Count;
            for (int i = 0; i < num; ++i)
            {
                if (FreezePosXList.Count < num) FreezePosXList.Add(false);
                if (FreezePosYList.Count < num) FreezePosYList.Add(true);    // 位移不用管Y軸
                if (FreezePosZList.Count < num) FreezePosZList.Add(false);

                if (DisplacementCurveXList.Count < num) DisplacementCurveXList.Add(new AnimationCurve());
                if (DisplacementCurveYList.Count < num) DisplacementCurveYList.Add(new AnimationCurve());
                if (DisplacementCurveZList.Count < num) DisplacementCurveZList.Add(new AnimationCurve());

                if (FreezeRotXList.Count < num) FreezeRotXList.Add(true);   // 大部份動作不用管旋轉
                if (FreezeRotYList.Count < num) FreezeRotYList.Add(true);   // 大部份動作不用管旋轉 (少部份動作旋轉也只是轉Y軸)
                if (FreezeRotZList.Count < num) FreezeRotZList.Add(true);   // 大部份動作不用管旋轉

                if (QuaternionCurveXList.Count < num) QuaternionCurveXList.Add(new AnimationCurve());
                if (QuaternionCurveYList.Count < num) QuaternionCurveYList.Add(new AnimationCurve());
                if (QuaternionCurveZList.Count < num) QuaternionCurveZList.Add(new AnimationCurve());
                if (QuaternionCurveWList.Count < num) QuaternionCurveWList.Add(new AnimationCurve());

                if (EulerXList.Count < num) EulerXList.Add(new AnimationCurve());
                if (EulerYList.Count < num) EulerYList.Add(new AnimationCurve());
                if (EulerZList.Count < num) EulerZList.Add(new AnimationCurve());

                if (LockCurvePosXList.Count < num) LockCurvePosXList.Add(false);
                if (LockCurvePosYList.Count < num) LockCurvePosYList.Add(false);
                if (LockCurvePosZList.Count < num) LockCurvePosZList.Add(false);
                if (LockCurveRotXList.Count < num) LockCurveRotXList.Add(false);
                if (LockCurveRotYList.Count < num) LockCurveRotYList.Add(false);
                if (LockCurveRotZList.Count < num) LockCurveRotZList.Add(false);

                if (IsUsingMotionTQList.Count < num) IsUsingMotionTQList.Add(true);    //20200731 安國說Default勾上
            }
        }

        public void ResetAnimationCurve()
        {
            FreezePosXList.Clear();
            FreezePosYList.Clear();
            FreezePosZList.Clear();

            DisplacementCurveXList.Clear();
            DisplacementCurveYList.Clear();
            DisplacementCurveZList.Clear();

            FreezeRotXList.Clear();
            FreezeRotYList.Clear();
            FreezeRotZList.Clear();

            QuaternionCurveXList.Clear();
            QuaternionCurveYList.Clear();
            QuaternionCurveZList.Clear();
            QuaternionCurveWList.Clear();

            EulerXList.Clear();
            EulerYList.Clear();
            EulerZList.Clear();

            LockCurvePosXList.Clear();
            LockCurvePosYList.Clear();
            LockCurvePosZList.Clear();
            LockCurveRotXList.Clear();
            LockCurveRotYList.Clear();
            LockCurveRotZList.Clear();

            IsUsingMotionTQList.Clear();
        }
#endif

        #region Curve            

        public List<AnimationCurve> DisplacementCurveXList = new List<AnimationCurve>();
        public List<AnimationCurve> DisplacementCurveYList = new List<AnimationCurve>();
        public List<AnimationCurve> DisplacementCurveZList = new List<AnimationCurve>();
        public List<AnimationCurve> EulerXList = new List<AnimationCurve>();
        public List<AnimationCurve> EulerYList = new List<AnimationCurve>();
        public List<AnimationCurve> EulerZList = new List<AnimationCurve>();

        public List<bool> FreezePosXList = new List<bool>();
        public List<bool> FreezePosYList = new List<bool>();
        public List<bool> FreezePosZList = new List<bool>();
        public List<bool> FreezeRotXList = new List<bool>();
        public List<bool> FreezeRotYList = new List<bool>();
        public List<bool> FreezeRotZList = new List<bool>();

        public List<AnimationCurve> QuaternionCurveXList = new List<AnimationCurve>();
        public List<AnimationCurve> QuaternionCurveYList = new List<AnimationCurve>();
        public List<AnimationCurve> QuaternionCurveZList = new List<AnimationCurve>();
        public List<AnimationCurve> QuaternionCurveWList = new List<AnimationCurve>();

        public List<bool> LockCurvePosXList = new List<bool>();
        public List<bool> LockCurvePosYList = new List<bool>();
        public List<bool> LockCurvePosZList = new List<bool>();
        public List<bool> LockCurveRotXList = new List<bool>();
        public List<bool> LockCurveRotYList = new List<bool>();
        public List<bool> LockCurveRotZList = new List<bool>();

        public List<bool> IsUsingMotionTQList = new List<bool>();

        #endregion
        
        // About Previous/Next Action & Condition
        public PreviousMotion PreviousMotion = new PreviousMotion();
        public List<NextMotion> NextMotionList = new List<NextMotion>();
        
        // (Move) About Apply Rotate
        public bool IsAllowRotate = false;
        public AllowRotateType RotateType = AllowRotateType.ART_MOTION;
        public List<AllowRotateData> AllowRotateFrameList = new List<AllowRotateData>();

        // (Move) About Apply Move By InputAxis
        public bool IsAllowMoveByInputAxis = false;
        public List<AllowMoveData> AllowMoveList = new List<AllowMoveData>();

        public bool IsAllowAssistMove = false;
        public List<AssistMoveData> AssistMoveDatas = new List<AssistMoveData>();

        // (Position) About Fly Action
        public bool IsUsingFlyAction = false;
        public float FlyActionStartFrame = 0.0f;
        public float FlyActionEndFrame = 0.0f;
        public float FlyMinHeight = 0f;
        public float FlyMaxHeight = 0f;
        public float FixHeightStartTime = 0f;
        public float FixHeightEndTime = 0f;
        public float PosFixValue = 0f;

        // (Weapon Animation) Weapon Action Setting
        public bool IsArmed = false;
        public float ArmedTimeRate = 0f;
        public Vector3 ArmedAngle = Vector3.zero;

        public bool IsUnArmed = false;
        public float UnArmedTimeRate = 0f;
        public Vector3 UnArmedAngle = Vector3.zero;

        public bool IsUseWeaponAction = false;
        public List<WeaponActionData> WeaponActionDatas = new List<WeaponActionData>();

        // (Other) About LookAt
        public List<LookatFrame> LookatFrameList = new List<LookatFrame>();

        // (Other) About Camera
        public bool IsUsingUnlockCamera = false;
        public float UnlockCameraStartFrame = 0.0f; // 頭尾皆為0則表示全動作
        public float UnlockCameraEndFrame = 0.0f; // 頭尾皆為0則表示全動作

        // (Other) Stamina
        public ActionStaminaSetting Stamina = new ActionStaminaSetting();

        // (Other) StateFlags
        public List<ActionStateFlagData> StateFlagDatas = new List<ActionStateFlagData>();

        // (Other) additive
        public bool IsAllowAdditive = false;

        [FormerlySerializedAs("IsAllowIK")] public bool IsAllowHandIK = false;
        public List<IKHandFrameData> IkHandFrameDatas = new List<IKHandFrameData>();

        public bool IsAllowFootIK = false;
        public List<IKFootFrameData> IkFootFrameDatas = new List<IKFootFrameData>();

        //new
        public int ActionGroup = -1;

        public List<SyncMarker> SyncMarkers = new List<SyncMarker>();
    }
}