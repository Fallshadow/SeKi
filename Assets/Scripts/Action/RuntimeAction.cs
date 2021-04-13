using System;
using System.Collections.Generic;
using ASeKi.debug;
using Framework.AnimGraphs;
using MoonSharp.Interpreter.Compatibility;
using Unity.Collections;
using UnityEngine;

// 运行状态的动作数据 使用结构体
namespace ASeKi.action
{
    
    #region 动作帧 - 状态/阶段 信息

    public struct ActionTimeStateStruct
    {
        public ActionTimeState State;
        public float Rate;
    }
    
    public struct ActionTimeStateSwitchStruct
    {
        public ActionType NextType;
        public int Frame;
    }

    #endregion

    #region 武器动作帧 - 状态枚举 信息

    public struct WeaponActionStruct
    {
        public int TriggerFrame;
        public action.WeaponActionState WeaponActionState;
    }

    #endregion
        
    #region 动作条件

    public struct MotionSpecialConditionData
    {
        // public NativeList<SpeicalConditionData> SpecialConditionList ;
        public ChunkData SpecialConditionChunck;
    }

    public struct MotionConditionData
    {
        public bool IsUseButtonCondition;
        public ButtonConditionData ButtonCondition;

        public ChunkData ConditionChunck;
        // public NativeList<TriggerConditionData> ConditionList;
    }

    public struct ButtonConditionData
    {
        public input.ButtonType ButtonType;
        public input.PressType ButtonPressType;
    }

    public struct TriggerConditionData
    {
        public int ConditionName;
        public float ConditionValue;
        public ValueConditionType ConditionType;
    }

    public struct SpecialConditionData
    {
        public ActionComboSpecialType Type;
        public ActionType ActionType;
        public float ActionTimeStartFrame;
        public float ActionTimeEndFrame;
        public ActionComboResourceValueType ResourceValueType;
        public ActionComboResourceType ResourceType;
        public ValueConditionType ResourceConditionType;
        public float ResourceValue;
    }

    #endregion

    #region 旋转

    public struct AllowRotateStruct
    {
        public Vector2 AllowRotateLimit;
        public int BeginFrame;
        public int EndFrame;
        public float LimitAnglePerFrame;
            
        public bool IsAllowAssistingRotate;
        public float AssistingAngle;
        public float AssistingRotateDistance;
    }

    #endregion

    #region 移动

    public struct AllowMoveStruct
    {
        public int BeginFrame;
        public int EndFrame;
        public float MoveSpeed;
        public bool NeedAssisting;
        public float AssistingDistance;
    }
        
    public struct AssistMoveDataStruct
    {
        public int BeginFrame;
        public int EndFrame;
        public float AssistMoveSpeed;
        public float AssistingDistance;
    }

    #endregion

    #region 注视

    public struct LookatFrameStruct
    {
        public float BeginRatio;
        public float EndRatio;
    }

    #endregion

    #region 耐力
    public struct ActionStaminaStruct
    {
        public int StaminaBeginFrame;
        public int StaminaConsumpValue;
        public float LoopTime;
        public float LifeTime;
        public StaminaConsumptionType StaminaConsumpType;
    }
    #endregion

    #region IK
    
    public struct IKFrameDatastruct
    {
        public int FrameKey;
        public bool IsIKOn;
    }

    #endregion
    
    public struct ActionStateFlagStruct
    {
        public FixedString64 StateFlag;
        public int StartFrame;
        public int EndFrame;
    }
    
    public struct SyncMarkerData
    {
        public int MarkerHash;
        public List<float> markTimes;
        //public string MarkerName;
        //public ChunkData TargetTimeChunk;
    }
    
    public static class ComboUtility
    {
        //TODO 搬移
        static string[] spcArray = { "SpecialCondition1", "SpecialCondition2", "SpecialCondition3", "SpecialCondition4", "SpecialCondition5" };

        public static bool MatchSPConditionName(string name)
        {
            return Array.Exists(spcArray, element => element == name);
        }

        public static string GetSPConditionString(int i)
        {
            switch (i)
            {
                case 1:
                    return "SpecialCondition1";
                    break;
                case 2:
                    return "SpecialCondition2";
                    break;
                case 3:
                    return "SpecialCondition3";
                    break;
                case 4:
                    return "SpecialCondition4";
                    break;
                case 5:
                    return "SpecialCondition5";
                    break;
            }

            PrintSystem.LogError("[ComboManager] Special Condition String Not Find!");
            return "";
        }

    }
    
    #region 上一个/下一个 动作
    
    [Serializable]
    public class NextActionCondition
    {
        public bool IsButtonTrigger = false;
        public List<ButtonCondition> ButtonList = new List<ButtonCondition>();
        public bool IsConditionTrigger = false;
        public List<TriggerCondition> ConditionList = new List<TriggerCondition>();
        public List<TriggerCondition> ConditionList2 = new List<TriggerCondition>();

