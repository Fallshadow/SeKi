using System;
using System.Collections.Generic;

namespace UnityEngine.Animations.Rigging
{
    using Animations;
    using Playables;
    using Experimental.Animations;

    [RequireComponent(typeof(Animator))]
    [DisallowMultipleComponent, ExecuteInEditMode, AddComponentMenu("Animation Rigging/Setup/AnimGraph Rig Builder")]
    [HelpURL("https://docs.unity3d.com/Packages/com.unity.animation.rigging@latest?preview=1&subfolder=/manual/index.html")]
    public class AnimGraphRigBuilder : MonoBehaviour, IAnimationWindowPreview, IRigEffectorHolder
    {
        [Serializable]
        public class RigLayer
        {
            const int k_InvalidDataIndex = -1;

            public Rig rig;
            public bool active = true;

            [NonSerialized]
            public int data;

            public RigLayer(Rig rig, bool active = true)
            {
                this.rig = rig;
                this.active = active;
                data = k_InvalidDataIndex;
            }

            public void Reset()
            {
                data = k_InvalidDataIndex;
                if (rig != null)
                    rig.Destroy();
            }

            public bool Initialize(Animator animator)
            {
                data = k_InvalidDataIndex;
                if (rig != null)
                    return rig.Initialize(animator);

                return false;
            }

            public bool IsValid() => rig != null && data != k_InvalidDataIndex;
        }

        struct LayerData
        {
            public AnimationPlayableOutput output;
            public AnimationScriptPlayable[] playables;
        }

        private class SyncSceneToStreamLayer
        {
            private Animator animator;

            public bool Initialize(Animator animator, List<RigLayer> layers)
            {
                if (isInitialized)
                    return true;
                this.animator = animator;

                List<Rig> rigs = new List<Rig>(layers.Count);
                m_RigIndices = new List<int>(layers.Count);
                for (int i = 0; i < layers.Count; ++i)
                {
                    if (!layers[i].IsValid())
                        continue;

                    m_RigIndices.Add(i);
                    rigs.Add(layers[i].rig);
                }

                m_Data = RigUtils.CreateSyncSceneToStreamData(animator, rigs.ToArray());
                if (!m_Data.IsValid())
                    return false;

                job = RigUtils.syncSceneToStreamBinder.Create(animator, m_Data);
                return (isInitialized = true);
            }

            public void SyncUpdate(List<RigLayer> layers)
            {
                RigSyncSceneToStreamJob m_job = (RigSyncSceneToStreamJob)job;
                for (int i = 0, count = layers.Count; i < count; ++i)
                {
                    var layer = layers[i];
                    if (layer.active == false)
                        continue;
                    
                    m_job.targetSyncer.BindProperty(animator, layer.rig, "m_Weight", layer.rig.weight);
                    
                    var constraints = layer.rig.constraints;
                    for (int j = 0; j < constraints.Length; j++)
                    {
                        if (constraints[j] is MultiAimConstraint single)
                        {
                            m_job.targetSyncer.BindProperty(animator, single, ConstraintProperties.s_Weight, single.weight);

                            for (int x = 0; x < single.data.sourceObjects.Count; x++)
                            {
                                m_job.targetSyncer.Bind(animator, single.data.sourceObjects[x].transform);
                            }
                        }
                        
                        if (constraints[j] is MultiParentConstraint multiParent)
                        {
                            m_job.targetSyncer.BindProperty(animator, multiParent, ConstraintProperties.s_Weight, multiParent.weight);
                            m_job.targetSyncer.Bind(animator, multiParent.data.constrainedObject);

                            for (int x = 0; x < multiParent.data.sourceObjects.Count; x++)
                            {
                                m_job.targetSyncer.BindProperty(animator, multiParent,
                                    PropertyUtils.ConstructConstraintDataPropertyName("m_SourceObjects") + ".m_Item" + x + ".weight", 
                                    multiParent.data.sourceObjects[x].weight);
                            }
                        }
                        
                        if (constraints[j] is MultiPositionConstraint multiPosition)
                        {
                            m_job.targetSyncer.BindProperty(animator, multiPosition, ConstraintProperties.s_Weight, multiPosition.weight);

                            for (int x = 0; x < multiPosition.data.sourceObjects.Count; x++)
                            {
                                m_job.targetSyncer.Bind(animator, multiPosition.data.sourceObjects[x].transform);
                            }
                        }
                        
                        if (constraints[j] is TwoBoneIKConstraint twobone)
                        {
                            m_job.targetSyncer.Bind(animator, twobone.data.target);
                            m_job.targetSyncer.BindProperty(animator, twobone, ConstraintProperties.s_Weight, twobone.weight);

                            if (twobone.data.hint != null)
                            {
                                m_job.targetSyncer.Bind(animator, twobone.data.hint);
                                m_job.targetSyncer.BindProperty(animator, twobone, PropertyUtils.ConstructConstraintDataPropertyName("m_HintWeight"), twobone.data.hintWeight);
                            }
                        }

                        if (constraints[j] is SyncTransConstraint syncTransConstraint)
                        {
                            syncTransConstraint.OnGraphChange();
                            
                            if (syncTransConstraint.data.constraintObject != null && syncTransConstraint.data.sourceObject != null)
                            {
                                //m_job.targetSyncer.Bind(animator, syncTransConstraint.data.constraintObject);
                                m_job.targetSyncer.Bind(animator, syncTransConstraint.data.sourceObject);
                                m_job.targetSyncer.BindProperty(animator, syncTransConstraint, ConstraintProperties.s_Weight, syncTransConstraint.weight);
                            }
                        }
                        
                        
                    }
                }
            }

