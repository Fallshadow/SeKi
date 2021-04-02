using Unity.Collections;
using UnityEngine;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class GraphCreater : System.IDisposable
    {
        public struct GraphStruct
        {
            public AnimatorUpdateMode updateMode;
            public PlayableGraph m_Graph;
            public int Hash;
        }

        private NativeList<GraphStruct> graphs;

        private static GraphCreater instance;
        public static GraphCreater Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GraphCreater();
                }
                return instance;
            }
        }

        private GraphCreater()
        {
            const int MAX = 10;
            graphs = new NativeList<GraphStruct>(MAX, Allocator.Persistent);
        }

        public PlayableGraph GenerateGraph(AnimGraphPlayer player)
        {
            GraphStruct graph = new GraphStruct();
            graph.updateMode = player.UpdateMode;
            graph.Hash = player.GetHashCode();
            
            if (!graph.m_Graph.IsValid())
            {
                graph.m_Graph = PlayableGraph.Create(player.name);
            }
            
            graph.m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            graphs.Add(graph);
            
            return graph.m_Graph;
        }

        public void RemoveGraph(AnimGraphPlayer player)
        {
            var hash = player.GetHashCode();
            for (int i = 0; i < graphs.Length; i++)
            {
                var graph = graphs[i];
                if(hash == graph.Hash)
                {
                    graph.m_Graph.Destroy();
                    graphs.RemoveAtSwapBack(i);
                    return;
                }
            }
        }

        public void Evaluate(AnimatorUpdateMode updateMode = AnimatorUpdateMode.Normal)
        {
            if (updateMode != AnimatorUpdateMode.AnimatePhysics)
                return;
            
            int graphCount = graphs.Length;

            for (int i = 0; i < graphCount; i++)
            {
                var graph = graphs[i];

                if (graph.updateMode != updateMode)
                    continue;

                if (graph.m_Graph.IsValid())
                    graph.m_Graph.Evaluate();
            }
        }

        public void SwitchGraphUpdateMode(int playerHash, AnimatorUpdateMode newMode)
        {
//#if ANIMGRAPH_OPTIMIZE
            Debug.Log("You can't switch mode in [DirectorUpdateMode.GameTime]  !");
//#else
            for (int i = 0; i < graphs.Length; i++)
            {
                var graph = graphs[i];
                if (playerHash == graph.Hash)
                {
                    graph.updateMode = newMode;
                    graphs[i] = graph;
                    break;
                }
            }
                
            Debug.Log($"The Player index: {playerHash} is change UpdateMode to {newMode.ToString()}");
//#endif
        }

        public void Dispose()
        {
            if (graphs.IsCreated)
            {
                for (int i = 0; i < graphs.Length; i++)
                {
                    var graph = graphs[i];
                    if (graph.m_Graph.IsValid())
                        graph.m_Graph.Destroy();
                    graphs[i] = default;
                }
                graphs.Dispose();
            }
        }

#if UNITY_EDITOR
        ~GraphCreater()
        {
            if (graphs.IsCreated)
                Debug.LogError("未正确回收Native内存");
        }
#endif
    }
}