        public void RemoveAllSpeicalCondition()
        {
            if (IsConditionTrigger)
            {
                List<TriggerCondition> RecordList = new List<TriggerCondition>();
                int num = ConditionList.Count;
                for (int i = 0; i < num; i++)
                {
                    if (!ComboUtility.MatchSPConditionName(ConditionList[i].ConditionName))
                    {
                        RecordList.Add(ConditionList[i]);
                    }
                }
                ConditionList = RecordList;

                List<TriggerCondition> RecordList2 = new List<TriggerCondition>();
                num = ConditionList2.Count;
                for (int i = 0; i < num; i++)
                {
                    if (!ComboUtility.MatchSPConditionName(ConditionList2[i].ConditionName))
                    {
                        RecordList2.Add(ConditionList2[i]);
                    }
                }
                ConditionList2 = RecordList2;
            }
        }
    }
    
    [Serializable]
    public class NextAction
    {
        public int Id = 0;
        public NextActionCondition Condition = new NextActionCondition();
        public bool IsBtnAndTriggerConditionAnd = true;
        public bool IsLockApplyToAllNextState = false;

        public void Reset()
        {
            int num = Condition.ConditionList.Count;
            for (int i = 0; i < num; ++i)
            {
                TriggerCondition c = Condition.ConditionList[i];
                c.IsTrigger = false;
            }

            num = Condition.ConditionList2.Count;
            for (int i = 0; i < num; ++i)
            {
                TriggerCondition c = Condition.ConditionList2[i];
                c.IsTrigger = false;
            }
        }

        public bool IsTriggerByCondition()
        {
            bool isTrigger = true;
            if (Condition.IsConditionTrigger)
            {
                int num = Condition.ConditionList.Count;
                int num2 = Condition.ConditionList2.Count;
                if (num > 0) //避免策劃填錯沒填到第一個ConditionList而直接填第二個
                {
                    for (int i = 0; i < num; ++i)
                    {
                        TriggerCondition c = Condition.ConditionList[i];
                        if (!c.IsTrigger)
                        {
                            isTrigger = false;
                            break;
                        }
                    }
                }
                else if (num2 > 0)
                {
                    isTrigger = false;
                }
                if (!isTrigger)
                {
                    if (num2 > 0)
                    {
                        isTrigger = true;
                        for (int i = 0; i < num2; ++i)
                        {
                            TriggerCondition c = Condition.ConditionList2[i];
                            if (!c.IsTrigger)
                            {
                                isTrigger = false;
                                break;
                            }
                        }
                    }
                }
                return isTrigger;
            }
            return false;
        }

