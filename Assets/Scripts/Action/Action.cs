using System;
using System.Collections.Generic;
using MoonSharp.Interpreter.Compatibility;
using UnityEngine;

namespace ASeKi.action
{
    [Serializable]
    public class Action
    {
        public string m_ActionName = "";
        public int ActionId = 0;
        public int m_ConnectingState = -1;
        public int m_Layer = 0;
        public int LayerBlendMode = 0; //0 is override, 1 is Addtive
        public int m_TotalFrame = 0;
        public float m_TotalTime = 0;
        public float m_FrameRate = 30;
        public ActionType m_ActionType = ActionType.IDLE;
        public WrapMode m_Mode = WrapMode.Default;

        //important
        public int ActionState = 0;

        // About Weapon
        public WeaponType ActionWeaponType = WeaponType.None;

        // time flag
        public List<ActionTimeStateSetting> m_StateFrameList = new List<ActionTimeStateSetting>();
        public List<ActionTimeStateSwitchSetting> ActionTimeStateSwitchList = new List<ActionTimeStateSwitchSetting>();
        
        public List<string> m_ActionClipName = new List<string>();

#if UNITY_EDITOR
        public List<AnimationClip> m_ActionClip = new List<AnimationClip>();
#endif

        #region Curve            

        public List<AnimationCurve> DisplacementCurveXList = new List<AnimationCurve>();
        public List<AnimationCurve> DisplacementCurveYList = new List<AnimationCurve>();
        public List<AnimationCurve> DisplacementCurveZList = new List<AnimationCurve>();
        public List<AnimationCurve> EulerXList = new List<AnimationCurve>();
        public List<AnimationCurve> EulerYList = new List<AnimationCurve>();
        public List<AnimationCurve> EulerZList = new List<AnimationCurve>();

//#if UNITY_EDITOR
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
//#endif

        #endregion

        public List<LookatFrame> m_ListLookatFrame = new List<LookatFrame>();
        public float m_EndMoveRatio = 1.0f;

        // About Next Action & Condition
        public List<NextAction> NextStateList = new List<NextAction>();

        // About Camera
        public bool IsUsingUnlockCamera = false;
        public float UnlockCameraStartFrame = 0.0f; // 頭尾皆為0則表示全動作
        public float UnlockCameraEndFrame = 0.0f; // 頭尾皆為0則表示全動作

        // About Fly Action
        public bool IsUsingFlyAction = false;
        public float FlyActionStartFrame = 0.0f;
        public float FlyActionEndFrame = 0.0f;
        public float FlyMinHeight = 0f;
        public float FlyMaxHeight = 0f;
        public float FixHeightStartTime = 0f;
        public float FixHeightEndTime = 0f;
        public float PosFixValue = 0f;

        // About Apply Rotate
        public bool IsAllowRotate = false;
        public AllowRotateType rotateType = AllowRotateType.ART_MOTION;
        public List<AllowRotateData> AllowRotateFrameList = new List<AllowRotateData>();

        // About Apply Move By InputAxis
        public bool IsAllowMoveByInputAxis = false;
        public List<AllowMoveData> AllowMoveList = new List<AllowMoveData>();

        public ActionStaminaSetting Stamina = new ActionStaminaSetting();

        public string TransitionGroup = "";

        public bool bArmed = false;
        public float ArmedTimeRate = 0f;
        public Vector3 ArmedAngle = Vector3.zero;

        public bool bUnArmed = false;
        public float UnArmedTimeRate = 0f;
        public Vector3 UnArmedAngle = Vector3.zero;

        public bool IsUseWeaponAction = false;
        public List<WeaponActionData> WeaponActionDatas = new List<WeaponActionData>();

        //StateFlags
        public List<ActionStateFlagData> StateFlagDatas = new List<ActionStateFlagData>();

        public bool IsAllowAddtive = false;
        public bool IsAllowIK = false;

        public float Speed = 1f;


#if UNITY_EDITOR
        public void InitAnimationCurve()
        {
            //ResetAnimationCurve();

            int num = m_ActionClip.Count;
            for (int i = 0; i < num; ++i)
            {
                if (FreezePosXList.Count < num) FreezePosXList.Add(false);
                if (FreezePosYList.Count < num) FreezePosYList.Add(true); //位移不用管Y軸
                if (FreezePosZList.Count < num) FreezePosZList.Add(false);

                if (DisplacementCurveXList.Count < num) DisplacementCurveXList.Add(new AnimationCurve());
                if (DisplacementCurveYList.Count < num) DisplacementCurveYList.Add(new AnimationCurve());
                if (DisplacementCurveZList.Count < num) DisplacementCurveZList.Add(new AnimationCurve());

                if (FreezeRotXList.Count < num) FreezeRotXList.Add(true); //大部份動作不用管旋轉
                if (FreezeRotYList.Count < num) FreezeRotYList.Add(true); //大部份動作不用管旋轉 (少部份動作旋轉也只是轉Y軸)
                if (FreezeRotZList.Count < num) FreezeRotZList.Add(true); //大部份動作不用管旋轉

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

                if (IsUsingMotionTQList.Count < num) IsUsingMotionTQList.Add(true); //20200731 安國說Default勾上
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

        #region API

        public float GetTimeRate(ActionTimeState s)
        {
            int num = m_StateFrameList.Count;
            if (num > 0)
            {
                for (int i = 0; i < num; ++i)
                {
                    if (m_StateFrameList[i].m_State == s)
                    {
                        return m_StateFrameList[i].m_Rate;
                    }
                }
            }

            return 1.0f;
        }

        public float GetSwitchTime(ActionType nextType)
        {
            int num = ActionTimeStateSwitchList.Count;
            if (num > 0)
            {
                for (int i = 0; i < num; ++i)
                {
                    if (ActionTimeStateSwitchList[i].NextType == nextType)
                    {
                        if (m_TotalFrame > 0)
                        {
                            //return (float)ActionTimeStateSwitchList[i].Frame / (float)m_TotalFrame;
                            return Mathf.Clamp((float) ActionTimeStateSwitchList[i].Frame / (float) m_TotalFrame, 0.1f, 1.0f);
                        }
                    }
                }
            }

            return 1.0f;
        }

        public ActionTimeState GetActionTimeState(float ratio)
        {
            //m_StateFrameList.Sort((x, y) => { return x.m_Rate.CompareTo(y.m_Rate); });

            ActionTimeState result = ActionTimeState.BEGIN;
            int num = m_StateFrameList.Count;
            if (num > 0)
            {
                for (int i = 0; i < num; ++i)
                {
                    if (m_StateFrameList[i].m_Rate > ratio)
                    {
                        break;
                    }

                    result = m_StateFrameList[i].m_State;
                }
            }

            return result;
        }

        public bool IsExistsNextActionID(int targetAcionID)
        {
            return NextStateList.Exists(x => x.Id == targetAcionID);
        }

        int getProtectedRatio(ActionTimeState timeState)
        {
            int num = m_StateFrameList.Count;
            for (int i = 0; i < num; ++i)
            {
                if (m_StateFrameList[i].m_State == timeState)
                {
                    return (int) (m_StateFrameList[i].m_Rate * m_TotalFrame);
                }
            }

            return 0;
        }

        #endregion
    }
    
}