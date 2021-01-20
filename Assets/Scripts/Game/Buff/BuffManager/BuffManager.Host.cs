using System.Collections.Generic;

namespace ASeKi.battle
{
    public partial class BuffManager : Singleton<BuffManager>
    {
        // 由非host机子转到host机子
        void toHost()
        {
            foreach(KeyValuePair<ulong, Carrier> kvPair in dictCarrier)
            {
                kvPair.Value.ToHost();
            }
        }

        // 由host机子转到非host机子
        void toUnHost()
        {
            foreach(KeyValuePair<ulong, Carrier> kvPair in dictCarrier)
            {
                kvPair.Value.ToUnHost();
            }
        }

        public void RefreshHost()
        {
            if(network.model.BattleModel.instance.isHost)
            {
                toHost();
            }
            else
            {
                toUnHost();
            }
        }

    }
}