        public bool IsTriggerByButton(input.ButtonType button, input.PressType pressType)
        {
            if (Condition.IsButtonTrigger)
            {
                for (int i = 0; i < Condition.ButtonList.Count; ++i)
                {
                    if (Condition.ButtonList[i].ButtonType == button && Condition.ButtonList[i].ButtonPressType == pressType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public NextAction Clone()
        {
            //TODO: 
            return this;
        }
    }

    #endregion
    
    public struct PreviousMotionData
    {
        public ChunkData PreviousMotionConditionChunk;

        //public NativeList<MotionConditionData> PreviousMotionConditionList;            
        public MotionSpecialConditionData SpecialCondition;
        public ActionRoleType MotionRoleType;
    }

    public struct NextMotionData
    {
        public int Id;

        public ChunkData NextMotionConditionChunck;

        //public NativeList<MotionConditionData> NextMotionConditionList; //Trigger 1 , 2, 3 ... 
        public MotionSpecialConditionData SpecialCondition;

        public ActionRoleType MotionRoleType;
        //public bool IsLockApplyToAllNextState = false;
    }

    /// <summary>
    /// 新的Action資料結構, 在 Runtime 時使用 (會將原先的 Action 或新的 Motion 做實例)
    /// </summary>
    /// 使用struct 為了未來的 Job System [BurstCompile]
    public struct RuntimeAction : IDisposable
    {
        public bool IsInitialized { get; private set; }
        public int ActionNameHash;
        public int ActionId;
        public int Layer;
        public int LayerBlendMode;
        public int TotalFrame;
        public float TotalTime;
        public ActionType ActionType;
        public WrapMode Mode;

        // only for debug use
        // TODO find other way to debug these!
        public List<string> ActionClipName;
        public NativeList<int> ActionClipHash;

        // time flag
        public NativeList<ActionTimeStateStruct> StateFrameList;
        public NativeList<ActionTimeStateSwitchStruct> ActionTimeStateSwitchList;

        public int ActionState;

        // About Weapon
        public WeaponType ActionWeaponType;

        //TODO: BurstCompile
        public List<AnimationCurve> DisplacementCurveXList;
        public List<AnimationCurve> DisplacementCurveYList;
        public List<AnimationCurve> DisplacementCurveZList;
        public List<AnimationCurve> EulerXList;
        public List<AnimationCurve> EulerYList;
        public List<AnimationCurve> EulerZList;
        public List<AnimationCurve> EulerWList;

        public NativeList<bool> FreezePosXList;
        public NativeList<bool> FreezePosYList;
        public NativeList<bool> FreezePosZList;
        public NativeList<bool> FreezeRotXList;
        public NativeList<bool> FreezeRotYList;
        public NativeList<bool> FreezeRotZList;

        public NativeList<LookatFrameStruct> LookatFrameList;

        // About Next Action & Condition
        public NativeList<int> NextActionIdList { get; private set; }
        public List<NextAction> NextStateList;
        public NativeList<NextMotionData> NextMotionList;

        public FixedChunkNativeArray<MotionConditionData> ConditionChuncks;
        public FixedChunkNativeArray<TriggerConditionData> TriggerChuncks;
        public FixedChunkNativeArray<SpecialConditionData> SpecialConditionChuncks;

        // About Camera
        public bool IsUsingUnlockCamera;
        public float UnlockCameraStartFrame;
        public float UnlockCameraEndFrame;

        // About Fly Action
        public bool IsUsingFlyAction;
        public float FlyActionStartFrame;
        public float FlyActionEndFrame;
        public float FlyMinHeight;
        public float FlyMaxHeight;
        public float FixHeightStartTime;
        public float FixHeightEndTime;
        public float PosFixValue;

        // About Apply Rotate
        public bool IsAllowRotate;
        public AllowRotateType RotateType;
        public NativeList<AllowRotateStruct> AllowRotateFrameList;

        // About Apply Move By InputAxis
        public bool IsAllowMoveByInputAxis;
        public NativeList<AllowMoveStruct> AllowMoveList;

        public bool IsAllowAssistMove;
        public NativeList<AssistMoveDataStruct> AssistMoveDataList;

        public ActionStaminaStruct Stamina;

        // (Weapon Animation) Weapon Action Setting
        public bool IsArmed;
        public float ArmedTimeRate;
        public Vector3 ArmedAngle;

        public bool IsUnArmed;
        public float UnArmedTimeRate;
        public Vector3 UnArmedAngle;

        public bool IsUseWeaponAction;
        public NativeList<WeaponActionStruct> WeaponActionDatas;

        public List<ActionStateFlagStruct> StateFlagDatas;

        public bool IsAllowAdditive;

        public bool IsAllowHandIK;
        public bool IsAllowFootIK;

        public NativeList<IKFrameDatastruct> IkHandFrameDatas;
        public NativeList<IKFrameDatastruct> IkFootFrameDatas;

        //Other
        public int ActionGroup;

        public List<SyncMarkerData> SyncMarkerDatas;
        //public FixedChunkNativeArray<float> SyncMarkerTargetTime;

        public RuntimeAction(Action a)
        {
            IsInitialized = true;

            ActionNameHash = a.m_ActionName.HashCode();

#if UNITY_EDITOR //AnimGraph                                
            AnimGraphPlayer.stateNameDict.AddIfNotContains(ActionNameHash, a.m_ActionName);
#endif
            ActionId = a.ActionId;
            Layer = a.m_Layer;
            LayerBlendMode = a.LayerBlendMode;
            TotalFrame = a.m_TotalFrame;
            TotalTime = a.m_TotalTime;
            ActionType = a.m_ActionType;
            Mode = a.m_Mode;

            //ActionClipName = new NativeList<FixedString64>(Allocator.Persistent);
            ActionClipName = new List<string>();
            ActionClipHash = new NativeList<int>(Allocator.Persistent);
            StateFrameList = new NativeList<ActionTimeStateStruct>(Allocator.Persistent);
            ActionTimeStateSwitchList = new NativeList<ActionTimeStateSwitchStruct>(Allocator.Persistent);

            ActionState = a.ActionState;
            ActionWeaponType = a.ActionWeaponType;

            DisplacementCurveXList = new List<AnimationCurve>();
            DisplacementCurveYList = new List<AnimationCurve>();
            DisplacementCurveZList = new List<AnimationCurve>();
            EulerXList = new List<AnimationCurve>();
            EulerYList = new List<AnimationCurve>();
            EulerZList = new List<AnimationCurve>();
            EulerWList = new List<AnimationCurve>();
            FreezePosXList = new NativeList<bool>(Allocator.Persistent);
            FreezePosYList = new NativeList<bool>(Allocator.Persistent);
            FreezePosZList = new NativeList<bool>(Allocator.Persistent);
            FreezeRotXList = new NativeList<bool>(Allocator.Persistent);
            FreezeRotYList = new NativeList<bool>(Allocator.Persistent);
            FreezeRotZList = new NativeList<bool>(Allocator.Persistent);

            LookatFrameList = new NativeList<LookatFrameStruct>(Allocator.Persistent);

            NextActionIdList = new NativeList<int>(Allocator.Persistent);
            NextStateList = new List<NextAction>();
            NextMotionList = new NativeList<NextMotionData>(Allocator.Persistent);

            IsUsingUnlockCamera = a.IsUsingUnlockCamera;
            UnlockCameraStartFrame = a.UnlockCameraStartFrame;
            UnlockCameraEndFrame = a.UnlockCameraEndFrame;

            IsUsingFlyAction = a.IsUsingFlyAction;
            FlyActionStartFrame = a.FlyActionStartFrame;
            FlyActionEndFrame = a.FlyActionEndFrame;
            FlyMinHeight = a.FlyMinHeight;
            FlyMaxHeight = a.FlyMaxHeight;
            FixHeightStartTime = a.FixHeightStartTime;
            FixHeightEndTime = a.FixHeightEndTime;
            PosFixValue = a.PosFixValue;

            IsAllowRotate = a.IsAllowRotate;
            RotateType = a.rotateType;
            AllowRotateFrameList = new NativeList<AllowRotateStruct>(Allocator.Persistent);

            IsAllowMoveByInputAxis = a.IsAllowMoveByInputAxis;
            AllowMoveList = new NativeList<AllowMoveStruct>(Allocator.Persistent);

            //Action 没有加这个功能
            IsAllowAssistMove = false;
            AssistMoveDataList = new NativeList<AssistMoveDataStruct>(Allocator.Persistent);

            Stamina = new ActionStaminaStruct();

            IsArmed = a.bArmed;
            ArmedTimeRate = a.ArmedTimeRate;
            ArmedAngle = a.ArmedAngle;

            IsUnArmed = a.bUnArmed;
            UnArmedTimeRate = a.UnArmedTimeRate;
            UnArmedAngle = a.UnArmedAngle;

            IsUseWeaponAction = a.IsUseWeaponAction;
            WeaponActionDatas = new NativeList<WeaponActionStruct>(Allocator.Persistent);

            StateFlagDatas = new List<ActionStateFlagStruct>();

            IsAllowAdditive = a.IsAllowAddtive;
            IsAllowHandIK = a.IsAllowIK;
            //TODO ActionEditor is not use anymore?
            IsAllowFootIK = true;

            IkHandFrameDatas = new NativeList<IKFrameDatastruct>(Allocator.Persistent);
            IkFootFrameDatas = new NativeList<IKFrameDatastruct>(Allocator.Persistent);
            ActionGroup = -1;

            ConditionChuncks = new FixedChunkNativeArray<MotionConditionData>();
            TriggerChuncks = new FixedChunkNativeArray<TriggerConditionData>();
            SpecialConditionChuncks = new FixedChunkNativeArray<SpecialConditionData>();

            SyncMarkerDatas = new List<SyncMarkerData>();
            //SyncMarkerTargetTime = new FixedChunkNativeArray<float>(12);

            //Init
            initActionClipNameAndHash(a.m_ActionClipName);
            initStateFrameList(a.m_StateFrameList);
            initActionTimeStateSwitchList(a.ActionTimeStateSwitchList);
            initCurve(DisplacementCurveXList, a.DisplacementCurveXList);
            initCurve(DisplacementCurveYList, a.DisplacementCurveYList);
            initCurve(DisplacementCurveZList, a.DisplacementCurveZList);
            initCurve(EulerXList, a.QuaternionCurveXList);
            initCurve(EulerYList, a.QuaternionCurveYList);
            initCurve(EulerZList, a.QuaternionCurveZList);
            initCurve(EulerWList, a.QuaternionCurveWList);
            initCurveFreezeSetting(FreezePosXList, a.FreezePosXList);
            initCurveFreezeSetting(FreezePosYList, a.FreezePosYList);
            initCurveFreezeSetting(FreezePosZList, a.FreezePosZList);
            initCurveFreezeSetting(FreezeRotXList, a.FreezeRotXList);
            initCurveFreezeSetting(FreezeRotYList, a.FreezeRotYList);
            initCurveFreezeSetting(FreezeRotZList, a.FreezeRotZList);

            initLookatFrameList(a.m_ListLookatFrame);

            //Next System
            initNextActionList(a.NextStateList);

            initAllowRotateList(a.AllowRotateFrameList);
            initAllowMoveList(a.AllowMoveList);
            //initAssistMoveData(a.AssistMoveDatas);
            initActionStamina(a.Stamina);

            initWeaponActionDatas(a.WeaponActionDatas);

            initStateFlagDatas(a.StateFlagDatas);
        }

        public RuntimeAction(MotionData m)
        {
            IsInitialized = true;

            ActionNameHash = m.ActionNameHash != 0 ? m.ActionNameHash : m.ActionName.HashCode();

#if UNITY_EDITOR //AnimGraph
            AnimGraphPlayer.stateNameDict.AddIfNotContains(ActionNameHash, m.ActionName);
#endif
            ActionId = m.ActionId;
            Layer = m.Layer;
            LayerBlendMode = m.LayerBlendMode;
            TotalFrame = m.TotalFrame;
            TotalTime = m.TotalTime;
            ActionType = m.ActionType;
            Mode = m.Mode;

            //ActionClipName = new NativeList<FixedString64>(Allocator.Persistent);
            ActionClipName = new List<string>();
            ActionClipHash = new NativeList<int>(Allocator.Persistent);
            StateFrameList = new NativeList<ActionTimeStateStruct>(Allocator.Persistent);
            ActionTimeStateSwitchList = new NativeList<ActionTimeStateSwitchStruct>(Allocator.Persistent);

            ActionState = m.ActionState;
            ActionWeaponType = m.WeaponType;

            DisplacementCurveXList = new List<AnimationCurve>();
            DisplacementCurveYList = new List<AnimationCurve>();
            DisplacementCurveZList = new List<AnimationCurve>();
            EulerXList = new List<AnimationCurve>();
            EulerYList = new List<AnimationCurve>();
            EulerZList = new List<AnimationCurve>();
            EulerWList = new List<AnimationCurve>();
            FreezePosXList = new NativeList<bool>(Allocator.Persistent);
            FreezePosYList = new NativeList<bool>(Allocator.Persistent);
            FreezePosZList = new NativeList<bool>(Allocator.Persistent);
            FreezeRotXList = new NativeList<bool>(Allocator.Persistent);
            FreezeRotYList = new NativeList<bool>(Allocator.Persistent);
            FreezeRotZList = new NativeList<bool>(Allocator.Persistent);

            LookatFrameList = new NativeList<LookatFrameStruct>(Allocator.Persistent);

            NextActionIdList = new NativeList<int>(Allocator.Persistent);
            NextStateList = new List<NextAction>();
            NextMotionList = new NativeList<NextMotionData>(Allocator.Persistent);

            //PreviousMotion = m.PreviousMotion;
            //NextMotionList = m.NextMotionList;

            IsUsingUnlockCamera = m.IsUsingUnlockCamera;
            UnlockCameraStartFrame = m.UnlockCameraStartFrame;
            UnlockCameraEndFrame = m.UnlockCameraEndFrame;

            IsUsingFlyAction = m.IsUsingFlyAction;
            FlyActionStartFrame = m.FlyActionStartFrame;
            FlyActionEndFrame = m.FlyActionEndFrame;
            FlyMinHeight = m.FlyMinHeight;
            FlyMaxHeight = m.FlyMaxHeight;
            FixHeightStartTime = m.FixHeightStartTime;
            FixHeightEndTime = m.FixHeightEndTime;
            PosFixValue = m.PosFixValue;

            IsAllowRotate = m.IsAllowRotate;
            RotateType = m.RotateType;
            AllowRotateFrameList = new NativeList<AllowRotateStruct>(Allocator.Persistent);

            IsAllowMoveByInputAxis = m.IsAllowMoveByInputAxis;
            AllowMoveList = new NativeList<AllowMoveStruct>(Allocator.Persistent);

            IsAllowAssistMove = m.IsAllowAssistMove;
            AssistMoveDataList = new NativeList<AssistMoveDataStruct>(Allocator.Persistent);

            Stamina = new ActionStaminaStruct();

            IsArmed = m.IsArmed;
            ArmedTimeRate = m.ArmedTimeRate;
            ArmedAngle = m.ArmedAngle;

            IsUnArmed = m.IsUnArmed;
            UnArmedTimeRate = m.UnArmedTimeRate;
            UnArmedAngle = m.UnArmedAngle;

            IsUseWeaponAction = m.IsUseWeaponAction;
            WeaponActionDatas = new NativeList<WeaponActionStruct>(Allocator.Persistent);

            StateFlagDatas = new List<ActionStateFlagStruct>();

            IsAllowAdditive = m.IsAllowAdditive;
            IsAllowHandIK = m.IsAllowHandIK;
            IsAllowFootIK = m.IsAllowFootIK;

            IkHandFrameDatas = new NativeList<IKFrameDatastruct>(Allocator.Persistent);
            IkFootFrameDatas = new NativeList<IKFrameDatastruct>(Allocator.Persistent);

            ActionGroup = m.ActionGroup;

            ConditionChuncks = new FixedChunkNativeArray<MotionConditionData>();
            TriggerChuncks = new FixedChunkNativeArray<TriggerConditionData>();
            SpecialConditionChuncks = new FixedChunkNativeArray<SpecialConditionData>();
            SyncMarkerDatas = new List<SyncMarkerData>();
            //SyncMarkerTargetTime = new FixedChunkNativeArray<float>(12);

            //initActionClipNameAndHash(m.ActionClipNameList);
            initActionClipName(m.ActionClipNameList);
            initActionClipHash(m.ActionClipNameHash, m.ActionClipNameList);

            initStateFrameList(m.StateFrameList);
            initActionTimeStateSwitchList(m.ActionTimeStateSwitchList);
            initCurve(DisplacementCurveXList, m.DisplacementCurveXList);
            initCurve(DisplacementCurveYList, m.DisplacementCurveYList);
            initCurve(DisplacementCurveZList, m.DisplacementCurveZList);
            initCurve(EulerXList, m.QuaternionCurveXList);
            initCurve(EulerYList, m.QuaternionCurveYList);
            initCurve(EulerZList, m.QuaternionCurveZList);
            initCurve(EulerWList, m.QuaternionCurveWList);
            initCurveFreezeSetting(FreezePosXList, m.FreezePosXList);
            initCurveFreezeSetting(FreezePosYList, m.FreezePosYList);
            initCurveFreezeSetting(FreezePosZList, m.FreezePosZList);
            initCurveFreezeSetting(FreezeRotXList, m.FreezeRotXList);
            initCurveFreezeSetting(FreezeRotYList, m.FreezeRotYList);
            initCurveFreezeSetting(FreezeRotZList, m.FreezeRotZList);

            initLookatFrameList(m.LookatFrameList);

            //Previous System
            initPrevious(m.PreviousMotion);

            //Next System
            initNextActionList(m.NextMotionList);

            initAllowRotateList(m.AllowRotateFrameList);
            initAllowMoveList(m.AllowMoveList);
            initAssistMoveData(m.AssistMoveDatas);
            initActionStamina(m.Stamina);

            initWeaponActionDatas(m.WeaponActionDatas);

            initStateFlagDatas(m.StateFlagDatas);

            initIKFrameDatas(m.IkHandFrameDatas, m.IkFootFrameDatas);
            initSyncMarkerDatas(m.SyncMarkers);
        }

        #region Costruct

        private void initActionClipNameAndHash(List<string> list)
        {
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                //FixedString64 str = new FixedString64(list[i]);
                ActionClipName.Add(list[i]);
                ActionClipHash.Add(list[i].HashCode());
            }
        }

        private void initActionClipName(List<string> list)
        {
            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                //FixedString64 str = new FixedString64(list[i]);
                ActionClipName.Add(list[i]);
            }
        }

        private void initActionClipHash(List<int> list, List<string> listName)
        {
            //TODO 后续迁移完毕后 这个函数可以拿掉
            if (list.IsNullOrEmpty())
            {
                foreach (var name in listName)
                {
                    ActionClipHash.Add(name.HashCode());
                }
            }

            int count = list.Count;

            for (int i = 0; i < count; i++)
            {
                ActionClipHash.Add(list[i]);
            }
        }

        private void initStateFrameList(List<ActionTimeStateSetting> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                ActionTimeStateStruct data = new ActionTimeStateStruct();
                data.State = list[i].m_State;
                data.Rate = list[i].m_Rate;
                StateFrameList.Add(data);
            }
        }

        private void initActionTimeStateSwitchList(List<ActionTimeStateSwitchSetting> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                ActionTimeStateSwitchStruct data = new ActionTimeStateSwitchStruct();
                data.NextType = list[i].NextType;
                data.Frame = list[i].Frame;
                ActionTimeStateSwitchList.Add(data);
            }
        }

