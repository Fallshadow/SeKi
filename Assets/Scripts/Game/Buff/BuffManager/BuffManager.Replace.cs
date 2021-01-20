using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.battle
{
    // 专门为替换BUFF准备，有些BUFF会改变BUFF的生效
    // 比如油火龙喷火，本来挂的点燃BUFFID为1，持续3秒，但是当玩家身上有油脂BUFF时，可能会要求替换点燃BUFF，换成ID为2的持续7秒。
    // 当有这种需求时，应该在BUFF挂载之前就替换传递进来的BUFFID，也就是有两种做法：一种是检测玩家身上的BUFF列表？一种是当油脂这类BUFF在生效的时候就对公共数据中的X进行记录，当有ID为Y的buff在挂载的时候替换为ID为Z的buff
    // dictMainPlayerReplaceBuffs就是干这件事情的，我认为应该让挂载者管理这份数据才对
    public partial class BuffManager : Singleton<BuffManager>
    {
        private Dictionary<int, int> dictMainPlayerReplaceBuffs = new Dictionary<int, int>();

        public void SetReplaceBuff(int oldId, int newId, bool replace = true)
        {
            if(replace)
            {
                dictMainPlayerReplaceBuffs[oldId] = newId;
            }
            else
            {
                dictMainPlayerReplaceBuffs.Remove(oldId);
            }
        }

        private void clearReplaceBuff()
        {
            dictMainPlayerReplaceBuffs.Clear();
        }

        private int checkReplaceBuff(ulong source,int buffID)
        {
            if(source == BattleActorManager.instance.MainPlayer.ID)
            {
                if(dictMainPlayerReplaceBuffs.TryGetValue(buffID, out int newId))
                {
                    buffID = newId;
                }
            }
            return buffID;
        }
    }
}