using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.battle
{
    public partial class BuffManager : Singleton<BuffManager>
    {
        private BuffAttachApplyData buffAttachApplyData = new BuffAttachApplyData();

        // 申请挂载BUFF，得到的回应储存在BuffAttachApplyData中，游戏中保持一份，防止内存占用，由于也是立即使用所以也不用太过担心
        private void sendAttachApply(ulong source, ulong carrier, int configId)
        {
            buffAttachApplyData.Reset();
            buffAttachApplyData.Source = source;
            buffAttachApplyData.Carrier = carrier;
            buffAttachApplyData.ConfigID = configId;
            evt.EventManager.instance.Send<BuffAttachApplyData>(evt.EventGroup.BUFF, (short)evt.BuffEvent.BUFF_ATTACH_APPLY, buffAttachApplyData);
        }
    }
}