            public void Update(List<RigLayer> layers)
            {
                if (!isInitialized || !m_Data.IsValid())
                    return;

                IRigSyncSceneToStreamData syncData = (IRigSyncSceneToStreamData)m_Data;
                for (int i = 0, count = m_RigIndices.Count; i < count; ++i)
                    syncData.rigStates[i] = layers[m_RigIndices[i]].active;

                RigUtils.syncSceneToStreamBinder.Update(job, m_Data);
            }

            public void Destroy()
            {
                if (!isInitialized)
                    return;

                if (m_Data != null && m_Data.IsValid())
                {
                    RigUtils.syncSceneToStreamBinder.Destroy(job);
                    m_Data = null;
                }

                isInitialized = false;
            }

            public bool IsValid() => job != null && m_Data != null;

            public bool isInitialized { get; private set; }

            public IAnimationJob job;

            private IAnimationJobData m_Data;
            private List<int> m_RigIndices;
        }

        [SerializeField]
        private List<RigLayer> m_RigLayers;
        private List<LayerData> m_RigLayerData;

        private List<LayerData> m_PreviewRigLayerData;
        private SyncSceneToStreamLayer m_SyncSceneToStreamLayer;

#if UNITY_2019_3_OR_NEWER
        private static readonly ushort k_AnimationOutputPriority = 1000;
#endif

#if UNITY_EDITOR
        [SerializeField] private List<RigEffectorData> m_Effectors = new List<RigEffectorData>();
        public IEnumerable<RigEffectorData> effectors { get => m_Effectors; }
#endif

        public delegate void OnAddRigBuilderCallback(AnimGraphRigBuilder rigBuilder);
        public delegate void OnRemoveRigBuilderCallback(AnimGraphRigBuilder rigBuilder);

        public static OnAddRigBuilderCallback onAddRigBuilder;
        public static OnRemoveRigBuilderCallback onRemoveRigBuilder;

        //void OnEnable()
        //{
        //    onAddRigBuilder?.Invoke(this);
        //}

        //void OnDisable()
        //{
        //    // Clear runtime data.
        //    if (Application.isPlaying)
        //        Clear();

        //    onRemoveRigBuilder?.Invoke(this);
        //}

        void OnDestroy()
        {
            Clear();
        }

        public void SyncSetting()
        {
            syncSceneToStreamLayer.SyncUpdate(layers);
        }

        void Update()
        {
            if (!graph.IsValid())
                return;

            syncSceneToStreamLayer.Update(layers);

            for (int i = 0, count = layers.Count; i < count; ++i)
            {
                if (layers[i].IsValid() && layers[i].active)
                    layers[i].rig.UpdateConstraints();
            }
        }

