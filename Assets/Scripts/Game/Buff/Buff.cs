using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.battle
{
    public class Buff
    {
        public uint Id = 0;
        public ulong SourceID = 0;              // 源
        public ulong CarrierID = 0;             // 载体
        public ConfigTable.BuffData Config;

        public int Layer
        {
            private set;
            get;
        }

        public float ExistTime
        {
            get
            {
                return exist_time;
            }
            set
            {
                exist_time = value;
            }
        }

        public float TotalTime
        {
            get { return Config.exist_time / constants.ConfigSetting.MILLISECOND; }
        }

        public bool OverTime
        {
            get
            {
                return exist_time <= 0;
            }
        }

        public bool Keep
        {
            get
            {
                return Config.exist_time == -1;
            }
        }

        private float exist_time = 0;

        public bool IsHostLogic = false;            // 是否是本机为主机
        public bool IgnoreRemoveEffect = false;
        public BuffEffect BuffEffect = new BuffEffect();
        private List<BuffPerform> performs = new List<BuffPerform>();
        HashSet<string> stateFlags = new HashSet<string>();
        public BuffAreaData BuffArea = new BuffAreaData();

        private BuffFinishTrigger finishTrigger = new BuffFinishTrigger();
        private BuffTrigger buffTrigger;

        #region Init 相关

        public virtual void Init(ulong sourceID, ulong carrierID, int buffConfigId, uint buffDid)
        {
            Id = buffDid;
            SourceID = sourceID;
            CarrierID = carrierID;
            Config = new ConfigTable.BuffData(buffConfigId);
            exist_time = 1.0f * Config.exist_time / constants.ConfigSetting.MILLISECOND;

            Layer = 1;
            IgnoreRemoveEffect = false;
            createBuffEffect();
            createBuffPerform();

            if(IsHostLogic)
            {
                createStateFlagCheck();             // 不是主机没有必要进行更新检测，交给主机执行然后直接同步就好。
                createBuffArea();
                buffTrigger = BuffTriggerFactory.Create((constants.BuffCarrierTrigger)Config.event_config1, this);
                finishTrigger.Init(this);
            }
        }

        void createBuffEffect()
        {
            BuffEffect.Init(this);
        }

        void createBuffPerform()
        {
            if(performs.Count <= 0)
            {
                performs.Add(new BuffSoundPerform());
                performs.Add(new BuffVFXPerform());
                performs.Add(new BuffUIPerform());
            }

            for(int i = 0; i < performs.Count; i++)
            {
                performs[i].ParentBuff = this;
            }
        }

        // 根据载体位置绘制BUFF区域
        void createBuffArea()
        {
            if(Config.target_area_id > 0)
            {
                Entity unit = BattleActorManager.instance.GetActorById(CarrierID);
                if(unit != null)
                {
                    BuffArea.SetData(unit.RootGo.transform.position, unit.RootGo.transform.rotation,
                        Config.target_area_id);
                }
            }
        }

        private ASeKi.time.Timer timerStateFlag = null;

        // 创建更新检测
        void createStateFlagCheck()
        {
            bool containContent = false;
            for(int i = 0; i < Config.state_flags.Length; i++)
            {
                if(!string.IsNullOrEmpty(Config.state_flags[i]) && Config.state_flags[i] != "0")
                {
                    containContent = true;
                    break;
                }
            }
            if(!containContent || Config.stage_change_remove == 0)
            {
                return;
            }
            timerStateFlag = ASeKi.time.TimeManager.instance.AddUpdateTimer(false, "bsf_" + Id, checkStateFlag);
        }

        // 更新检测，如果载体状态不对，执行对应操作
        void checkStateFlag(float cycle, object obj = null)
        {
            stateFlags.Clear();
            ASeKi.action.ActionManager.instance.GetCurrentStateFlags(CarrierID, stateFlags);
            for(int i = 0; i < Config.state_flags.Length; i++)
            {
                if(!string.IsNullOrEmpty(Config.state_flags[i]))
                {
                    if(stateFlags.Contains(Config.state_flags[i]))
                    {
                        return;
                    }
                }
            }

            //移除效果
            if(Config.stage_change_remove == 1)
            {
                BuffEffect?.Effect(0, false);
            }
            //移除buff
            else if(Config.stage_change_remove == 2)
            {
                BuffManager.instance.DetachBuff(this);
            }
        }
        #endregion

        public virtual void Release(bool ignoreRemoveEffect = false)
        {
            for(int index = 0; index < performs.Count; index++)
            {
                performs[index]?.OnDetach();
            }
            buffTrigger?.Release();
            IgnoreRemoveEffect = IgnoreRemoveEffect || ignoreRemoveEffect;
            if(OverTime && !Keep) // 生命周期结束引起的结束，不触发时触发
            {
                IgnoreRemoveEffect = true;
            }
            BuffEffect.Release();
            BuffArea.Release();
            finishTrigger.Release();
            effected = false;
            if(timerStateFlag != null)
            {
                ASeKi.time.TimeManager.instance.RemoveTimer(timerStateFlag);
                timerStateFlag = null;
            }
        }

        // 由载体buff挂载时触发
        public virtual void Start()
        {
            buffTrigger?.Start();
            PerformAttach();
        }

        public void PerformAttach()
        {
            for(int i = 0; i < performs.Count; i++)
            {
                performs[i]?.OnAttach();
            }
        }

        #region buff叠加

        public void Addtive()
        {
            Layer++;
            // if(!BuffEffect.Enabled)
            // {
            // 	return;
            // }
            if(Layer >= Config.addtive_effect_layer)
            {
                BuffEffect?.ExcuteAddtiveEffect();
            }
            if(Config.max_addtive_layer > 0 && Layer > Config.max_addtive_layer)
            {
                Layer = Config.max_addtive_layer;
                return;
            }
            BuffEffect?.Addtive();
        }

        public void AddtiveFresh()
        {
            Layer++;
            // if(!BuffEffect.Enabled)
            // {
            // 	return;
            // }
            if(Layer >= Config.addtive_effect_layer)
            {
                BuffEffect?.ExcuteAddtiveEffect();
            }
            if(Config.max_addtive_layer > 0 && Layer > Config.max_addtive_layer)
            {
                Layer = Config.max_addtive_layer;
                exist_time = 1.0f * Config.exist_time / constants.ConfigSetting.MILLISECOND;
                return;
            }
            exist_time = 1.0f * Config.exist_time / constants.ConfigSetting.MILLISECOND;
            BuffEffect?.Addtive();
        }

        #endregion

        #region HOST

        // 非host buff转成host buff
        public void FakeToReal()
        {
            if(IsHostLogic)
            {
                ASeKi.debug.PrintSystem.Log("[buff] real buff want to real");
                return;
            }
            IsHostLogic = true;
            createStateFlagCheck();
            createBuffArea();
            buffTrigger = BuffTriggerFactory.Create((constants.BuffCarrierTrigger)Config.event_config1, this, false);
            buffTrigger.FakeToReal();
            finishTrigger.Init(this);
        }

        // host buff转成非host buff
        public void RealToFake()
        {
            if(!IsHostLogic)
            {
                ASeKi.debug.PrintSystem.Log("[buff] fake buff want to fake");
                return;
            }
            IsHostLogic = false;
            if(timerStateFlag != null)
            {
                ASeKi.time.TimeManager.instance.RemoveTimer(timerStateFlag);
                timerStateFlag = null;
            }
            BuffArea.Release();
            buffTrigger?.Release();
            finishTrigger.Release();
        }

        #endregion

        #region 重连生效

        public void Resume(int layer, bool isEffect, bool sendAttachRequest)
        {
            if(isEffect)
            {
                TriggerWithoutNet(0, true, sendAttachRequest);
                BuffEffect.Resume(layer);
                Layer = layer;
            }
            if(!isEffect || !(buffTrigger is BuffTriggerAttach))
            {
                buffTrigger?.Start();
            }
        }

        #endregion

        #region 生效

        public void TriggerWithoutNet(ulong targetId = 0, bool satisfy = true, bool sendRequest = false)
        {
            effected = true;
            BuffEffect?.Effect(targetId, satisfy);
            for(int i = 0; i < performs.Count; i++)
            {
                performs[i]?.OnEffect();
            }
        }

        // 表现生效
        public void Effect(ulong targetId = 0, bool satisfy = true, bool sendAttachRequest = true)
        {
            effected = true;
            // if(BuffEffect.Enabled)
            // {
            BuffEffect?.Effect(targetId, satisfy, sendAttachRequest);
            // }
            for(int i = 0; i < performs.Count; i++)
            {
                performs[i]?.OnEffect();
            }
        }

        #endregion

        public void UpdateExistTime(float timeScale)
        {
            if(Config.countdown_type == 0)
            {
                exist_time -= Time.deltaTime * timeScale;
            }
            else
            {
                if(effected)
                {
                    exist_time -= Time.deltaTime * timeScale;
                }
            }
        }

        public void DisableVFXPerform()
        {
            if(performs.Count < 1 || !(performs[1] is BuffVFXPerform))
            {
                return;
            }
            (performs[1] as BuffVFXPerform).OnDetach();
        }























        public int SourceSkill = 0;             // 目前只记录技能释放时给自己挂载的buff


        private bool effected = false;

        // 生效
        public void TriggerWithNet(ulong targetId, bool satisfy = true,bool sendRequest = false)
        {
            // TODO:发送服务端消息
            TriggerWithoutNet(targetId, satisfy);
        }
    }
}
