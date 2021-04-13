using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ASeKi.action
{
    enum CurveAxis
    {
        X,Y,Z
    }
    
    enum CurveType
    {
        Displacement,
        RotationEuler,
    }
    
    struct CurveDisplayData
    {
        public CurveAxis Axis;
        public CurveType CurveType;
        public MotionData MotionData;
        public int Index;

        public CurveDisplayData(CurveAxis axis, CurveType type, MotionData data, int index)
        {
            Axis = axis;
            CurveType = type;
            MotionData = data;
            Index = index;
        }
    }
    
    public class LeftWindow : IElementWindow
    {
        #region Elements
        
        private VisualElement meRoot;
        private MotionEditor parent;
        
        // Filter
        private VisualElement me_Filter;
        private Label totalMotion;
        private ActionType filterActType = ActionType.ALL;
        private EnumField filterActionTypeField;
        private WeaponType filterWeaponType = WeaponType.WT_ALL;
        private EnumField filterWeaponTypeFiled;
        private ActionPopupField statePopupField = new ActionPopupField();
        private TextField searchParam;
        private bool showSearchFailed = false;
        private int searchID = 0;
        private Button selectFirstBtn = null;
        private Button selectPreviousBtn = null;
        private Button selectNextBtn = null;
        private Button selectEndBtn = null;
        private Button selectBackBtn = null;
        private int backActionId = -1;
        
        // Info
        private VisualElement me_Info;
        private Label actionIDLabel = null;
        private TextField actionNameTextField = null;
        private IntegerField actionLayerIntField = null;
        private Label totalFrameLabel = null;
        private Label weaponTypeLabel = null;
        private Label actionGroupLabel = null;
        private Label actionStateValueLabel = null;
        private EnumField actionTypeField = null;
        
        ActionPopupField actClipsPopupField = new ActionPopupField();
        
        #endregion
        
        private List<int> selectIndexList = new List<int>();
        private List<string> selectNameList = new List<string>();
        private Dictionary<string, CurveDisplayData> curvesDisplay = new Dictionary<string, CurveDisplayData>(6);
        
        public void OnEnable()
        {
            
            #region 筛选操作子VE
            
            me_Filter = meRoot.Q<VisualElement>("ME-Filter");
            
            // 信息部分
            totalMotion = me_Filter.Q<Label>("Label-TotalMotion");
            
            // 筛选部分
            filterActionTypeField = me_Filter.Q<EnumField>("EnumField-ActionType");
            filterWeaponTypeFiled = me_Filter.Q<EnumField>("EnumField-WeaponType");
            TemplateContainer templateContainerActionPopUp = me_Filter.Q<TemplateContainer>("ActionPopupField");
            initStatePopupField(templateContainerActionPopUp);

            // 交互部分（查找、删除）
            VisualElement searchVE = me_Filter.Q<VisualElement>("VisualElement-Search");
            searchParam = searchVE.Q<TextField>("TextField-SearchParam");
            Button searchBtn = searchVE.Q<Button>("Button-Search");
            initSearchButton(searchBtn);
            selectFirstBtn = me_Filter.Q<Button>("Button-select-first");
            selectPreviousBtn = me_Filter.Q<Button>("Button-select-previous");
            selectNextBtn = me_Filter.Q<Button>("Button-select-next");
            selectEndBtn = me_Filter.Q<Button>("Button-select-end");
            selectBackBtn = me_Filter.Q<Button>("Button-select-back");
            Button delBtn = me_Filter.Q<Button>("Button-Delete");
            initDelBtn(delBtn);
            
            #endregion

            #region 信息操作子VE

            me_Info = meRoot.Q<VisualElement>("ME-Info");
            actionIDLabel = me_Info.Q<Label>("Label-Action-ID");
            actionNameTextField = me_Info.Q<TextField>("TextField-Action-Name");            
            actionLayerIntField = me_Info.Q<IntegerField>("IntegerField-Action-Layer");
            totalFrameLabel = me_Info.Q<Label>("Label-Total-Frame");
            weaponTypeLabel = me_Info.Q<Label>("Label-Weapon-Type");
            actionStateValueLabel = me_Info.Q<Label>("Label-Action-State-Value"); 
            actionTypeField = me_Info.Q<EnumField>("enum-action-type");
            actionGroupLabel = me_Info.Q<Label>("Label-Action-Group");

            TemplateContainer templateContainerActionClipPopUp = me_Filter.Q<TemplateContainer>("ActionClipPopupField");
            initClipsPopupField(templateContainerActionClipPopUp);

            #endregion
        }

        public void OnDisable()
        {
            
        }

        public void Update()
        {
            
        }

        public void OnGUI()
        {
            
        }

        public LeftWindow(VisualElement controlWindow, MotionEditor motionEditor)
        {
            meRoot = controlWindow;
            parent = motionEditor;
        }

        
        
        #region Filter交互部分（枚举查找state、ID/Name查找state、删除state）

        private void initStatePopupField(TemplateContainer templateContainerActionPopUp)
        {
            Label popUpLabel = templateContainerActionPopUp.Q<Label>("Label-Title");
            VisualElement popUpVE = templateContainerActionPopUp.Q<VisualElement>("ME-Action-Popup");
            statePopupField.Set(popUpLabel, popUpVE);
            statePopupField.Init("Select : ", selectNameList, "", onActionPopupChange);
        }
        
        private void onActionPopupChange(string str)
        {
            Debug.Log("onActionPopupChange " + str);
            int index = selectNameList.IndexOf(str);
            if (parent.CurrentMotionData.ActionId != selectIndexList[index])
            {   
                Debug.Log("change to " + str);
                bool result = parent.SearchMotionData(selectIndexList[index]);
            }            
        }

        private void initSearchButton(Button btn)
        {
            btn.clicked += search;
        }
        
        private void search()
        {
            int select = 0;
            showSearchFailed = false;
            bool isNumeric = int.TryParse(searchParam.value, out searchID);
            if (isNumeric)
            {
                if (searchID >= 0)
                {
                    select = searchID;
                    
                }
                else
                {
                    showSearchFailed = true;
                }
            }
            else
            {
                int find = selectNameList.IndexOf(searchParam.value);
                if (find >= 0)
                {
                    select = selectIndexList[find];
                }
                else
                {
                    showSearchFailed = true;
                }
            }

            if (!showSearchFailed)
            {
                bool result = parent.SearchMotionData(select);
                if (result)
                {
                    searchParam.value = parent.CurrentMotionData.ActionId.ToString();
                    int find = selectIndexList.IndexOf(parent.CurrentMotionData.ActionId);
                    if (find >= 0)
                    {
                        string str = selectNameList[find];
                        statePopupField.ChangeLabel(str);
                    }
                    else
                    {
                        resetFilterAndList(parent.CurrentMotionData.ActionType, parent.CurrentMotionData.WeaponType);
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Search Motion Data", "Format error!!", "ok");
            }
        }
        
        private void initDelBtn(Button delBtn)
        {
            delBtn.clicked += deleteMotion;
        }
        
        private void deleteMotion()
        {
            if (parent.RemoveCurMotionData())
            {
                // 刪除成功重刷(找)一次
                filterActType = ActionType.ALL;
                filterWeaponType = WeaponType.WT_ALL;
                filterActionTypeField.value = filterActType;
                filterWeaponTypeFiled.value = filterWeaponType;

                parent.SearchMotionData(filterActType, filterWeaponType);
                ProcessMapping();
                statePopupField.Reset(selectNameList, "");
            }
        }
        
        public void ProcessMapping()
        {
            selectIndexList.Clear();
            selectNameList.Clear();

            int num = parent.SearchMotionList.Count;
            for (int i = 0; i < num; i++)
            {
                selectIndexList.Add(parent.SearchMotionList[i].ActionId);
                selectNameList.Add(parent.SearchMotionList[i].ActionName);
            }
        }
        
        #endregion

        
        
        #region Info交互部分（查找clip、重命名state）

        void initClipsPopupField(TemplateContainer temp)
        {
            Label labelTitle = temp.Q<Label>("Label-Title");
            VisualElement actPopupVE = temp.Q<VisualElement>("ME-Action-Popup");
            actClipsPopupField.Set(labelTitle, actPopupVE);
            actClipsPopupField.Init("Clips :", parent.CurrentMotionData.ActionClipNameList, "", onClipsPopupChange);
        }

        void onClipsPopupChange(string str)
        {
            Debug.Log("onClipsPopupChange " + str);
            int index = parent.CurrentMotionData.ActionClipNameList.IndexOf(str);
            if (index >= 0)
            {
                setClips(index);
            }
            else
            {
                Debug.LogError("ActionClip no found!!");
            }            
        }
        
        #endregion
        
        
        
        #region 主动作状态改变导致刷新界面信息
        
        public void ProcessBeforeChange()
        {
            backActionId = parent.CurrentMotionData.ActionId;    // 更新之前ID
        }
        
        public void Refresh()
        {
            setCtrl();
            setInfo();
            setClips();
        }
        
        void setCtrl()
        {
            totalMotion.text = $"Total Motion : {parent.TotalMotions}";
            searchParam.value = parent.CurrentMotionData.ActionId.ToString();

            int count = selectIndexList.Count;
            int index = selectIndexList.IndexOf(parent.CurrentMotionData.ActionId);
            if (index >= 0)
            {
                if (index == 0)
                {
                    selectPreviousBtn.text = $"{selectIndexList[count - 1]}"; //最後一個
                }
                else
                {
                    selectPreviousBtn.text = $"<< {selectIndexList[index - 1]}"; //上一個
                }

                if (index == count - 1)
                {
                    selectNextBtn.text = $"{selectIndexList[0]}"; //第一個
                }
                else
                {
                    selectNextBtn.text = $"{selectIndexList[index + 1]} >>"; //下一個
                }
            }
        }
        
        void setInfo()
        {
            statePopupField.ChangeLabel(parent.CurrentMotionData.ActionName);
            actionIDLabel.text = $" Action ID : {parent.CurrentMotionData.ActionId}";
            actionNameTextField.SetValueWithoutNotify(parent.CurrentMotionData.ActionName);
            actionLayerIntField.SetValueWithoutNotify(parent.CurrentMotionData.Layer);
            totalFrameLabel.text = $" Total Frame : {parent.CurrentMotionData.TotalFrame}";
            actionStateValueLabel.text = parent.CurrentMotionData.ActionState.ToString();
            weaponTypeLabel.text = $" Weapon Type : {parent.CurrentMotionData.WeaponType.ToString()}";
            actionTypeField.SetValueWithoutNotify(parent.CurrentMotionData.ActionType);
            actionGroupLabel.text = MotionEditorUtility.GetMaskValue(parent.CurrentMotionData.ActionGroup, typeof(ActionGroupTag));
            if (backActionId != -1)
            {
                selectBackBtn.text = $"Back {backActionId.ToString()}";
            }
        }
        
        void setClips(int index = 0)
        {
            if (index == 0)
            {
                actClipsPopupField.Reset(parent.CurrentMotionData.ActionClipNameList, "");
            }
            
            if (parent.CurrentMotionData.ActionClipList.Count > index)
            {
                AnimationClip clip = parent.CurrentMotionData.ActionClipList[index];
                if (clip == null)
                {
                    Debug.LogError($"Current Clip {parent.CurrentMotionData.ActionClipNameList[index]} is null!!!!!");
                    EditorUtility.DisplayDialog("我的天！", $"当前Clip {parent.CurrentMotionData.ActionClipNameList[index]}竟然是空得！！！", "问题不大");
                    return;
                }
                
                int currentFrame = (int)(clip.length * clip.frameRate);
                if (parent.CurrentMotionData.TotalFrame != currentFrame)
                {
                    Debug.Log($"Current Motion {parent.CurrentMotionData.ActionName} total Frame update from{parent.CurrentMotionData.TotalFrame} to {currentFrame}");
                    parent.CurrentMotionData.TotalFrame = currentFrame;
                    parent.CurrentMotionData.TotalTime = clip.length;
                    totalFrameLabel.text = $" Total Frame : {parent.CurrentMotionData.TotalFrame}";
                }
                
                string clipName = parent.CurrentMotionData.ActionClipNameList[index];
                if (parent.CurrentMotionData.DisplacementCurveXList.Count < index
                    || parent.CurrentMotionData.ActionClipList.Count != parent.CurrentMotionData.DisplacementCurveXList.Count)
                {
                    parent.CurrentMotionData.InitAnimationCurve();
                }

                curvesDisplay.Clear();
                setCurveField($"{clipName}.Pos.X", index, CurveAxis.X, CurveType.Displacement);
                setCurveField($"{clipName}.Pos.Y", index, CurveAxis.Y, CurveType.Displacement);
                setCurveField($"{clipName}.Pos.Z", index, CurveAxis.Z, CurveType.Displacement);
                setCurveField($"{clipName}.Rot.X", index, CurveAxis.X, CurveType.RotationEuler);
                setCurveField($"{clipName}.Rot.Y", index, CurveAxis.Y, CurveType.RotationEuler);
                setCurveField($"{clipName}.Rot.Z", index, CurveAxis.Z, CurveType.RotationEuler);
            }
        }
        
        void setCurveField(string title, int index, CurveAxis axis, CurveType type)
        {
            curvesDisplay.AddOrReplace(title, new CurveDisplayData(axis, type, parent.CurrentMotionData, index));
        }
        
        // 刷新筛选
        void resetFilter()
        {
            resetFilterAndList(ActionType.ALL, WeaponType.WT_ALL);
        }
        
        void resetFilterAndList(ActionType newType, WeaponType newWeaponType)
        {
            filterActType = newType;
            filterWeaponType = newWeaponType;
            filterActionTypeField.SetValueWithoutNotify(newType);
            filterWeaponTypeFiled.SetValueWithoutNotify(newWeaponType);
            parent.SearchMotionData(newType, newWeaponType);
            ProcessMapping();
            statePopupField.Reset(selectNameList, "");
            Refresh();
        }
        
        #endregion
    }
}