        public bool Build(PlayableGraph graph)
        {
            Clear();
            var animator = GetComponent<Animator>();
            if (animator == null)
                return false;

            if(m_RigLayers == null)
            {
                m_RigLayers = new List<RigLayer>(10);
            }

            m_RigLayers.Clear();

            Rig[] rigs = animator.gameObject.GetComponentsInChildren<Rig>();
            for (int i = 0; i < rigs.Length; i++)
            {
                var layer = new RigLayer(rigs[i], true);
                m_RigLayers.Add(layer);
            }
            if (layers.Count == 0)
                return false;

            //string graphName = gameObject.transform.name + "_Rigs";

            this.graph = graph;

            //var output = AnimationPlayableOutput.Create(graph, "RigIntegrationOutput", animator);
            
            // Create sync scene to stream layer
            syncLayerOutput = AnimationPlayableOutput.Create(graph, "syncSceneToStreamOutput", animator);
            syncLayerOutput.SetAnimationStreamSource(AnimationStreamSource.PreviousInputs);

#if UNITY_2019_3_OR_NEWER
            syncLayerOutput.SetSortingOrder(k_AnimationOutputPriority);
#endif

            // Create all rig layers
            m_RigLayerData = new List<LayerData>(layers.Count);
            foreach (var layer in layers)
            {
                if (!layer.Initialize(animator))
                    continue;

                LayerData data = new LayerData();
                data.output = AnimationPlayableOutput.Create(graph, "rigOutput", animator);
                data.output.SetAnimationStreamSource(AnimationStreamSource.PreviousInputs);

#if UNITY_2019_3_OR_NEWER
                data.output.SetSortingOrder(k_AnimationOutputPriority);
#endif

                data.playables = BuildRigPlayables(graph, layer.rig);

                // Connect last rig playable to output
                if (data.playables != null && data.playables.Length > 0)
                    data.output.SetSourcePlayable(data.playables[data.playables.Length - 1]);

                layer.data = m_RigLayerData.Count;
                m_RigLayerData.Add(data);
            }

            // Create sync to stream job with all rig references
            if (syncSceneToStreamLayer.Initialize(animator, layers) && syncSceneToStreamLayer.IsValid())
                syncLayerOutput.SetSourcePlayable(RigUtils.syncSceneToStreamBinder.CreatePlayable(graph, syncSceneToStreamLayer.job));

            // Create Sync Fix Helper Layer
            //initSyncer(graph, output, animator);

            return true;
        }

        void initSyncer(PlayableGraph a_playableGraph, AnimationPlayableOutput output, Animator a_animator)
        {
            riggingSyncer = a_animator.gameObject.GetComponent<IUnityRiggingIntegration>();
            riggingSyncer?.Initialize(a_playableGraph, output, a_animator);
        }
        
        public Transform[] GetSyncTransform()
        {
            List<Transform> result = new List<Transform>();
            for (int i = 0, count = layers.Count; i < count; ++i)
            {
                var layer = layers[i];
                if (layer.active == false)
                    continue;
                var constraints = layer.rig.constraints;
                for (int j = 0; j < constraints.Length; j++)
                {
                    if (constraints[j] is MultiAimConstraint)
                    {
                        MultiAimConstraint single = constraints[j] as MultiAimConstraint;
                        for (int x = 0; x < single.data.sourceObjects.Count; x++)
                        {
                            result.Add(single.data.sourceObjects[x].transform);
                        }
                    }
                    
                        
                    if (constraints[j] is TwoBoneIKConstraint)
                    {
                        TwoBoneIKConstraint twobone = constraints[j] as TwoBoneIKConstraint;
                        result.Add(twobone.data.target);
                        if (twobone.data.hint != null)
                            result.Add(twobone.data.hint);
                    }                        
                }
            }

            return result.ToArray();
        }
        
        
        public void Clear()
        {
            foreach (var layer in layers)
                layer.Reset();

            if (m_RigLayerData != null)
            {
                foreach (var rigLayerData in m_RigLayerData)
                {
                    if(rigLayerData.output.IsOutputValid())
                        graph.DestroyOutput(rigLayerData.output);
                }
                m_RigLayerData.Clear();
            }
                

            if (m_PreviewRigLayerData != null)
                m_PreviewRigLayerData.Clear();

            syncSceneToStreamLayer.Destroy();
            if(syncLayerOutput.IsOutputValid())
                graph.DestroyOutput(syncLayerOutput);
        }

