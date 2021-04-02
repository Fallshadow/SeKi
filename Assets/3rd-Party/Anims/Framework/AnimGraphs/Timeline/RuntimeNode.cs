using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.AnimGraphs
{

    public abstract class RuntimeNode : System.IDisposable
    {
        #region 在时间轴上所需的基础信息
        public bool isActive;
        public int mark;
        public uint intervalStart;
        public uint intervalEnd;
        public int layerIndex { get; protected set; }
        #endregion

        public abstract void Dispose();

        private static int MARK_INDEX = 1;
        protected static int GenMark()
        {
            return MARK_INDEX++;
        }
    }
}
