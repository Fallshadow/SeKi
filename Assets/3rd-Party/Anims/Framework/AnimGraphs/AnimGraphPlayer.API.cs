using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Playables;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {
        #region Animator API
        public void Rebind()
        {
            if (animator == null)
                throw new System.Exception("AnimGraphPlayer 未进行初始化，不可调用接口");
            animator.Rebind();
        }

        public void ResetTrigger(string name)
        {
            if (animator == null)
                throw new System.Exception("AnimGraphPlayer 未进行初始化，不可调用接口");
            animator.ResetTrigger(name);
        }

        public bool enabled
        {
            set
            {
                if (animator == null)
                    throw new System.Exception("AnimGraphPlayer 未进行初始化，不可调用接口");
                animator.enabled = value;
            }
            get
            {
                if (animator == null)
                    throw new System.Exception("AnimGraphPlayer 未进行初始化，不可调用接口");
                return animator.enabled;
            }
        }

        public bool applyRootMotion
        {
            set
            {
                if (animator == null)
                    throw new System.Exception("AnimGraphPlayer 未进行初始化，不可调用接口");
                animator.applyRootMotion = value;
            }
            get
            {
                if (animator == null)
                    throw new System.Exception("AnimGraphPlayer 未进行初始化，不可调用接口");
                return animator.applyRootMotion;
            }
        }
        #endregion

        #region Handle查询API

        public void GetNodes(List<RuntimeNode> nodes)
        {
            uint uintEndTime = localTime;
            uint uintStartTime = (uint)math.max((int)localTime - (int)deltaTime, 0);
            timeline.IntersectsWithRange(uintStartTime, uintEndTime, nodes);
        }

        public int CurrentState(int layerIndex)
        {
            ref var layer = ref layers[layerIndex];

            if (layer.nodes.Length == 0)
                return 0;
            if(layer.nodes.Length == 1)
                return (layer.nodes[0].Target as RuntimePlayNode).stateName;
            return layer.stateName;
        }

        public int NextState(int layerIndex)
        {
            ref var layer = ref layers[layerIndex];

            return layer.dstStateName;
        }
        
        public string CurrentStateName(int layerIndex)
        {
            ref var layer = ref layers[layerIndex];
            int stateName = 0;
            if (layer.nodes.Length == 0)
                return "";
            if (layer.nodes.Length == 1)
            {
                stateName = (layer.nodes[0].Target as RuntimePlayNode).stateName;
            }
            else
            {
                stateName = layer.stateName;
            }
            
#if UNITY_EDITOR //AnimGraph
            return stateNameDict[stateName];
#else
            return $"Hash {stateName}";
#endif
        }

        public float NormalizedTime(int layerIndex,int stateName)
        {
            ref var layer = ref layers[layerIndex];
            if(layer.handleMapping.TryGetValue(stateName, out NodeHandle value) == false)
                return 0;
            return value.normalizedTime;
        }

        public bool IsTransition(int layerIndex)
        {
            ref var layer = ref layers[layerIndex];
            return layer.isTransition;
        }

        public void GetWeights(int layerIndex,List<AnimationClipWeight> weights)
        {
            ref var layer = ref layers[layerIndex];
            for (int i = 0; i < layer.nodes.Length; i++)
            {
                var n = layer.nodes[i].Target as RuntimePlayNode;
                if (n == null)
                    continue;
                n.GetWeights(weights);
            }
        }
        #endregion

        #region 外部API
        
#if UNITY_EDITOR //AnimGraph
        public static Dictionary<int, string> stateNameDict = new Dictionary<int, string>(128);
#endif
        public Animator GetAnimator()
        {
            return animator;
        }

        public Transform GetHeirachyRoot()
        {
            if (animator == null)
            {
                return null;
            }
            
            return hierachyRoot == null ? animator.transform : hierachyRoot;
        }
        
        public float GetStateSpeed(string stateName)
        {
            return GetStateSpeed(stateName.HashCode());
        }

        public float GetStateSpeed(int stateName)
        {
            if(stateSpeedMapping.TryGetValue(stateName,out float speed))
            {
                return speed;
            }
            return 1;
        }

        public void SetStateSpeed(string stateName,float speed)
        {
            SetStateSpeed(stateName.HashCode(), speed);
        }

        public void SetStateSpeed(int stateName, float speed)
        {
            if(stateSpeedMapping.ContainsKey(stateName) == false)
            {
                stateSpeedMapping.Add(stateName, 1);
            }
            stateSpeedMapping[stateName] = speed;
        }
        /// <summary>
        /// 使用Controller改变逻辑，有如下约定：
        /// 1. 原有的Controller与新替换的Controller的Layer名称和层级关系一致
        /// 2. 原有的Controller与新替换的Controller的Parameter列表的名称和序列关系一致
        /// 3. Controller对应的指针，不会被GC回收，确保生命周期内不会访问野指针
        /// </summary>
        public void ChangeAnimatorController(byte* ptr)
        {
            controller = new AnimGraphController(ptr);
        }

        // 要中断的层级
        public void Interrupt(int layer,float duration = 0)
        {
            if (layer == 0)
                Debug.LogWarning($"层级 {layer} 不应该有中断特性调用!");
            
            timeline.RemoveOldLayerTransitionNode(layer);

            var node = timeline.Peek(layer);
           
#if ANIMGRAPHS            
            Debug.Log($"Node LayerOut ");
#endif            
            if (node is RuntimePlayNode playNode && !playNode.nodeAsset.motionStateAsset.fadeOutLayerTtransition.isNull)
            {
#if ANIMGRAPHS
                Debug.Log($"Node LayerOut {stateNameDict[playNode.stateName]}");
#endif                
                AddRuntimeLayerTransitionNode(bodyLayer, localTime, playNode.nodeAsset.motionStateAsset.fadeOutLayerTtransition.duration, LayerTransitionType.FadeOut, layer);
            }
            else
            {
                AddRuntimeLayerTransitionNode(bodyLayer, localTime, duration.ToUintTime(), LayerTransitionType.FadeOut, layer);
            }
            
            //AddRuntimeLayerTransitionNode(bodyLayer, localTime, duration.ToUintTime(), LayerTransitionType.FadeOut, layer);
        }

        public NodeHandle PlayState(string stateName)
        {
#if ANIMGRAPHS            
            Debug.Log($"动画状态压栈 {Time.frameCount} => {stateName}");
#endif

#if UNITY_EDITOR //AnimGraph
            if (stateNameDict.ContainsKey(stateName.HashCode()) == false)
                stateNameDict.Add(stateName.HashCode(), stateName);
#endif            
            
            return PlayState(stateName.HashCode());
        }
        
        public NodeHandle PlayState(int stateName)
        {
            var nodeAsset = controller.GetNodeAsset(stateName);
            if (nodeAsset.isNull)
            {
#if UNITY_EDITOR //AnimGraph
                if (stateNameDict.TryGetValue(stateName, out string clipName))
                    Debug.LogError($"动画系统播放的状态 {clipName} 不存在");
#endif        
                return null;
            }

            var motionStateAsset = nodeAsset.motionStateAsset;
            if (motionStateAsset.stateName != stateName)
            {
                Debug.LogError($"传入的runtimeNodeAssetID {stateName} 与获得节点配置信息对应的ID {motionStateAsset.stateName} 不匹配");
                return null;
            }

            //if (builder != null)
            //    builder.SyncSetting();

            PlayCommand cmd = new PlayCommand();
            cmd.cmdType = PlayCommandType.Play;
            cmd.nodeAsset = nodeAsset;

            ref var layer = ref layers[motionStateAsset.layerIndex];
            layer.readyPlayQueue.Add(cmd);

            NodeHandle handle;
            if (layer.handleMapping.TryGetValue(stateName, out handle))
            {
                layer.handleMapping.Remove(stateName);
            }
            handle = new NodeHandle();
            layer.handleMapping.Add(stateName, handle);
            return handle;
        }

        private int currentFrame = -1;
        public void Evaluate(float deltaTime)
        {
            //Debug.LogError($"{GetHashCode()} [AnimGraphPlayer] Evaluate {deltaTime} {Time.realtimeSinceStartup} {Time.frameCount}");
            
            if(currentFrame == GetFrameCount())
            {
                Debug.LogError($"动画系统的Evaluate不可以同一帧调用多次!");
            }
            currentFrame = GetFrameCount();
            this.deltaTime = (speed * deltaTime).ToUintTime();
            localTime += this.deltaTime;
        }

        public int GetFrameCount()
        {
            bool isNormal = this.animator.updateMode == AnimatorUpdateMode.Normal;
            
            return isNormal ? Time.frameCount : Mathf.RoundToInt(Time.fixedTime / Time.fixedDeltaTime);
        }
        
        public void EvaluateAt(float localTime)
        {
            this.localTime = localTime.ToUintTime();
            this.deltaTime = (1f / FPS).ToUintTime();
        }

        public float speed { get; set; } = 1f;
        #endregion

        #region Parameter API
        public void SetBool(int id, bool value, string name = "")
        {
            if (id < 0 || id >= parameters.Length)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数id不存在 {name}");
                return;
            }

            ref var p = ref parameters[id];

            if (p.type != ParameterType.Bool)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数type错误 {name}");
                return;
            }

            p.value = value;
        }

        public void SetFloat(int id, float value, string name = "")
        {
            if (id < 0 || id >= parameters.Length)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数id不存在 {name}");
                return;
            }

            ref var p = ref parameters[id];

            if (p.type != ParameterType.Float)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数type错误 {name}");
                return;
            }

            p.value = value;
        }

        public void SetInt(int id,int value, string name = "")
        {
            if (id < 0 || id >= parameters.Length)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数id不存在 {name}");
                return;
            }

            ref var p = ref parameters[id];

            if (p.type != ParameterType.Int)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数type错误 {name}");
                return;
            }

            p.value = value;
        }

        public bool GetBool(int id, string name = "")
        {
            if (id < 0 || id >= parameters.Length)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数id不存在 {name}");
                return false;
            }

            ref var p = ref parameters[id];

            if(p.type != ParameterType.Bool)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数type错误 {name}");
                return false;
            }

            return p.value.BoolValue;
        }
        public float GetFloat(int id, string name = "")
        {
            if (id < 0 || id >= parameters.Length)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数id不存在 {name}");
                return -1f;
            }

            ref var p = ref parameters[id];

            if (p.type != ParameterType.Float)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数type错误 {name}");
                return -1f;
            }
            return p.value.FloatValue;
        }
        public int GetInt(int id, string name = "")
        {
            if (id < 0 || id >= parameters.Length)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数id不存在 {name}");
                return -1;
            }

            ref var p = ref parameters[id];

            if (p.type != ParameterType.Int)
            {
                Debug.LogError($"AnimGraphPlayer 获取的参数type错误 {name}");
                return -1;
            }
            return p.value.IntValue;
        }

        public int GetParmeterId(string parmeter)
        {
            var h = Animator.StringToHash(parmeter);
            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                if (p.hash == h)
                    return i;
            }
            return -1;
        }

        public int GetParmeterId(int h)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                var p = parameters[i];
                if (p.hash == h)
                    return i;
            }
            return -1;
        }
        
        public int layerCount
        {
            get
            {
                return layers.Length;
            }
        }
        public void SetLayerWeight(int layer,float w)
        {
            w = math.clamp(w, 0, 1);

            if (layer >= layers.Length)
            {
                Debug.LogError($"Not Exist LayerIndex {layer}");
                return;
            }
                        
            layers[layer].LayerWeight = w;
            bodyLayer.SetInputWeight(layer, layers[layer].LayerWeight * layers[layer].InputWeight);
        }
        public float GetLayerWeight(int layerIndex)
        {
            return bodyLayer.GetInputWeight(layerIndex);
        }
        public int GetLayerIndex(string layerName)
        {
            int h = layerName.HashCode();
            for (int i = 0; i < layers.Length; i++)
            {
                var l = layers[i];
                if (l.layerAsset.layerHash == h)
                    return i;
            }
            return -1;
        }
        #endregion

        #region Rig

        public bool RegisterRigBuilder()
        {
            if (builder == null)
            {
                builder = animator.gameObject.GetComponent<AnimGraphRigBuilder>();

                if (builder == null)
                {
                    builder = animator.gameObject.AddComponent<AnimGraphRigBuilder>();

                    builder.enabled = true;
                    builder.Build(m_Graph);
                    return true;
                }
            }

            if (builder != null)
            {
                builder.enabled = true;
                builder.Build(m_Graph);
                return true;
            }
            return false;
        }

        public bool RemoveRigBuilder()
        {
            if (builder == null)
                return false;
            GameObject.Destroy(builder);
            builder = null;
            return true;
        }

        #endregion

        #region 内部API
        internal int OverrideType
        {
            get
            {
                return overrideControllerType;
            }
        }

        internal void Process()
        {
            //Debug.Log("[UpdateGraph] Process");
            
//            if (builder != null)
//            {
//                builder.riggingSyncer.FixRigTransforms();
//            }
            
            PreProcessCommand();
            PreProcessTimeline();
            PreProcessGraph();
            ProcessNode();
            ProcessHandle();
        }

        public void LateUpdate()
        {
            //Debug.LogError($"{GetHashCode()} [AnimGraphPlayer] LateUpdate {deltaTime} {Time.realtimeSinceStartup} {Time.frameCount}");
            PostProcessGraph();
            PostProcessTimeline();
            PostPrecessClean();
        }

        internal AvatarMask GetBodyLayerMask()
        {
            const int BASE_INDEX = 0;
            var layerAssets = controller.GetLayerAssets();
            return layerAssets.Length == 0 ? null : layerAssets[BASE_INDEX].avatarMask(0);
        }
        
        #endregion
    }
}
