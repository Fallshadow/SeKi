using System;
using System.Collections.Generic;

namespace ASeKi.battle
{
    class BuffFactory
    {
        private Dictionary<ulong, int> buffDict = new Dictionary<ulong, int>();

        private ASeKi.Utility.ObjectPool<Buff> buffPool = new ASeKi.Utility.ObjectPool<Buff>();
        

        // BUFF 动态ID 由源目标ID和动态递增ID构成 
        public uint GetDynamicBuffId(ulong source)
        {
            int sequenceId = getSequenceNumID(source);
            ulong offsetSource = BuffCarrierUtil.GetDynamicIdCarrierPart(source);
            return (uint)((uint)offsetSource + sequenceId);
        }

        private int getSequenceNumID(ulong source)
        {
            buffDict.TryGetValue(source, out var sequenceId);
            sequenceId++;
            buffDict[source] = sequenceId;
            return sequenceId;
        }

        public Buff CreateBuff(ulong source,ulong carrier,int configID,uint dynamicID)
        {
            Buff buff = buffPool.Get();
            buff.IsHostLogic = BuffSourceUtility.CheckNativeLogic();
            buff.Init(source, carrier, configID, dynamicID);
            return buff;
        }

        public void ReleaseBuff(Buff buff)
        {
            buff.Release();
            evt.EventManager.instance.Send<ulong, int>(evt.EventGroup.BUFF, (short)evt.BuffEvent.BUFF_DETACH, buff.CarrierID, buff.Config.buff_id);
            buffPool.Release(buff);
        }

        public void SetMaxSequence(ulong source, uint dynamicId)
        {
            int sequenceId = (int)(dynamicId & 0x00FF);
            buffDict.TryGetValue(source, out int saveSequenceId);
            saveSequenceId = Math.Max(saveSequenceId, sequenceId);
            buffDict[source] = saveSequenceId;
        }

        public void Clear()
        {
            buffDict.Clear();
            buffPool.Clear();
        }
    }
}