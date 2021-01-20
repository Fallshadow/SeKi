using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigTable
{
    public struct BuffAttrConfig
    {
        // 属性效果类型
        public int attr_config;
        // 固定值
        public int attr_param1;
        // 固定值随等级
        public int attr_param2;
        // 百分比
        public int attr_param3;
        // 百分比随等级
        public int attr_param4;
        // 属性效果延迟
        public int attr_delaytime;
    }
}
