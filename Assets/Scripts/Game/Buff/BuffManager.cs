using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.game
{
    public class BuffManager : Singleton<BuffManager>
    {
        private BuffAttachApplyData buffAttachApplyData = new BuffAttachApplyData();

        private Dictionary<ulong, Carrier> dictCarrier = new Dictionary<ulong, Carrier>();

        public uint AttachBuff(ulong source, ulong carrier, int configId, int curSkillId = 0)
        {
            if(configId == 0)
            {
                return 0;
            }

            SendAttachApply(source, carrier, configId);

            if(buffAttachApplyData.Refuse)
            {
                return 0;
            }
            return getCarrier(carrier).AttachBuff(source, carrier, configId, curSkillId);
        }

        

        // 申请挂载BUFF，得到的回应储存在BuffAttachApplyData中，游戏中保持一份，防止内存占用，由于也是立即使用所以也不用太过担心
        private void SendAttachApply(ulong source, ulong carrier, int configId)
        {
            buffAttachApplyData.Reset();
            buffAttachApplyData.Source = source;
            buffAttachApplyData.Carrier = carrier;
            buffAttachApplyData.ConfigID = configId;
            evt.EventManager.instance.Send<BuffAttachApplyData>(evt.EventGroup.BUFF, (short)evt.BuffEvent.BUFF_ATTACH_APPLY, buffAttachApplyData);
        }

        // 根据ID获得/创建载体
        private Carrier getCarrier(ulong carrierId)
        {
            if(!dictCarrier.TryGetValue(carrierId, out Carrier carrier))
            {
                carrier = new Carrier(carrierId);
                dictCarrier[carrierId] = carrier;
            }

            return carrier;
        }
    }
}