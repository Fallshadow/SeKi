
namespace ASeKi.battle
{
    // 应用到属性上的效果
    public class AttrExecuter : BuffExecuter
    {
        public ConfigTable.BuffAttrConfig AttrConfig
        {
            get;
            private set;
        }

        public void Init(ConfigTable.BuffAttrConfig config, Buff buff)
        {
            AttrConfig = config;
            this.buff = buff;
        }
        //public Framework.StructAttr AttrConfig
        //{
        //    get;
        //    private set;
        //}
        //public void Init(Framework.StructAttr config, Buff buff)
        //{
        //    AttrConfig = config;
        //    this.buff = buff;
        //}

        public override void Execute()
        {
            BuffSelectTarget.GetBuffTargets(buff, units, targetId);
            if(units == null)
            {
                return;
            }
            //if(AttrConfig.attr_delaytime <= 0)
            //{
            //    delayDo();
            //    return;
            //}
            //timerDelay = act.time.TimeManager.instance.AddCountDownTimer(false, "Buff_Effect_Execute_" + buff.Id,
            //    1.0f * AttrConfig.attr_delaytime / act.data.ConfigSetting.MILLISECOND, delayDo);
        }
    }
}
