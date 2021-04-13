using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

// 编辑器下的动作数据
namespace ASeKi.action
{
    
    #region 动作帧 - 状态/阶段 信息

    public enum ActionType
    {
        IDLE = 0,
        MOVE,
        ACTION,
        DODGE,
        STATUS,
        HIT,
        DIE,
        SPECIAL,
        ROTATE,
        MOVESTOP,
        SKILL,
        BORN,

        // last element
        ALL
    }
    
    [Serializable]
    public class ActionTimeStateSwitchSetting
    {
        public ActionType NextType = ActionType.IDLE;
        public int Frame;
    }
    
    public enum ActionTimeState
    {
        BEGIN = 0,
        PROTECT = 1,
        CANCEL = 2,
        EARLY_QUIT = 3,
        LATE_QUIT = 4,
        RESET = 5,
        END = 6,

        // last element
        ALL
    }
    
    [Serializable]
    public class ActionTimeStateSetting
    {
        public ActionTimeState m_State = ActionTimeState.BEGIN;
        public float m_Rate;
    }

    #endregion
    
    #region 武器动作帧 - 状态枚举 信息

    [Serializable]
    public class WeaponActionData
    {
        public int TriggerFrame = 0;
        public WeaponActionState WeaponActionState = WeaponActionState.WAS_IDLE_0;
    }
    
    public enum WeaponActionState
    {
        WAS_IDLE_0 = 0,
        WAS_IDLE_1 = 1,
        WAS_IDLE_2 = 2,
        WAS_IDLE_3 = 3,
        WAS_IDLE_4 = 4,
        WAS_IDLE_5 = 5,
        WAS_ATTACK_1 = 11,
        WAS_ATTACK_2 = 12,
        WAS_ATTACK_3 = 13,
        WAS_ATTACK_4 = 14,
        WAS_ATTACK_5 = 15,
        WAS_ATTACK_6 = 16,
        WAS_POSTURE_1 = 21,
        WAS_POSTURE_2 = 22,
        WAS_CHARGE_1 = 31,//充能1
        WAS_CHARGE_2 = 32,//充能2
        CHANGE_POSTURE_0 = 40,
        CHANGE_POSTURE_1 = 41,
        CHANGE_POSTURE_2 = 42,
        CHANGE_POSTURE_3 = 43,
        DEFEND_START = 50,
        DEFEND_LOOP = 51,
        DEFEND_END = 52,
    }

    #endregion
    
    #region 动作条件

    [Serializable]
    public class MotionCondition
    {
        // ReSharper disable once RedundantDefaultMemberInitializer
        public bool IsUseButtonCondition = false;
        public ButtonCondition ButtonCondition = new ButtonCondition();
        public List<TriggerCondition> ConditionList = new List<TriggerCondition>();
    }
    
    [Serializable]
    public class ButtonCondition
    {
        public input.ButtonType ButtonType = input.ButtonType.NONE;
        public input.PressType ButtonPressType = input.PressType.NONE;
    }

    [Serializable]
    public class TriggerCondition
    {
        public string ConditionName = "";
        public float ConditionValue = 0.0f;
        public ValueConditionType ConditionType = ValueConditionType.VCT_EQUAL;
        public bool IsTrigger = false;
    }
    
    public enum ValueConditionType
    {
        VCT_EQUAL = 0,
        VCT_LESS,
        VCT_GREATER,
        VCT_LESS_AND_EQUAL,
        VCT_GREATER_AND_EQUAL,
        VCT_NOT_EQUAL,
        VCT_CONTAINS,
        VCT_NO_CONTAINS
    }
    
    [Serializable]
    public class MotionSpecialCondition
    {
        public List<ActionComboSpeicalConditionData> SpecialConditionList = new List<ActionComboSpeicalConditionData>();
    }
    
