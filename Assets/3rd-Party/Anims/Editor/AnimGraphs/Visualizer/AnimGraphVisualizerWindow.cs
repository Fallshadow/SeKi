using Framework.AnimGraphs;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class AnimGraphVisualizerWindow : EditorWindow
{
    public class QueueData
    {
        public string stateName;
        public float endingTime;
    }

    [MenuItem("Rex/AnimGraphs/Visualizer",     false,3940000)]
    public static void ShowWindow()
    {
        GetWindow<AnimGraphVisualizerWindow>();
    }

    private GUIStyle m_NodeRectStyle;

    private const float historyTime = 5f;
    
    private void OnEnable()
    {
        m_NodeRectStyle = new GUIStyle
        {
            normal =
                {
                    background = (Texture2D)Resources.Load("Node"),
                    textColor = Color.black,
                },
            border = new RectOffset(10, 10, 10, 10),
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            clipping = TextClipping.Clip
        };

        nodes = new List<RuntimeNode>(10);
        playNodes = new List<RuntimePlayNode>(10);
        transitionNodes = new List<RuntimeTransitionNode>(10);
        left = new List<RuntimePlayNode>(10);
        right = new List<RuntimePlayNode>(10);
        taskQueue = new List<QueueData>(10);
        lastMapping = new Dictionary<int, string>(10);
    }

    void Update()
    {
        if (EditorApplication.isPlaying)
            Repaint();
    }

    private void DrawNode(string stateName, float x, float y, float width, float height, float normalTime)
    {
        GUI.Box(new Rect(x, y, width, height), stateName, m_NodeRectStyle);
        GUIStyle progressBackground = "MeLivePlayBackground";
        GUIStyle bar = "MeLivePlayBar";
        if (Event.current.type == EventType.Repaint && normalTime >= 0)
        {
            float x2 = x + 4f;
            float y2 = y + height - 20;
            float w2 = width - 8;
            bar.Draw(new Rect(x2, y2, w2 * normalTime, 5), false, false, false, false);
            progressBackground.Draw(new Rect(x2, y2, w2, 5), false, false, false, false);
        }
    }

    private void DrawLine(Vector2 start, Vector2 end)
    {
        Vector3[] points, tangents;
        GetTangents(start, end, out points, out tangents);
        Handles.DrawBezier(points[0], points[1], tangents[0], tangents[1], Color.white, null, 5f);
    }

    private void GetTangents(Vector2 start, Vector2 end, out Vector3[] points, out Vector3[] tangents)
    {
        points = new Vector3[] { start, end };
        tangents = new Vector3[2];

        const float minTangent = 30;
        const float weight = 0.5f;
        float cleverness = Mathf.Clamp01(((start - end).magnitude - 10) / 50);
        tangents[0] = start + new Vector2((end.x - start.x) * weight + minTangent, 0) * cleverness;
        tangents[1] = end + new Vector2((end.x - start.x) * -weight - minTangent, 0) * cleverness;
    }

    private AnimGraphPlayer player;
    private List<RuntimeNode> nodes;

    private List<RuntimePlayNode> playNodes;
    private List<RuntimeTransitionNode> transitionNodes;

    private List<RuntimePlayNode> left;
    private List<RuntimePlayNode> right;

    private List<QueueData> taskQueue;
    private Dictionary<int, string> lastMapping;

    private int index = 0;
    void OnGUI()
    {
        if (EditorApplication.isPlaying == false || AnimGraphContainer.Instance.Count == 0)
        {
            AnimGraphContainer.Instance.Clear();
            return;
        }

        string[] menuString = new string[AnimGraphContainer.Instance.Count];
        for (int i = 0; i < AnimGraphContainer.Instance.Count; i++)
        {
            string currentName = AnimGraphContainer.Instance.Get(i).name;
            if(menuString.Contains(currentName))
                menuString[i] = currentName + AnimGraphContainer.Instance.Get(i).GetHashCode();
            else
            {
                menuString[i] = currentName;
            }
        }
        index = EditorGUI.Popup(new Rect(0, 0, 200, 60), index, menuString);
        player = AnimGraphContainer.Instance.Get(index);

        nodes.Clear();
        playNodes.Clear();
        transitionNodes.Clear();
        left.Clear();
        right.Clear();
        player.GetNodes(nodes);

        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];

            if(node is RuntimePlayNode)
            {
                playNodes.Add(node as RuntimePlayNode);
                lastMapping.Remove(node.GetHashCode());
                continue;
            }
            if(node is RuntimeTransitionNode)
            {
                transitionNodes.Add(node as RuntimeTransitionNode);
                continue;
            }
        }

        foreach (var item in lastMapping.Values)
        {
            var q = new QueueData();
            q.stateName = item;
            q.endingTime = Time.time;
            taskQueue.Add(q);
        }
        lastMapping.Clear();

        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i] as RuntimePlayNode;
            if (node != null)
            {
                lastMapping.Add(node.GetHashCode(), AnimGraphPlayer.stateNameDict[node.stateName]);
                continue;
            }
        }

        RuntimeTransitionNode trans;
        if(transitionNodes.Count > 0)
        {
            trans = transitionNodes[transitionNodes.Count - 1];
            for (int i = 0; i < playNodes.Count; i++)
            {
                var node = playNodes[i];
                if(node.stateName == trans.destination)
                {
                    right.Add(node);
                }
                else
                {
                    left.Add(node);
                }
            }
        }
        else
        {
            for (int i = 0; i < playNodes.Count; i++)
            {
                var node = playNodes[i];
                left.Add(node);
            }
        }

        

        int x = 0;
        int y = 100;
        for (int i = 0; i < left.Count; i++)
        {
            var playNode = left[i];
            string stateName = "";
            if (AnimGraphPlayer.stateNameDict.ContainsKey(playNode.stateName))
            {
                stateName = AnimGraphPlayer.stateNameDict[playNode.stateName];
            }
            var px = x;
            var py = y + i * 100 + i * 50;
            DrawNode(stateName, px, py, 100, 100,playNode.nodeHandle.normalizedTime);

            if(right.Count > 0)
            {
                DrawLine(new Vector2(px + 100, py + 50), new Vector2(150, 150));
            }
        }

        x = 150;
        for (int i = 0; i < right.Count; i++)
        {
            var playNode = right[i];
            string stateName = "";
            if (AnimGraphPlayer.stateNameDict.ContainsKey(playNode.stateName))
            {
                stateName = AnimGraphPlayer.stateNameDict[playNode.stateName];
            }
            var px = x;
            var py = y + i * 100 + i * 50;
            DrawNode(stateName, px, py, 100, 100, playNode.nodeHandle.normalizedTime);
        }

        // 绘制历史记录
        for (int i = 0; i < taskQueue.Count; i++)
        {
            if(Time.time - taskQueue[i].endingTime > historyTime)
            {
                taskQueue.RemoveAt(i);
            }
        }

        y = 600;
        x = 0;
        for (int i = 0; i < taskQueue.Count; i++)
        {
            var playNode = taskQueue[i];
            var px = x + i * 100 + i * 50;
            var py = y ;
            DrawNode(playNode.stateName, px, py, 100, 100,-1f);
        }
    }
}
