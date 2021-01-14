using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASeKi.battle 
{
    class BuffFinishTrigger
    {
        public bool Enabled = false;
        private int triggerCount = 0;    // 提前结束的次数
        Buff buff;

        public void Init(Buff buff)
        {
            this.buff = buff;
            if((constants.BuffFinishType)buff.Config.buff_finish_type == constants.BuffFinishType.NONE)
            {
                return;
            }
            Enabled = true;
            triggerCount = buff.Config.buff_finish_param1;
            registerEvent();
        }

        void registerEvent()
        {
            //监听跟这个条件挂钩的事件
            //Entity unit = BattleActorManager.instance.GetActorById(buff.Carrier);
            //if(unit is Hero)
            //{
            //    registerHeroEvent();
            //}
            //else if(unit is Monster)
            //{
            //    registerMonsterEvent();
            //}
        }

        void unregisterEvent()
        {
            //Entity unit = BattleActorManager.instance.GetActorById(buff.Carrier);
            //if(unit is Hero)
            //{
            //    unregisterHeroEvent();
            //}
            //else if(unit is Monster)
            //{
            //    unregisterMonsterEvent();
            //}
        }

        void triggered()
        {
            BuffManager.instance.DetachBuff(buff);
        }

        public void Release()
        {
            if(Enabled)
            {
                unregisterEvent();
            }
            this.buff = null;
            Enabled = false;
        }
    }
}