    [Serializable]
    public class ActionComboSpeicalConditionData
    {
        public ActionComboSpecialType Type = ActionComboSpecialType.ACST_NONE;
        public ActionType ActionType = ActionType.IDLE;    // 只能在特定類型動作後
        //public ActionTimeState ActionTimeState = ActionTimeState.BEGIN;    // 只能在特定階段輸入後    // 目前似乎挪到了帧对应上
        public float ActionTimeStartFrame = 0.0f;
        public float ActionTimeEndFrame = 0.0f;

        // 特定資源相關 限制/扣除
        public ActionComboResourceValueType ResourceValueType = ActionComboResourceValueType.ACRV_NUMBER;
        public ActionComboResourceType ResourceType = ActionComboResourceType.ACRT_HP;
        public ValueConditionType ResourceConditionType = ValueConditionType.VCT_EQUAL;
        public float ResourceValue = 0f;
    }
    
    public enum ActionComboResourceValueType
    {
        ACRV_NUMBER,
        ACRV_PERCENTAGE
    }
    
    public enum ActionComboResourceType
    {
        ACRT_HP = 0,
        ACRT_ENERGY,
        ACRT_ACTION_POINT,
        ACRT_ACTION_GUAGE,
        ACRT_WEAPON_ENERGY,
    }
    
    public enum ActionComboSpecialType
    {
        ACST_NONE = 0,
        ACST_CHECK_ACTIONTYPE,              // 只能在特定類型動作後
        ACST_CHECK_ACTIONTIMESTATE,         // 只能在特定階段輸入後
        ACST_CHECK_LAST_ATK_HIT,            // 成功命中怪物後
        //ACST_CheckDodgeSuccess,           // 成功閃避攻擊後
        ACST_CHECK_ACTION_TIME_ONHIT_STATE, // 只能在特定階段被擊中後
        ACST_CHECK_RESOURCE_LIMIT,          // 特定資源限制
        ACST_CHECK_RESOURCE_COST            // 特定資源扣除
    }

    #endregion

    #region 旋转

    public enum AllowRotateType
    {
        ART_MOTION = 0, //Rotate by the motion Dir
        ART_CAMERA = 1, //Rotate by the Camera Dir
        ART_MOTION_ASSIST = 2 //Rotate by the Assist Dir First
    }
    
    [Serializable]
    public class AllowRotateData
    {
        public Vector2 AllowRotateLimit = Vector2.zero;
        public int BeginFrame = 0;
        public int EndFrame = 0;
        public float LimitAnglePerFrame = 0f;

        // Assisting
        public bool IsAllowAssistingRotate = false;
        public float AssistingAngle = 0f;            
        public float AssistingRotateDistance = 8.5f;    // default
    }

    #endregion

    #region 移动
    
    [Serializable]
    public class AllowMoveData
    {
        public int BeginFrame = 0;
        public int EndFrame = 0;
        public float MoveSpeed = 0f;
        // public bool NeedAssisting = false;
        // public float AssistingDistance = 0f;
    }
    
    [Serializable]
    public class AssistMoveData
    {
        public int BeginFrame = 0;
        public int EndFrame = 0;
        public float AssistMoveSpeed = 0f;
        public float AssistingDistance = 0f;
    }

    #endregion
    
    #region 注视

    [Serializable]
    public class LookatFrame
    {
        public float BeginRatio = 0.0f;
        public float EndRatio = 1.0f;
    }

    #endregion

    #region 耐力

    [Serializable]
    public class ActionStaminaSetting
    {
        public int StaminaBeginFrame = 0;
        public int StaminaConsumpValue = 0;
        public float LoopTime = 0.0f;
        public float LifeTime = 0.0f;
        public StaminaConsumptionType StaminaConsumpType = StaminaConsumptionType.NONE;
    }
    
    public enum StaminaConsumptionType : int
    {
        // NONE
        NONE = 0,
        // ONCE
        ONCE = 1,
        // CONTINUOUS
        CONTINUOUS = 2,
        // CONTINUOUS HAS LIFE TIME
        CONTINUOUS_HAS_LIFE_TIME = 3,
    }

