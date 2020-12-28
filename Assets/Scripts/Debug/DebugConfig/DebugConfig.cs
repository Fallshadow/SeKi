using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.debug
{
    public class DebugConfig : SingletonMonoBehavior<DebugConfig>
    {
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