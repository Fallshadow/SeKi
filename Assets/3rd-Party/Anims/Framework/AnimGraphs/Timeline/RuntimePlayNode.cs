using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    /// <summary>
    /// 动画的播放节点
    /// 通过代理Motion获得最终节点播放组织结构
    /// </summary>
    public unsafe class RuntimePlayNode : RuntimeNode
    {
        #region 预处理时获取的数据
        // 节点数据句柄
        public NodeHandle nodeHandle;
        // 节点配置信息
        public NodeAsset nodeAsset { get; protected set; }
        // 节点状态名称
        public int stateName { get; protected set; }
        #endregion

        #region 连接时的数据
        // 动画节点
        private Motion motion;
        // 动画节点所属播放器
        public AnimGraphPlayer player;
        // 当前节点的上一级节点
        public AnimationMixerPlayable parent;
        // 当前节点的InputPort
        public int inputPort { get; protected set; }

        public uint localTime { get; protected set; }

        #endregion

        #region 运行时
        public bool isLooping => motion.isLooping;
        public uint duration => motion.duration;
        public int loopCount = 0;
        private float normalizedTime = 0;

        public uint fixedTime {
            get {
                return (motion.duration.ToFloatTime() * normalizedTime).ToUintTime();
            }
        }

        public bool isConnected { get; protected set; }
        #endregion
        /// <summary>
        /// PlayNode的预处理函数
        /// 在添加到Timeline容器之前被调用
        /// </summary>
        public void Preparatory(AnimGraphPlayer player, int layerIndex, uint startTime, ref NodeAsset nodeAsset, NodeHandle nodeHandle,float offset)
        {
            var stateAsset = nodeAsset.motionStateAsset;
            var motionAsset = stateAsset.motion;
            mark = GenMark();
            this.layerIndex = layerIndex;
            this.nodeAsset = nodeAsset;
            intervalStart = math.max(0, startTime - (uint)(motionAsset.avgDuration * offset));
            intervalEnd = intervalStart + motionAsset.avgDuration;
            isActive = true;
            stateName = stateAsset.stateName;
            this.nodeHandle = nodeHandle;
            localTime = 0;
            loopCount = 0;
            normalizedTime = 0;

            switch (motionAsset.motionType)
            {
                case MotionType.AnimationClip:
                    //TODO: 修改为对象池
                    motion = new AnimationClipMotion();
                    break;
                case MotionType.BlendTree:
                    //TODO: 修改为对象池
                    motion = new BlendTreeMotion();
                    break;
                default:
                    throw new System.Exception($"类型分支未实现 {motionAsset.motionType}");
            }
            motion.Preparatory(ref motionAsset, stateAsset.speed * player.GetStateSpeed(stateAsset.stateName));
        }

        public void SetWeight(float weight)
        {
            parent.SetInputWeight(inputPort, weight);
        }

        public bool parentViald()
        {
            if (parent.IsValid())
            {
                return parent.GetInputCount() > 0;
            }
            return false;
        }

        public float GetWeight()
        {
            return parent.GetInputWeight(inputPort);
        }

        public void GetWeights(List<AnimationClipWeight> weights)
        {
            motion.GetWeights(1, stateName, weights);
        }

        public uint GetDeltaTime(uint exitTime)
        {
            uint delta = 0;
            var lt = fixedTime;
            if (exitTime > lt)
                delta = exitTime - lt;
            else
                delta = motion.duration - (lt - exitTime);
            return delta;
        }

        public void Connecting(AnimGraphPlayer player,AnimationMixerPlayable mixer, int inputPort)
        {
            this.inputPort = inputPort;
            parent = mixer;
            var stateAsset = nodeAsset.motionStateAsset;
            var motionAsset = stateAsset.motion;
            motion.Connecting(player, mixer, inputPort);
            motion.SetFootIK(stateAsset.iKOnFeet);
            loopCount = 0;
            isConnected = true;
        }

        public void Disconnected()
        {
            if (isConnected == false)
                return;
            motion.Disconnected();
            isConnected = false;
        }

        public void InternalUpdate(uint localTime)
        {
            var duration = motion.duration.ToFloatTime();
            var deltaTime = (localTime - this.localTime).ToFloatTime();
            var durationTime = duration * normalizedTime + deltaTime;
            normalizedTime = durationTime / duration;

            this.localTime = localTime;
            motion.Update(normalizedTime, loopCount);
            if (motion.isLooping)
            {
                while (normalizedTime >= 1f)
                {
                    normalizedTime -= 1f;
                    loopCount++;
                }
                intervalEnd = uint.MaxValue;
            }
            else
                intervalEnd = intervalStart + motion.duration;
        }

        public void HandleUpdate(uint localTime)
        {
            nodeHandle.normalizedTime = math.clamp(normalizedTime,0f,1f);
        }

        public override void Dispose()
        {
            Disconnected();
            motion.Dispose();
            nodeAsset = default;
            nodeHandle = null;
            stateName = 0;
            motion = null;
            player = null;
            parent = default;
            inputPort = -1;
            localTime = 0;
            loopCount = 0;
            normalizedTime = 0;
            isActive = false;
        }

        #if UNITY_EDITOR

        ~RuntimePlayNode()
        {
            // 析构函数检测回收是否正确
            if (motion != null)
                Debug.LogError($"RuntimePlayNode未正确回收");
        }

        #endif
    }
}