    #endregion

    #region IK

    [Serializable]
    public class IKHandFrameData
    {
        public int FrameKey = 0;
        public bool IsIKOn = false;
    }
    
    [Serializable]
    public class IKFootFrameData
    {
        public int FrameKey = 0;
        public bool IsIKOn = false;
    }

    #endregion
    
    [Serializable]
    public class ActionStateFlagData
    {
        public string StateFlag = null;
        public int StartFrame = 0;
        public int EndFrame = 0;
    }
    
    [Serializable]
    public class SyncMarker
    {
        public int MarkerHash;
        public string MarkerName;
        public List<float> TargetTime;
    }
    
    #region 前一个/下一个 动作

    [Serializable]
    public class PreviousMotion
    {
        public List<MotionCondition> PreviousMotionConditionList = new List<MotionCondition>();
        public MotionSpecialCondition SpecialCondition = new MotionSpecialCondition();
        public ActionRoleType MotionRoleType = ActionRoleType.ART_PLAYER_NONE;

        public PreviousMotion Clone()
        {
            PreviousMotion newClass = new PreviousMotion();
            newClass.MotionRoleType = MotionRoleType;

            int count = PreviousMotionConditionList.Count;
            for (int i = 0; i < count; i++)
            {
                MotionCondition newMC = new MotionCondition();
                newMC.IsUseButtonCondition = PreviousMotionConditionList[i].IsUseButtonCondition;

                ButtonCondition newBC = new ButtonCondition();
                newBC.ButtonType = PreviousMotionConditionList[i].ButtonCondition.ButtonType;
                newBC.ButtonPressType = PreviousMotionConditionList[i].ButtonCondition.ButtonPressType;
                newMC.ButtonCondition = newBC;

                newMC.ConditionList = new List<TriggerCondition>();
                int num = PreviousMotionConditionList[i].ConditionList.Count;
                for (int j = 0; j < num; j++)
                {
                    TriggerCondition tc = PreviousMotionConditionList[i].ConditionList[j];
                    TriggerCondition newTC = new TriggerCondition();
                    newTC.ConditionName = tc.ConditionName;
                    newTC.ConditionType = tc.ConditionType;
                    newTC.ConditionValue = tc.ConditionValue;

                    newMC.ConditionList.Add(newTC);
                }

                newClass.PreviousMotionConditionList.Add(newMC);
            }

            int count2 = SpecialCondition.SpecialConditionList.Count;
            for (int i = 0; i < count2; i++)
            {
                ActionComboSpeicalConditionData newData = new ActionComboSpeicalConditionData();
                var spc = SpecialCondition.SpecialConditionList[i];
                newData.Type = spc.Type;
                newData.ActionType = spc.ActionType;
                newData.ActionTimeStartFrame = spc.ActionTimeStartFrame;
                newData.ActionTimeEndFrame = spc.ActionTimeEndFrame;
                newData.ResourceValueType = spc.ResourceValueType;
                newData.ResourceConditionType = spc.ResourceConditionType;
                newData.ResourceValue = spc.ResourceValue;

                newClass.SpecialCondition.SpecialConditionList.Add(newData);
            }

            return newClass;
        }