        AnimationScriptPlayable[] BuildRigPlayables(PlayableGraph graph, Rig rig)
        {
            if (rig == null || rig.jobs == null || rig.jobs.Length == 0)
                return null;

            var count = rig.jobs.Length;
            var playables = new AnimationScriptPlayable[count];
            for (int i = 0; i < count; ++i)
            {
                var binder = rig.constraints[i].binder;
                playables[i] = binder.CreatePlayable(graph, rig.jobs[i]);
            }

            // Connect rig playables serially
            for (int i = 1; i < count; ++i)
                playables[i].AddInput(playables[i - 1], 0, 1);

            return playables;
        }

        //
        // IAnimationWindowPreview methods implementation
        //

        public void StartPreview()
        {
            if (!enabled)
                return;

            var animator = GetComponent<Animator>();
            if (animator != null)
            {
                foreach (var layer in layers)
                {
                    layer.Initialize(animator);
                }
            }
        }

        public void StopPreview()
        {
            if (!enabled)
                return;

            if (Application.isPlaying)
                return;

            Clear();
        }

        public void UpdatePreviewGraph(PlayableGraph graph)
        {
            if (!enabled)
                return;

            if (!graph.IsValid())
                return;

            syncSceneToStreamLayer.Update(layers);

            foreach (var layer in layers)
            {
                if (layer.IsValid() && layer.active)
                    layer.rig.UpdateConstraints();
            }
        }

        public Playable BuildPreviewGraph(PlayableGraph graph, Playable inputPlayable)
        {
            if (!enabled)
                return inputPlayable;

            var animator = GetComponent<Animator>();
            if (animator == null || layers.Count == 0)
                return inputPlayable;

            List<AnimationScriptPlayable> playables = new List<AnimationScriptPlayable>();

            m_PreviewRigLayerData = new List<LayerData>(layers.Count);
            foreach (var layer in layers)
            {
                if (!layer.Initialize(animator))
                    continue;

                LayerData data = new LayerData();
                data.playables = BuildRigPlayables(graph, layer.rig);
                if (data.playables == null)
                    continue;

                layer.data = m_PreviewRigLayerData.Count;
                m_PreviewRigLayerData.Add(data);
            }

            // Create sync to stream job with all rig references
            if (syncSceneToStreamLayer.Initialize(animator, layers) && syncSceneToStreamLayer.IsValid())
            {
                var syncSceneToStreamPlayable = RigUtils.syncSceneToStreamBinder.CreatePlayable(graph, syncSceneToStreamLayer.job);
                syncSceneToStreamPlayable.AddInput(inputPlayable, 0, 1);
                inputPlayable = syncSceneToStreamPlayable;
            }

            foreach (var layerData in m_PreviewRigLayerData)
            {
                if (layerData.playables == null || layerData.playables.Length == 0)
                    continue;

                layerData.playables[0].AddInput(inputPlayable, 0, 1);
                inputPlayable = layerData.playables[layerData.playables.Length - 1];
            }

            return inputPlayable;
        }

#if UNITY_EDITOR
        public void AddEffector(Transform transform)
        {
            var effector = new RigEffectorData();
            effector.Initialize(transform, RigEffectorData.defaultStyle);

            m_Effectors.Add(effector);
        }

        public void RemoveEffector(Transform transform)
        {
            m_Effectors.RemoveAll((RigEffectorData data) => data.transform == transform);
        }

        public bool ContainsEffector(Transform transform)
        {
            return m_Effectors.Exists((RigEffectorData data) => data.transform == transform);
        }
#endif

        public List<RigLayer> layers
        {
            get
            {
                if (m_RigLayers == null)
                    m_RigLayers = new List<RigLayer>();

                return m_RigLayers;
            }

            set => m_RigLayers = value;
        }

        private SyncSceneToStreamLayer syncSceneToStreamLayer
        {
            get
            {
                if (m_SyncSceneToStreamLayer == null)
                    m_SyncSceneToStreamLayer = new SyncSceneToStreamLayer();

                return m_SyncSceneToStreamLayer;
            }

            set => m_SyncSceneToStreamLayer = value;
        }

        public PlayableGraph graph { get; private set; }

        private AnimationPlayableOutput syncLayerOutput;
        
        public IUnityRiggingIntegration riggingSyncer   { get; private set; }     
    }
    
    public interface IUnityRiggingIntegration
    {
        void Initialize(PlayableGraph playableGraph, AnimationPlayableOutput output, Animator animator);
        void CacheTransforms();
        void FixRigTransforms();
    }
}
