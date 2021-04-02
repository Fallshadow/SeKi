using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer : System.IDisposable
    {
        public AnimGraphPlayer(AnimGraphPlayerConfig data)
        {
            // ptr为序列化数据头
            controller = new AnimGraphController(data.assetPtr);
            overrideControllerType = data.overrideControllerType;
            // 时间参数初始化
            localTime = DEFAULT_LOCAL_START;
            deltaTime = 0;
            
            // 先初始化animator
            animator = data.animator;
            hierachyRoot = data.GetHierachyRoot();
            
            name = animator.gameObject.name;
            
            // Graph初始化
            m_Graph = GraphCreater.Instance.GenerateGraph(this);
            
            // 数据容器
            const int MAX = 10;
            timeline = new TimelineContainer();
            currentTimeNodes = new List<RuntimeNode>(MAX);
            currentTimePlayNodes = new List<RuntimePlayNode>(MAX);
            currentTimeTransitionNodes = new List<RuntimeTransitionNode>(MAX);
            currentTimeLayerTransitionNodes = new List<RuntimeLayerTransitionNode>(MAX);
            currentRemovePlayNodes = new List<RuntimePlayNode>(MAX);
            currentRuntimePlayNodes = new Dictionary<int, RuntimePlayNode>(MAX);
            stateSpeedMapping = new Dictionary<int, float>(MAX);

            InitParameter();
            InitPipeline();

            InitLayer(data);
            
            //paste this with AnimationPlayableUtilities.Play
            //see the link : https://forum.unity.com/threads/custom-playable-graph-with-animate-physics-turned-on.723437/
            InitOutPut();
            //m_Graph.Play();
            
            AnimGraphContainer.Instance.Register(this);

        }

        private bool cleanedUp = false;
        
        public void Dispose()
        {
            if (cleanedUp)
            {
                Debug.Log("请勿多次销毁AnimGraph!");
                return;
            }
            
            if (builder != null)
            {
                GameObject.DestroyImmediate(builder);
                builder = null;
            }

            // 清理Layer
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i].Dispose();
            }
            
            destroyDisposeJobs();
            
            if (controlPlayable.IsValid())
            {
                m_Graph.DestroyPlayable(controlPlayable);
            }
            
            //m_Graph.DestroyOutput(output);

            for (int i = 0; i < parameters.Length; i++)
            {
                parameters[i] = default;
            }

            timeline.Dispose();

            GraphCreater.Instance.RemoveGraph(this);
            AnimGraphContainer.Instance.Remove(this);
            
            GC.SuppressFinalize(this);
            cleanedUp = true;
            
            Debug.Log($"AnimGraphPlayer调度了回收函数");
        }

        void destroyDisposeJobs()
        {
            if (_disposables == null)
            {
                return;
            }
            
            while (_disposables.Count != 0)
            {
                _disposables.Dequeue().Dispose();
            }
            
            _disposables = null;
        }
        
        
    }
}