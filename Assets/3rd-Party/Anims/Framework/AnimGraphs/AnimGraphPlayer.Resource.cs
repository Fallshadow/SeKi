using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Animations.Rigging;
using UnityEngine.Experimental.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {
        #region 动画系统逻辑控制相关
        private AnimGraphController controller;
        // 透传给加载器的分类，用于代理替换资源加载
        private int overrideControllerType;
        #endregion

        #region Playable相关
        private PlayableGraph m_Graph;
        // 输出端节点
        private AnimationPlayableOutput output;
        // 身层融合节点
        private AnimationLayerMixerPlayable bodyLayer;
        // 整个动画的控制节点
        private ScriptPlayable<PlayerControlPlayable> controlPlayable;
        #endregion

        #region 动画系统驱动
        public string name {
            get;
            private set;
        }
        // 帧率
        private const int FPS = 30;

        //local Time Start From
        //To prevent first clip has a huge offset
        //the uint will be wrong
        private const uint DEFAULT_LOCAL_START = 10000;
        
        // 当前对象控制的角色
        private Animator animator;
        private Transform hierachyRoot;
        public AnimatorUpdateMode UpdateMode => animator != null ? animator.updateMode : AnimatorUpdateMode.Normal;
        
        // 当前Player的持续播放时间
        private uint localTime;
        // 当前帧的间隔时间
        private uint deltaTime;
        #endregion

        #region 动画系统相关数据容器
        // 时间线数据容器
        private TimelineContainer timeline;
        private Layer[] layers;
        private ParameterValue[] parameters;

        // 当前时间片内的节点
        private List<RuntimeNode> currentTimeNodes;
        // 当前时间片内的PlayNode列表
        private List<RuntimePlayNode> currentTimePlayNodes;
        // 当前时间片内的Transition列表
        private List<RuntimeTransitionNode> currentTimeTransitionNodes;
        // 当前时间片内的TransitionLayer列表
        private List<RuntimeLayerTransitionNode> currentTimeLayerTransitionNodes;

        private List<RuntimePlayNode> currentRemovePlayNodes;

        private Dictionary<int,RuntimePlayNode> currentRuntimePlayNodes;

        private Dictionary<int, float> stateSpeedMapping;
        #endregion

        private AnimGraphRigBuilder builder;
        
        private Queue<IDisposable> _disposables;

        public Queue<IDisposable> Disposables => _disposables ?? (_disposables = new Queue<IDisposable>());

    }
}