        // 创建游戏中的PreviousMotionData结构体
        public PreviousMotionData Create(
            FixedChunkNativeArray<MotionConditionData> conditions,
            FixedChunkNativeArray<TriggerConditionData> triggers,
            FixedChunkNativeArray<SpecialConditionData> specials
        )
        {
            PreviousMotionData newPreviousData = new PreviousMotionData();
            newPreviousData.MotionRoleType = MotionRoleType;

            int count = PreviousMotionConditionList.Count;
            var tempConditions = new NativeArray<MotionConditionData>(count, Allocator.Temp);

            for (int i = 0; i < count; i++)
            {
                MotionConditionData newMC = new MotionConditionData();
                newMC.IsUseButtonCondition = PreviousMotionConditionList[i].IsUseButtonCondition;

                ButtonConditionData newBC = new ButtonConditionData();
                newBC.ButtonType = PreviousMotionConditionList[i].ButtonCondition.ButtonType;
                newBC.ButtonPressType = PreviousMotionConditionList[i].ButtonCondition.ButtonPressType;
                newMC.ButtonCondition = newBC;

                int num = PreviousMotionConditionList[i].ConditionList.Count;
                var tempTriggers = new NativeArray<TriggerConditionData>(num, Allocator.Temp);
                for (int j = 0; j < num; j++)
                {
                    TriggerCondition tc = PreviousMotionConditionList[i].ConditionList[j];
                    TriggerConditionData newTC = new TriggerConditionData();
                    newTC.ConditionName = Animator.StringToHash(tc.ConditionName);
                    newTC.ConditionType = tc.ConditionType;
                    newTC.ConditionValue = tc.ConditionValue;

                    tempTriggers[j] = newTC;
                }

                newMC.ConditionChunck = triggers.AddChunk(num);
                triggers.ToJobArray().CopyFromFast(newMC.ConditionChunck.startIndex, tempTriggers);

                tempConditions[i] = newMC;
                tempTriggers.Dispose();
            }

            newPreviousData.PreviousMotionConditionChunk = conditions.AddChunk(count);
            conditions.ToJobArray().CopyFromFast(newPreviousData.PreviousMotionConditionChunk.startIndex, tempConditions);
            tempConditions.Dispose();

            int count2 = SpecialCondition.SpecialConditionList.Count;
            for (int i = 0; i < count2; i++)
            {
                var spc = SpecialCondition.SpecialConditionList[i];

                SpecialConditionData newData = new SpecialConditionData();
                newData.Type = spc.Type;
                newData.ActionType = spc.ActionType;
                newData.ActionTimeStartFrame = spc.ActionTimeStartFrame;
                newData.ActionTimeEndFrame = spc.ActionTimeEndFrame;
                newData.ResourceValueType = spc.ResourceValueType;
                newData.ResourceConditionType = spc.ResourceConditionType;
                newData.ResourceValue = spc.ResourceValue;

                newPreviousData.SpecialCondition.SpecialConditionChunck = specials.Add(newData);
            }

            return newPreviousData;
        }
    }

    [Serializable]
    public class NextMotion
    {
        public int Id = 0;
        public List<MotionCondition> NextMotionConditionList = new List<MotionCondition>();    // Trigger 1 , 2, 3 ... 
        public MotionSpecialCondition SpecialCondition = new MotionSpecialCondition();
        public ActionRoleType MotionRoleType = ActionRoleType.ART_PLAYER_NONE;
        public bool IsLockApplyToAllNextState = false;