        private void initCurve(List<AnimationCurve> curveList, List<AnimationCurve> targetList)
        {
            int count = targetList.Count;
            for (int i = 0; i < count; i++)
            {
                AnimationCurve newCurve = new AnimationCurve();

                AnimationCurve curve = targetList[i];
                Keyframe[] keys = curve.keys;
                int num = keys.Length;
                Keyframe[] newKeys = new Keyframe[num];
                for (int j = 0; j < num; ++j)
                {
                    newKeys[j] = new Keyframe(keys[j].time, keys[j].value, keys[j].inTangent, keys[j].outTangent, keys[j].inWeight, keys[j].outWeight);
                    newKeys[j].tangentMode = keys[j].tangentMode;
                    newKeys[j].weightedMode = keys[j].weightedMode;

                    newCurve.AddKey(newKeys[j]);
                }

                curveList.Add(newCurve);
            }
        }

        private void initCurveFreezeSetting(NativeList<bool> curveList, List<bool> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                curveList.Add(list[i]);
            }
        }

        private void initLookatFrameList(List<LookatFrame> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                LookatFrameStruct data = new LookatFrameStruct();
                data.BeginRatio = list[i].BeginRatio;
                data.EndRatio = list[i].EndRatio;
                LookatFrameList.Add(data);
            }
        }

        private void initNextActionList(List<NextAction> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                NextActionIdList.Add(list[i].Id);
                NextAction newNextAction = list[i].Clone();
                NextStateList.Add(newNextAction);
            }
        }

        private void initNextActionList(List<NextMotion> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                NextActionIdList.Add(list[i].Id);
                var newNextMotion = list[i].Create(ConditionChuncks, TriggerChuncks, SpecialConditionChuncks);
                NextMotionList.Add(newNextMotion);
            }
        }

        private void initPrevious(PreviousMotion prev)
        {
            prev.Clone();
        }

        private void initAllowRotateList(List<AllowRotateData> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                AllowRotateStruct data = new AllowRotateStruct();
                data.AllowRotateLimit = list[i].AllowRotateLimit;
                data.BeginFrame = list[i].BeginFrame;
                data.EndFrame = list[i].EndFrame;
                data.LimitAnglePerFrame = list[i].LimitAnglePerFrame;

                //assisting
                data.IsAllowAssistingRotate = list[i].IsAllowAssistingRotate;
                data.AssistingAngle = list[i].AssistingAngle;
                data.AssistingRotateDistance = list[i].AssistingRotateDistance;

                AllowRotateFrameList.Add(data);
            }
        }

        private void initAllowMoveList(List<AllowMoveData> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                AllowMoveStruct data = new AllowMoveStruct();
                data.BeginFrame = list[i].BeginFrame;
                data.EndFrame = list[i].EndFrame;
                data.MoveSpeed = list[i].MoveSpeed;
                AllowMoveList.Add(data);
            }
        }

        private void initAssistMoveData(List<AssistMoveData> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                AssistMoveDataStruct data = new AssistMoveDataStruct();
                data.BeginFrame = list[i].BeginFrame;
                data.EndFrame = list[i].EndFrame;
                data.AssistMoveSpeed = list[i].AssistMoveSpeed;
                data.AssistingDistance = list[i].AssistingDistance;
                AssistMoveDataList.Add(data);
            }
        }

        private void initActionStamina(ActionStaminaSetting stamina)
        {
            Stamina.StaminaBeginFrame = stamina.StaminaBeginFrame;
            Stamina.StaminaConsumpValue = stamina.StaminaConsumpValue;
            Stamina.LoopTime = stamina.LoopTime;
            Stamina.LifeTime = stamina.LifeTime;
            Stamina.StaminaConsumpType = stamina.StaminaConsumpType;
        }

        private void initWeaponActionDatas(List<WeaponActionData> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                WeaponActionStruct data = new WeaponActionStruct();
                data.TriggerFrame = list[i].TriggerFrame;
                data.WeaponActionState = list[i].WeaponActionState;
                WeaponActionDatas.Add(data);
            }
        }

        private void initStateFlagDatas(List<ActionStateFlagData> list)
        {
            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                ActionStateFlagStruct data = new ActionStateFlagStruct();
                //data.StateFlag = list[i].StateFlag;
                FixedString64 str = new FixedString64(list[i].StateFlag);
                data.StateFlag = str;
                data.StartFrame = list[i].StartFrame;
                data.EndFrame = list[i].EndFrame;
                StateFlagDatas.Add(data);
            }
        }

        private void initSyncMarkerDatas(List<SyncMarker> markers)
        {
            foreach (var marker in markers)
            {
                marker.MarkerHash = Animator.StringToHash(marker.MarkerName);

                SyncMarkerData markerData = new SyncMarkerData();
                markerData.MarkerHash = marker.MarkerHash;
                markerData.markTimes = new List<float>();
                for (var index = 0; index < marker.TargetTime.Count; index++)
                {
                    var time = marker.TargetTime[index];
                    markerData.markTimes.Add(time);
                }

                SyncMarkerDatas.Add(markerData);

//                    markerData.TargetTimeChunk = SyncMarkerTargetTime.AddChunk(targetTimes.Length);
//                    SyncMarkerTargetTime.ToJobArray().CopyFromFast(markerData.TargetTimeChunk.startIndex, targetTimes);
            }
        }

        private void initIKFrameDatas(List<IKHandFrameData> handDatas, List<IKFootFrameData> footFrameDatas)
        {
            int handDatasCount = handDatas.Count;
            for (int i = 0; i < handDatasCount; i++)
            {
                var frameData = handDatas[i];

                IKFrameDatastruct runtimeData = new IKFrameDatastruct
                {
                    FrameKey = frameData.FrameKey,
                    IsIKOn = frameData.IsIKOn
                };

                IkHandFrameDatas.Add(runtimeData);
            }

            int footDataCount = footFrameDatas.Count;
            for (int i = 0; i < footDataCount; i++)
            {
                var frameData = footFrameDatas[i];

                IKFrameDatastruct runtimeData = new IKFrameDatastruct
                {
                    FrameKey = frameData.FrameKey,
                    IsIKOn = frameData.IsIKOn
                };

                IkHandFrameDatas.Add(runtimeData);
            }
        }

        #endregion

        #region

        public void Dispose()
        {
            //ActionClipName.Dispose();
            ActionClipHash.Dispose();
            StateFrameList.Dispose();
            ActionTimeStateSwitchList.Dispose();

            FreezePosXList.Dispose();
            FreezePosYList.Dispose();
            FreezePosZList.Dispose();
            FreezeRotXList.Dispose();
            FreezeRotYList.Dispose();
            FreezeRotZList.Dispose();

            NextActionIdList.Dispose();
            NextMotionList.Dispose();
            LookatFrameList.Dispose();
            AllowRotateFrameList.Dispose();
            AllowMoveList.Dispose();
            AssistMoveDataList.Dispose();

            WeaponActionDatas.Dispose();

            IkFootFrameDatas.Dispose();
            IkHandFrameDatas.Dispose();

            ConditionChuncks.Dispose();
            TriggerChuncks.Dispose();
            SpecialConditionChuncks.Dispose();

            //SyncMarkerTargetTime.Dispose();
        }

        #endregion

        #region Action API

        public bool IsExistsNextActionID(int targetAcionID)
        {
            //TODO: 现阶段 默认都是连接着
            return true;
        }

        public float GetTimeRate(ActionTimeState s)
        {
            int num = StateFrameList.Length;
            if (num > 0)
            {
                for (int i = 0; i < num; ++i)
                {
                    if (StateFrameList[i].State == s)
                    {
                        return StateFrameList[i].Rate;
                    }
                }
            }

            return 1.0f;
        }

        public float GetSwitchTime(ActionType nextType)
        {
            int num = ActionTimeStateSwitchList.Length;
            if (num > 0)
            {
                for (int i = 0; i < num; ++i)
                {
                    if (ActionTimeStateSwitchList[i].NextType == nextType)
                    {
                        if (TotalFrame > 0)
                        {
                            return Mathf.Clamp((float) ActionTimeStateSwitchList[i].Frame / (float) TotalFrame, 0f, 1.0f);
                        }
                    }
                }
            }

            return 1.0f;
        }

        public ActionTimeState GetActionTimeState(float ratio)
        {
            ActionTimeState result = ActionTimeState.BEGIN;
            int num = StateFrameList.Length;
            if (num > 0)
            {
                for (int i = 0; i < num; ++i)
                {
                    if (StateFrameList[i].Rate > ratio)
                    {
                        break;
                    }

                    result = StateFrameList[i].State;
                }
            }

            return result;
        }

        int getProtectedRatio(ActionTimeState timeState)
        {
            int num = StateFrameList.Length;
            for (int i = 0; i < num; ++i)
            {
                if (StateFrameList[i].State == timeState)
                {
                    return (int) (StateFrameList[i].Rate * TotalFrame);
                }
            }

            return 0;
        }

        public NativeList<LookatFrameStruct> GetLookatFrameList()
        {
            //TODO:
            //NativeList<LookatFrameStruct> LookatFrameList;

            //List<LookatFrame> list = new List<LookatFrame>();
            //return LookatFrameList.Select(x=>new LookatFrame(){BeginRatio = x.BeginRatio, EndRatio = x.EndRatio}).ToList();

            return this.LookatFrameList;
        }

        public AllowRotateStruct GetAllowRotateData(int index)
        {
            //TODO:
            //AllowRotateData data = new AllowRotateData(); 
            return this.AllowRotateFrameList[index];
        }

        public AllowMoveStruct GetAllowMoveData(int index)
        {
            //TODO:
            //AllowMoveData data = new AllowMoveData();
            return this.AllowMoveList[index];
        }

        public AssistMoveDataStruct GetAssistMoveData(int index)
        {
            return this.AssistMoveDataList[index];
        }

        #endregion

        public static bool operator ==(RuntimeAction c1, RuntimeAction c2)
        {
            return c1.Equals(c2);
        }

        public static bool operator !=(RuntimeAction c1, RuntimeAction c2)
        {
            return !c1.Equals(c2);
        }

        public override bool Equals(object obj)
        {
            return Equals((RuntimeAction) obj);
        }

        public bool Equals(RuntimeAction ra)
        {
            return ActionId == ra.ActionId;
        }
    }
}