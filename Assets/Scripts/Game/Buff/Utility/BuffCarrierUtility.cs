using UnityEngine;
using System.Collections.Generic;

namespace ASeKi.battle
{
    public static class BuffCarrierUtil
    {
        const int SEQUENCE_OFFSET = 16;                         // 动态id基于user id的左偏移

        public static uint GetCarrierIdByDynamicId(uint dynamicId)
        {
            return dynamicId >> SEQUENCE_OFFSET;
        }   
        
        public static ulong GetDynamicIdCarrierPart(ulong carrier)
        {
            return carrier << SEQUENCE_OFFSET;
        }
    }
}