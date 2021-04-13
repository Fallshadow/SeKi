using System;
using System.Collections.Generic;
using System.Linq;
using Framework.AnimGraphs;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ASeKi.action
{
    public class MotionEditor : EditorWindow
    {
        public static EditorWindow motionEditorWindows = null;
        public List<MotionData> SearchMotionList = new List<MotionData>();
        
        private const string RES_ROOT_PATH = "Assets/Editor/Action/MotionEditor/Resources/";
        private MotionDataListSO motionDataSetting = null;

        private MiddleWindow middleWindow;
        private LeftWindow leftWindow;
        private List<IElementWindow> windows = new List<IElementWindow>();    // 将子窗体添加到这里，方便调用子窗体们的周期方法来理清楚代码
        
        private bool btnIsDirtyTex = false;
        
        #region Motion Data

        public MotionData CurrentMotionData
        {
            get => curMotionData;
            set {
                // 可以加事件刷新頁面或其他
                beforeChangeAction?.Invoke();
                curMotionData = value;
                refreshAction?.Invoke();
                // EditorUtility.SetDirty(motionDataList);
                // AssetDatabase.SaveAssets();
            }
        }
        public int TotalMotions
        {
            get 
            {
                if (motionDataSetting != null)
                {
                    return motionDataSetting.GetTotalMotion();
                }
                return 0;
            }
        }
        
        private MotionData curMotionData = null;
        private System.Action beforeChangeAction = null;
        private System.Action refreshAction = null;

        #endregion

        [MenuItem("SeKi/Action/Open Motion Editor #&m", false, 1500000)]
        public static void OpenWindow()
        {
            motionEditorWindows = GetWindow<MotionEditor>();
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>($"{RES_ROOT_PATH}/Icon/motion-icon.png");
            motionEditorWindows.titleContent = new GUIContent("Motion Editor", icon);
        }

        public void OnEnable()
        {
            // TODO:AssetDatabase.LoadAssetAtPath<Texture> 拿到编辑器需要的图片
            // TODO:拿到编辑器需要的数据并进行初步操作（查空、按需排序等）
            
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Action/MotionEditor/Resources/MotionEditor.uxml");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Action/MotionEditor/Resources/MotionEditor.uss");
            VisualElement myTree = visualTree.CloneTree();
            root.Add(myTree);
            myTree.styleSheets.Add(styleSheet);
            
            VisualElement leftWindow = myTree.Q<VisualElement>("ME-Left");
            initControlWindow(leftWindow);
            
            VisualElement middleWindow = myTree.Q<VisualElement>("ME-Middle");
            initMiddleWindow(middleWindow);
            
            ToolbarButton addDataButton = myTree.Q<ToolbarButton>("ToolbarButton-Add");
            initAddDataButton(addDataButton);
            
            ToolbarButton saveDataButton = myTree.Q<ToolbarButton>("ToolbarButton-Save");
            initAddDataButton(saveDataButton);
        }

        private void initMiddleWindow(VisualElement visualElement)
        {
            middleWindow = new MiddleWindow(visualElement,this);
            middleWindow.OnEnable();
        }

        void initControlWindow(VisualElement tree)
        {
            leftWindow = new LeftWindow(tree, this);
            leftWindow.OnEnable();
            windows.Add(leftWindow);
            beforeChangeAction += leftWindow.ProcessBeforeChange;
            refreshAction += leftWindow.Refresh;
            // initResizer(tree); 分割线
        }
        
        

        #region 寻找动作数据API

        // 根据动作类型和武器类型 挑选一个动作数据出来
        public bool SearchMotionData(ActionType actType, WeaponType weaponType)
        {
            if (motionDataSetting.GetMotionData(actType, weaponType, false, ref SearchMotionList))
            {
                CurrentMotionData = SearchMotionList[0];
                return true;
            }
            else
            {
                EditorUtility.DisplayDialog("Search Motion Data", "Not found!!", "ok");
            }
            return false;
        }
        
        // 通过Key去寻找动作数据
        public bool SearchMotionData(int key)
        {
            MotionData mData = motionDataSetting.GetMotionData(key);
            if (mData != null)
            {
                CurrentMotionData = mData;
                return true;
            }
            else
            {
                EditorUtility.DisplayDialog("Search Motion Data", "Not found!!", "ok");
            }

            return false;
        }

        #endregion
        
        public bool RefreshMotionData(ActionType actType, WeaponType weaponType)
        {
            return motionDataSetting.GetMotionData(actType, weaponType, false, ref SearchMotionList);
        }
        
        #region 左侧窗口 数据动作 删除

        private bool removeMotionGraphStateData(string motionName)
        {
            AnimGraphNodeScriptableObject graphNode = GetGraphNodeByMotionName(motionName);
            if (graphNode == null)
            {
                Debug.Log($"Current Motion {motionName} Don't has Playable Data!'");
                return false;
            }
            AnimGraphStateScriptableObject graphState = graphNode.stateScriptable;
            string curGraphName = graphState.stateName;
            AnimGraphMotionScriptableObject graphMotion = graphState.motion;
            
            List<string> removeTargets = new List<string>();
            removeTargets.Add(AssetDatabase.GetAssetPath(graphNode));
            removeTargets.Add(AssetDatabase.GetAssetPath(graphState));
            removeTargets.Add(AssetDatabase.GetAssetPath(graphMotion));
            if (graphMotion is AnimGraphBlendTreeScriptableObject graphBlendTree)
            {
                var tempTree = graphBlendTree.tree;
                removeTargets.Add(AssetDatabase.GetAssetPath(tempTree));
            }

            EditorUtility.DisplayProgressBar("Delete motion Data", "收集依赖信息中...", 0.2f);
            MotionDependencyHelper helper = new MotionDependencyHelper(MotionDataCollection.Path);
            
            int counter = 0;
            foreach (var removeTarget in removeTargets)
            {
                EditorUtility.DisplayProgressBar("Delete motion Data", "删除中...", counter * 1f / removeTargets.Count);
                counter++;
                string[] targetDependencies = helper.GetReferences(removeTarget);
                string otherDepend = null;
                if (targetDependencies != null)
                {
                    foreach (var dependency in targetDependencies)
                    {
                        if (dependency.EndsWith(".anim") || dependency.EndsWith(".cs")) continue;
                        if (removeTargets.Exists(x => x == dependency)) continue;
                        otherDepend = dependency;
                    }
                }
                if (!string.IsNullOrEmpty(otherDepend))
                {
                    Debug.LogError($"{removeTarget} Current Target Has Depend{otherDepend} Can't remove!'");
                    continue;
                }
                Debug.Log($"Remove Target : {removeTarget}");
                AssetDatabase.DeleteAsset(removeTarget);
            }

            var motionDatas = GetMotionDatas();
            foreach (var motion in motionDatas)
            {
                var tempGraphState = GetGraphStateByMotionName(motion.ActionName);
                if (tempGraphState == null || tempGraphState.transitions.IsNullOrEmpty())
                {
                    Debug.Log($"Current {motion.ActionName} Transition is empty!!");
                    continue;
                }

                for (var index = tempGraphState.transitions.Count - 1; index >= 0; index--)
                {
                    var trans = tempGraphState.transitions[index];
                    if (trans.destinationState == curGraphName)
                    {
                        tempGraphState.transitions.Remove(trans);
                        EditorUtility.SetDirty(tempGraphState);
                    }
                }
                
                var transition = tempGraphState.anyTransition;
                if (transition.destinationState == curGraphName)
                {
                    tempGraphState.anyTransition = new AnimGraphTransitionScriptableObject();
                    tempGraphState.anyTransition.duration = 0;
                    tempGraphState.anyTransition.destinationState = "Any";
                    tempGraphState.anyTransition.exitTime = 0;
                    tempGraphState.anyTransition.hasExitTime = false;
                    tempGraphState.anyTransition.offset = 0;;
                    EditorUtility.SetDirty(tempGraphState);
                }
            }
            EditorUtility.ClearProgressBar();
            return true;
        }
        
        // 获取第一个名字叫motionName的node
        public AnimGraphNodeScriptableObject GetGraphNodeByMotionName(string motionName)
        {
            var statesPath = AssetDatabase.FindAssets($"t:{typeof(AnimGraphNodeScriptableObject)} {motionName}", new[] { MotionDataCollection.Path });
            if (statesPath == null || statesPath.Length == 0)
            {
                Debug.LogError($"Current Folder {MotionDataCollection.Path} is not contains {motionName}");
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<AnimGraphNodeScriptableObject>(AssetDatabase.GUIDToAssetPath(statesPath.First()));
        }
        
        // 获取第一个名字叫motionName的state
        public AnimGraphStateScriptableObject GetGraphStateByMotionName(string motionName)
        {
            var curFolder = middleWindow.GetCurFolder();
            var statesPath = AssetDatabase.FindAssets($"t:{typeof(AnimGraphStateScriptableObject)} {motionName}", new[] { AssetDatabase.GetAssetPath(curFolder) });
            if (statesPath == null || statesPath.Length == 0)
            {
                Debug.LogError($"Current Folder {AssetDatabase.GetAssetPath(curFolder)} is not contains {motionName}");
                return null;
            }
            
            return AssetDatabase.LoadAssetAtPath<AnimGraphStateScriptableObject>(AssetDatabase.GUIDToAssetPath(statesPath.First()));            
        }
        
        public bool RemoveCurMotionData()
        {
            if (EditorUtility.DisplayDialog("Delete motion Data", "Are You Sure?", "Yes!", "No!"))
            {
                removeMotionGraphStateData(CurrentMotionData.ActionName);
                
                bool result = motionDataSetting.DeletMotionData(CurrentMotionData.ActionId);
                EditorUtility.DisplayDialog("Delete motion Data", result ? "Success!!" : "Failed!!", "ok");

                if (result)
                {
                    motionDataSetting.Sort();
                    AssetDatabase.SaveAssets();
                    return true;
                }
            }
            return false;
        }
        
        #endregion
        
        #region 顶部按钮 数据动作 增加

        private void initAddDataButton(ToolbarButton toolbarButton)
        {
            toolbarButton.style.backgroundImage = new StyleBackground(EditorGUIUtility.IconContent("AnimatorOverrideController Icon").image as Texture2D);
            toolbarButton.clicked += () => onAddNewMotion();
        }
        
        void onAddNewMotion()
        {
            CreateMotionStateWindow createWindow = GetWindow<CreateMotionStateWindow>();
            createWindow.CenterOnMainWin();
            var content = EditorGUIUtility.IconContent("Project");
            content.text = "Add New State";
            createWindow.titleContent = content;
            createWindow.Finish = onCreateNewState;
        }
        
        // 给定状态名称和状态文件路径，创建状态文件
        void onCreateNewState(string stateName, string resPath)
        {
            string animSOPath = $"{MotionDataCollection.Path}/Controller/Animations/";
            string blendTreeSOPath = $"{MotionDataCollection.Path}/Controller/BlendTree/";
            string nodePath = $"{MotionDataCollection.Path}/Controller/";
            string statePath = $"{MotionDataCollection.Path}/Controller/State/";

            var stateSO = CreateInstance<AnimGraphStateScriptableObject>();
            
            stateSO.layer = 0;
            stateSO.stateName = stateName;
            stateSO.iKOnFeet = false;
            stateSO.writeDefaultValues = true;
            stateSO.transitions = new List<AnimGraphTransitionScriptableObject>();
            // AnyTransition
            stateSO.anyTransition = new AnimGraphTransitionScriptableObject();
            stateSO.anyTransition.duration = 0;
            stateSO.anyTransition.destinationState = "Any";
            stateSO.anyTransition.exitTime = 0;
            stateSO.anyTransition.hasExitTime = false;
            stateSO.anyTransition.offset = 0;

            stateSO.fadeInLayerTransition = new AnimGraphLayerTransitionScriptableObject();
            stateSO.fadeInLayerTransition.duration = 0;
            stateSO.fadeInLayerTransition.layerTransitionType = LayerTransitionType.FadeIn;

            stateSO.fadeOutLayerTransition = new AnimGraphLayerTransitionScriptableObject();
            stateSO.fadeOutLayerTransition.duration = 0;
            stateSO.fadeOutLayerTransition.layerTransitionType = LayerTransitionType.FadeOut;

            if (resPath.EndsWith(".anim"))
            {
                var clipRes = AssetDatabase.LoadAssetAtPath<AnimationClip>(resPath);

                var clipSO = AssetDatabase.LoadAssetAtPath<AnimGraphAnimationClipScriptableObject>($"{animSOPath}{clipRes.name}.asset");

                if (clipSO == null)
                {
                    clipSO = CreateInstance<AnimGraphAnimationClipScriptableObject>();
                    AssetDatabase.CreateAsset(clipSO, $"{animSOPath}{clipRes.name}.asset");
                }
                
                clipSO.animationClip = clipRes;
                clipSO.isLooping = clipRes.isLooping;
                clipSO.duration = clipRes.length;
                EditorUtility.SetDirty(clipSO);
                
                stateSO.motion = clipSO;
            }
            else if(resPath.EndsWith(".blendtree"))
            {
                var blendTree = AssetDatabase.LoadAssetAtPath<BlendTree>(resPath);

                var blendtreeSO = AssetDatabase.LoadAssetAtPath<AnimGraphBlendTreeScriptableObject>($"{blendTreeSOPath}{blendTree.name}.asset");

                if (blendtreeSO == null)
                {
                    blendtreeSO = CreateInstance<AnimGraphBlendTreeScriptableObject>();
                    AssetDatabase.CreateAsset(blendtreeSO, $"{blendTreeSOPath}{blendTree.name}.asset");
                }

                blendtreeSO.tree = blendTree;
                blendtreeSO.isLooping = blendTree.isLooping;
                blendtreeSO.blendParameterX = blendTree.blendParameter;
                blendtreeSO.blendParameterY = blendTree.blendParameterY;
                blendtreeSO.blendType = (Framework.AnimGraphs.BlendTreeType) blendTree.blendType;

                EditorUtility.SetDirty(blendtreeSO);

                stateSO.motion = blendtreeSO;
            }
            
            EditorUtility.SetDirty(stateSO);
            AssetDatabase.CreateAsset(stateSO, $"{statePath}{stateName}.asset");

            var NodeSO = CreateInstance<AnimGraphNodeScriptableObject>();
            NodeSO.nodeName = stateName;
            NodeSO.stateScriptable = stateSO;
            EditorUtility.SetDirty(NodeSO);
            AssetDatabase.CreateAsset(NodeSO, $"{nodePath}{stateName}.asset");
                            
            AssetDatabase.SaveAssets();

            onReceiveDragState(stateSO);
        }
        
        void onReceiveDragState(AnimGraphStateScriptableObject state)
        {
            Debug.Log($"ReceiveDrag {state.stateName}");
            var current = Event.current;
            var rect = new Rect(current.mousePosition.x, current.mousePosition.y, 0, 0);
            var options = Enum.GetNames(typeof(WeaponType)).Select(x => new GUIContent(x)).ToArray();
            if (tryGetTypeFromName(state.stateName, out WeaponType type))
            {
                addMotionByState(type, state);
            }
            else
            {
                Debug.LogError("现在已经不允许你这样弄了，你看看你是不是武器枚举错了了，正常应该不会报这条错的");
            }
        }
        
        bool tryGetTypeFromName(string motionName, out WeaponType type)
        {
            type = WeaponType.None;
            string firstCharacter = $"{motionName[0]}";
            if (int.TryParse(firstCharacter, out int typeNum))
            {
                type = (WeaponType)typeNum;
                return true;
            }
            return false;
        }
        
        // 动画设置数据，武器种类已经有了，开始创建动作数据
        // 从动作资源列表中查找所有的动作数据，看看是否有重名，有则刷新，无则创建
        // 设置当前编辑器指定的动作，设置脏数据以备保存
        void addMotionByState(WeaponType type, AnimGraphStateScriptableObject state)
        {
            List<MotionData> motionDatas = motionDataSetting.GetMotionDatas();
            MotionData ret = motionDataSetting.GetMotionData(state.stateName);

            if (ret != null)
            {
                Debug.Log($"已经存在名为{state.stateName}的motion!");
                
                RefreshMotionByState(ret, state);
            }
            else
            {
                ret = addNewMotionDataFromState(type, state, motionDatas);
            }
            // force to refresh!
            CurrentMotionData = ret;
            EditorUtility.SetDirty(motionDataSetting);
        }
        
        public void RefreshMotionByState(MotionData motionData, AnimGraphStateScriptableObject state)
        {
            setMotionDataFromStateData(motionData, state);
            setMotionNextStateByGraphStateData(motionData, state, motionDataSetting.GetMotionDatas());
        }
        
        private MotionData addNewMotionDataFromState(WeaponType type, AnimGraphStateScriptableObject state, List<MotionData> motionDatas)
        {
            MotionData result = null;
            if (motionDataSetting == null) return null;
            if (type == WeaponType.WT_ALL)
            {
                type = WeaponType.None;
            }
            result = motionDataSetting.AddNewMotionData(type);
            setMotionDataFromStateData(result, state);
            setMotionNextStateByGraphStateData(result, state, motionDatas);
            motionDataSetting.Sort();
            return result;
        }

        #region 通过动画设置数据去设置动作数据

        // 通过动画设置数据去设置动作数据
        void setMotionDataFromStateData(MotionData motionData, AnimGraphStateScriptableObject state)
        {
            if (motionData == null)
            {
                return;
            }
            motionData.ActionName = state.stateName;
            motionData.Layer = state.layer;
            motionData.LayerBlendMode = (int)loadStateLayerBlendingMode(state);
            getClipFromState(state, ref motionData.ActionClipList);


            if (!motionData.ActionClipList.IsNullOrEmpty() && state.motion != null)
            {
                float frameRate = 30f;

                var firstClip = motionData.ActionClipList.First();
                frameRate = firstClip.frameRate;
                motionData.TotalTime = state.motion.GetDuration();
                motionData.TotalFrame = Mathf.FloorToInt(motionData.TotalTime * frameRate);
            }
            else
            {
                Debug.LogError($"Current Motion: {motionData.ActionName} id:{motionData.ActionId} has no clip!!! Plz Check");
            }

            motionData.ActionClipNameList.Clear();
            foreach (var clip in motionData.ActionClipList)
            {
                motionData.ActionClipNameList.Add(clip.name);
            }
        }
        
        // 通过动画设置数据去获取动画混合方式
        LayerBlendingMode loadStateLayerBlendingMode(AnimGraphStateScriptableObject state)
        {
            if (state == null)
            {
                return LayerBlendingMode.Override;
            }
            string wholeName = $"State/{state.name}.asset";
            var controllerPath = AssetDatabase.GetAssetPath(state);
            var targetPath = controllerPath.Remove(controllerPath.Length - wholeName.Length, wholeName.Length) + "Layers/Layers.asset";
            var layerData = AssetDatabase.LoadAssetAtPath<AnimGraphLayersScriptableObject>(targetPath);
            if (layerData == null)
            {
                Debug.LogError("Layer Data is null!");
                return LayerBlendingMode.Override;
            }

            switch (layerData.layers[state.layer].blendingMode)
            {
                case AnimatorLayerBlendingMode.Override:
                    return LayerBlendingMode.Override;
                case AnimatorLayerBlendingMode.Additive:
                    return LayerBlendingMode.Additive;
            }
            return LayerBlendingMode.Override;
        }

        // 通过动画设置数据去获取动画clip数据
        private void getClipFromState(AnimGraphStateScriptableObject state, ref List<AnimationClip> clips)
        {
            clips.Clear();
            getClip(state.motion, ref clips);
        }
        
        private void getClip(AnimGraphMotionScriptableObject motion, ref List<AnimationClip> result)
        {
            if (motion is AnimGraphAnimationClipScriptableObject clipObject)
            {
                result.Add(clipObject.animationClip);
            }
            else if (motion is AnimGraphBlendTreeScriptableObject blendTreeObj)
            {
                foreach (var childMotion in blendTreeObj.childMotions)
                    getClip(childMotion.motion, ref result);
            }
        }

        #endregion

        #region 通过动画设置数据去设置动作数据的下一个动作数据列表

        void setMotionNextStateByGraphStateData(MotionData target, AnimGraphStateScriptableObject state, List<MotionData> motionDatas)
        {
            List<MotionData> nextMotions = new List<MotionData>();

            foreach (var transition in state.transitions)
            {
                var q = motionDatas.Find(x => x.ActionName.Equals(transition.destinationState, StringComparison.Ordinal));
                if (q != null)
                {
                    nextMotions.Add(q);
                }
                else
                {
                    Debug.LogError($"Current Motion Not Found {transition.destinationState}!!");
                }
            }

            //TODO: NextMotion
            foreach (var nextMotion in nextMotions)
            {
                var ret = target.NextMotionList.Find(x => x.Id == nextMotion.ActionId);
                if (ret == null)
                {
                    ActionRoleType type = GetActionRoleType(nextMotion.ActionId);
                    NextMotion newNextMotion = new NextMotion { Id = nextMotion.ActionId, MotionRoleType = type };
                    target.NextMotionList.Add(newNextMotion);
                    MotionCondition newMotionCondition = new MotionCondition();
                    newNextMotion.NextMotionConditionList.Add(newMotionCondition);
                }
                else
                {
                    Debug.Log($"Already has the NextMotion: {nextMotion.ActionName}");
                    continue;
                }
            }
        }
        
        public ActionRoleType GetActionRoleType(int id)
        {
            MotionData motionData = motionDataSetting.GetMotionData(id);
            if (motionData == null) return ActionRoleType.ART_PLAYER_NONE;
            return ActionSetting.GetActionRoleType(motionData.WeaponType);
        }
        
        #endregion
        
        #endregion

        #region 顶部按钮 数据动作 保存

        private void initSaveDataButton(ToolbarButton toolbarButton)
        {
            toolbarButton.style.backgroundImage = new StyleBackground(AssetDatabase.LoadAssetAtPath<Texture>($"{RES_ROOT_PATH}/Icon/save-modify.png") as Texture2D);
            btnIsDirtyTex = true;
            toolbarButton.clicked += () => { AssetDatabase.SaveAssets(); };
            toolbarButton.style.backgroundImage = new StyleBackground(EditorGUIUtility.IconContent("AnimatorOverrideController Icon").image as Texture2D);
            toolbarButton.clicked += () => onAddNewMotion();
        }

        #endregion

        #region motionDataSetting的接口

        public List<MotionData> GetMotionDatas()
        {
            return motionDataSetting.GetMotionDatas();
        }

        #endregion
        
    }
}
