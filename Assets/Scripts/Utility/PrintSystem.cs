using System.Collections;
using System;
using UnityEngine;

namespace ASeKi.debug
{
    // File - Build Settings - Player Settings - Other Settings
    // 打包的时候去掉SeKiDebug 就不会log影响性能了
    public static class PrintSystem
    {
        public static Action UpdatePrinterEvent;    // 更新当事人
        public static PrintBy OutPutLogger = 0;     // 当事人

        // 按位标志，存储打印log的人
        [Flags]
        public enum PrintBy
        {
            SunShuChao = 1 << 1,
            none = 1 << 2,
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        public static void LogWithoutWriter(string str)
        {
            Debug.Log($"{str}");
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        public static void Log(string str, PrintBy pb = PrintBy.none)
        {
            if(isSelectedPrinter(pb))
            {
                string title = printerTitle(pb);
                Debug.Log($"{title} {str}");
            }
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        public static void Log(string str, Color color, PrintBy pb = PrintBy.none)
        {
            if(isSelectedPrinter(pb))
            {
                string colorCode = ColorUtility.ToHtmlStringRGB(color);
                string title = printerTitle(pb);
                Debug.Log($"<color=#{colorCode}>{title}{str}</color>");
            }
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        public static void LogWarning(string str, PrintBy pb = PrintBy.none)
        {
            if(isSelectedPrinter(pb))
            {
                string title = printerTitle(pb);
                Debug.LogWarning($"{title}{str}");
            }
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        public static void LogWarning(string str, Color color)
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(color);
            Debug.LogWarning($"<color=#{colorCode}>{str}</color>");
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        public static void LogError(string str, PrintBy pb = PrintBy.none)
        {
            if(isSelectedPrinter(pb))
            {
                string title = printerTitle(pb);

                Debug.LogError($"{title}{str}");
            }
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        public static void LogError(string str, Color color, PrintBy pb = PrintBy.none)
        {
            if(isSelectedPrinter(pb))
            {
                string title = printerTitle(pb);

                string colorCode = ColorUtility.ToHtmlStringRGB(color);
                Debug.LogError($"{title}<color=#{colorCode}>{str}</color>");
            }
        }

        [System.Diagnostics.Conditional("SeKiDebug")]
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }

        // 如果是false打印信息
        [System.Diagnostics.Conditional("SeKiDebug")]
        public static void Assert(bool condition, string str)
        {
            Debug.Assert(condition, str);
        }

        // 判断是否是选中之人
        private static bool isSelectedPrinter(PrintBy pb)
        {
            UpdatePrinterEvent?.Invoke();
            return (OutPutLogger & pb) == pb;
        }

        private static string printerTitle(PrintBy pb)
        {
            return pb == PrintBy.none ? "" : $"[{pb}]";
        }
    }
}