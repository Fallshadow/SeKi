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
        public int SourceSkill = 0;             // 目前只记录技能释放时给自己挂载的buff
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
        }
        private float exist_time = 0;

        public bool IsHostLogic = false;            // 是否是本机为主机

        private BuffTrigger buffTrigger; 
        public BuffEffect BuffEffect = new BuffEffect();

        private bool effected = false;

        private List<BuffPerform> performs = new List<BuffPerform>();

        public virtual void Init(ulong source, ulong carrier, int buffConfigId, uint id)
        {
            this.SourceID = source;
            this.CarrierID = carrier;
            Config = new ConfigTable.BuffData(buffConfigId);
            exist_time = 1.0f * Config.exist_time / constants.ConfigSetting.MILLISECOND;
            Id = id;
            Layer = 1;
            if(IsHostLogic)
            {
                createStateFlagCheck();             // 不是主机没有必要进行更新检测，交给主机执行然后直接同步就好。
                createBuffArea();
                buffTrigger = BuffTriggerFactory.Create((constants.BuffCarrierTrigger)Config.event_config1, this);
                
            }
        }

        public virtual void Release(bool ignoreRemoveEffect = false)
        {
            for(int index = 0; index < performs.Count; index++)
            {
                performs[index]?.OnDetach();
            }
            buffTrigger?.Release();
        }

        // 生效
        public void TriggerWithNet(ulong targetId, bool satisfy = true)
        {
            // TODO:发送服务端消息
            TriggerWithoutNet(targetId, satisfy);
        }

        public void TriggerWithoutNet(ulong targetId = 0, bool satisfy = true)
        {
            effected = true;
            BuffEffect?.Effect(targetId, satisfy);
            for(int i = 0; i < performs.Count; i++)
            {
                performs[i]?.OnEffect();
            }
        }


        // 根据载体位置绘制BUFF区域
        void createBuffArea()
        {
            //if(Config.target_area_id > 0)
            //{
            //    Entity unit = BattleActorManager.instance.GetActorById(Carrier);
            //    if(unit != null)
            //    {
            //        BuffArea.SetData(unit.RootGo.transform.position, unit.RootGo.transform.rotation,
            //            Config.target_area_id);
            //    }
            //}
        }

        // 创建更新检测
        void createStateFlagCheck()
        {
            //bool containContent = false;
            //for(int i = 0; i < Config.state_flags.Length; i++)
            //{
            //    if(!string.IsNullOrEmpty(Config.state_flags[i]) && Config.state_flags[i] != "0")
            //    {
            //        containContent = true;
            //        break;
            //    }
            //}
            //if(!containContent || Config.stage_change_remove == 0)
            //{
            //    return;
            //}
            //timerStateFlag = act.time.TimeManager.instance.AddUpdateTimer(false, "bsf_" + Id, checkStateFlag);
        }

        // 更新检测，如果载体状态不对，执行对应操作
        void checkStateFlag(float cycle, object obj = null)
        {
            //stateFlags.Clear();
            //act.action.ActionManager.instance.GetCurrentStateFlags(Carrier, stateFlags);
            //for(int i = 0; i < Config.state_flags.Length; i++)
            //{
            //    if(!string.IsNullOrEmpty(Config.state_flags[i]))
            //    {
            //        if(stateFlags.Contains(Config.state_flags[i]))
            //        {
            //            return;
            //        }
            //    }
            //}

            ////移除效果
            //if(Config.stage_change_remove == 1)
            //{
            //    BuffEffect?.Effect(0, false);
            //}
            ////移除buff
            //else if(Config.stage_change_remove == 2)
            //{
            //    BuffManager.instance.RemoveBuff(this);
            //}
        }
    }
}
