using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.debug
{
    public class DebugConfig : SingletonMonoBehaviorNoDestroy<DebugConfig>
    {
        [Tooltip("AB包加载开关")]    // 默认在制作过程中可以关闭
        public bool DisableAssetBundle = false;
        [Tooltip("显示Debug UI")]
        public bool ShowDebugUI = false;
        [Tooltip("显示FPS")]
        public bool ShowFps = false;
        [Header("----输出日志查看----")]
        public PrintSystem.PrintBy WatchLogger;

        protected override void init()
        {
            updatePrinter();
            PrintSystem.UpdatePrinterEvent = updatePrinter;
        }

        void updatePrinter()
        {
            PrintSystem.OutPutLogger = WatchLogger;
        }
    }
}