        public NextMotion Clone()
        {
            NextMotion newClass = new NextMotion();
            newClass.Id = Id;
            newClass.MotionRoleType = MotionRoleType;

            int count = NextMotionConditionList.Count;
            for (int i = 0; i < count; i++)
            {
                MotionCondition newMC = new MotionCondition();
                newMC.IsUseButtonCondition = NextMotionConditionList[i].IsUseButtonCondition;

                ButtonCondition newBC = new ButtonCondition();
                newBC.ButtonType = NextMotionConditionList[i].ButtonCondition.ButtonType;
                newBC.ButtonPressType = NextMotionConditionList[i].ButtonCondition.ButtonPressType;
                newMC.ButtonCondition = newBC;

                newMC.ConditionList = new List<TriggerCondition>();
                int num = NextMotionConditionList[i].ConditionList.Count;
                for (int j = 0; j < num; j++)
                {
                    TriggerCondition tc = NextMotionConditionList[i].ConditionList[j];
                    TriggerCondition newTC = new TriggerCondition();
                    newTC.ConditionName = tc.ConditionName;
                    newTC.ConditionType = tc.ConditionType;
                    newTC.ConditionValue = tc.ConditionValue;

                    newMC.ConditionList.Add(newTC);
                }

                newClass.NextMotionConditionList.Add(newMC);
            }

            int count2 = SpecialCondition.SpecialConditionList.Count;
            for (int i = 0; i < count2; i++)
            {
                ActionComboSpeicalConditionData newData = new ActionComboSpeicalConditionData();
                var spc = SpecialCondition.SpecialConditionList[i];
                newData.Type = spc.Type;
                newData.ActionType = spc.ActionType;
                newData.ActionTimeStartFrame = spc.ActionTimeStartFrame;
                newData.ActionTimeEndFrame = spc.ActionTimeEndFrame;
                newData.ResourceValueType = spc.ResourceValueType;
                newData.ResourceConditionType = spc.ResourceConditionType;
                newData.ResourceValue = spc.ResourceValue;

                newClass.SpecialCondition.SpecialConditionList.Add(newData);
            }

            return newClass;
        }

        public NextMotionData Create(FixedChunkNativeArray<MotionConditionData> conditions,
            FixedChunkNativeArray<TriggerConditionData> triggers,
            FixedChunkNativeArray<SpecialConditionData> specials)
        {
            NextMotionData newClass = new NextMotionData {Id = Id, MotionRoleType = MotionRoleType};

            int count = NextMotionConditionList.Count;

            var tempConditions = new NativeArray<MotionConditionData>(count, Allocator.Temp);

            for (int i = 0; i < count; i++)
            {
                MotionConditionData newMC = new MotionConditionData
                {
                    IsUseButtonCondition = NextMotionConditionList[i].IsUseButtonCondition
                };

                ButtonConditionData newBC = new ButtonConditionData
                {
                    ButtonType = NextMotionConditionList[i].ButtonCondition.ButtonType,
                    ButtonPressType = NextMotionConditionList[i].ButtonCondition.ButtonPressType
                };
                newMC.ButtonCondition = newBC;

                int num = NextMotionConditionList[i].ConditionList.Count;
                var tempTriggers = new NativeArray<TriggerConditionData>(num, Allocator.Temp);

                for (int j = 0; j < num; j++)
                {
                    var tc = NextMotionConditionList[i].ConditionList[j];
                    TriggerConditionData newTC = new TriggerConditionData
                    {
                        ConditionName = Animator.StringToHash(tc.ConditionName),
                        ConditionType = tc.ConditionType,
                        ConditionValue = tc.ConditionValue
                    };

                    tempTriggers[j] = newTC;
                }

                newMC.ConditionChunck = triggers.AddChunk(num);
                triggers.ToJobArray().CopyFromFast(newMC.ConditionChunck.startIndex, tempTriggers);
                tempTriggers.Dispose();

                tempConditions[i] = newMC;
            }

            newClass.NextMotionConditionChunck = conditions.AddChunk(count);
            conditions.ToJobArray().CopyFromFast(newClass.NextMotionConditionChunck.startIndex, tempConditions);

            tempConditions.Dispose();

            int count2 = SpecialCondition.SpecialConditionList.Count;
            for (int i = 0; i < count2; i++)
            {
                var spc = SpecialCondition.SpecialConditionList[i];

                SpecialConditionData newData = new SpecialConditionData
                {
                    Type = spc.Type,
                    ActionType = spc.ActionType,
                    ActionTimeStartFrame = spc.ActionTimeStartFrame,
                    ActionTimeEndFrame = spc.ActionTimeEndFrame,
                    ResourceValueType = spc.ResourceValueType,
                    ResourceConditionType = spc.ResourceConditionType,
                    ResourceValue = spc.ResourceValue
                };

                newClass.SpecialCondition.SpecialConditionChunck = specials.Add(newData);
            }


            return newClass;
        }
    }

    #endregion

}