using UnityEditor;
using ASeKi.battle;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebugEditor : EditorWindow
{
    public struct BuffInfo
    {
        public int buffId;
        public string buffName;
        public string buffCarrierName;
        public ulong buffCarrierId;
        public ulong buffSourceId;
        public string buffSourceName;
        public string life;                 // 剩余存活时间
        public string buffEffect;
        public uint dynamicId;
        public int LayerNum;
        public string Desc;
    }

    public class StatusInfo
    {
        public ulong EntityId;
        public string EntityName;
        public string Describe;
    }

    public class MenuData
    {
        public ulong data1;
        public int data2;
    }

    private List<BuffInfo> buffInfos = new List<BuffInfo>();

    [MenuItem("GameEditor/Battle/Buff/Buff系统Debug工具")]
    private static void ShowWindow()
    {
        var window = GetWindow<BuffDebugEditor>();
        window.titleContent = new GUIContent("BuffDebug");
        window.Show();
    }

    [MenuItem("GameEditor/Battle/Buff/Buff系统Debug工具",true)]
    private static bool ShowWindow_Valifunc()
    {
        if(EditorApplication.isPlaying)
        {
            return true;
        }
        Debug.LogError("只有在运行状态下才能调试BUFF");
        return false;
    }
}
