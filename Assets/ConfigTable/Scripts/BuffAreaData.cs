using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ConfigTable
{
    public class BuffAreaData
    {
        // 编号
        public int id;
        // 警戒范围类型
        public int area_type;
        // 类型参数1
        public int area_param1;
        // 类型参数2
        public int area_param2;
        // 类型参数3
        public int area_param3;
        // 类型参数4
        public int area_param4;
        // 原点X轴偏移
        public int offset_x;
        // 原点Y轴偏移
        public int offset_y;
        // 原点Z轴偏移
        public int offset_z;
        // X旋转偏移
        public int rotate_x;
        // Y旋转偏移
        public int rotate_y;
        // Z旋转偏移
        public int rotate_z;
        public BuffAreaData(int configID)
        {
            id = configID;
        }
    }
}
