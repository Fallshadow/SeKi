using System.Collections.Generic;

namespace ASeKi.battle
{
    class BuffActionExecuter : BuffExecuter
    {
        protected bool sendAttachRequest = true;

        public ConfigTable.BuffEffectConfig Config
        {
            get;
            private set;
        }

        public virtual void Init(ConfigTable.BuffEffectConfig config, Buff buff, ulong targetId, bool sendAttachRequest)
        {
            this.Config = config;
            this.buff = buff;
            this.targetId = targetId;
            this.sendAttachRequest = sendAttachRequest;
        }

        public override void Execute()
        {

        }
    }
}
