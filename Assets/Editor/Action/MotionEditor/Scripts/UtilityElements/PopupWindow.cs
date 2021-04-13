using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ASeKi.action
{
    public class PopupWindow : EditorWindow
    {
        bool initializedPosition = false;
        
        public class PopupItem
        {
            public string name;
            public int index;
            public bool isSelect;
        }
        
        Vector2 scrollPos;
        
        List<PopupItem> items = new List<PopupItem>();

        private List<string> datas;
        
        System.Type enumType;

        GUIStyle textStyle;
        GUIStyle selectedBackgroundStyle;
        GUIStyle normalBackgroundStyle;
        GUIStyle searchToobar;

        bool isInitedStype = false;
        bool isSelected = false;
        
        string searchText = string.Empty;
        
        PopupItem selectItem = null;

        System.Action<string> callback;

        private const float elementHeight = 16;
        
        public void Init(int target, List<string> targetsNames, System.Action<string> onSelectCallBack)
        {
            items.Clear();

            this.callback = onSelectCallBack;
            this.datas = targetsNames;
            
            for (int i = 0; i < datas.Count; i++)
            {
                string curName = datas[i];
                PopupItem item = new PopupItem
                {
                    name = curName,
                    index = i,
                    isSelect = false
                };

                //default select
                if (i == target)
                {
                    item.isSelect = true;
                    selectItem = item;
                }

                items.Add(item);
            }

            int realIndex = selectItem == null ? 0 : items.IndexOf(selectItem);

            const float scrollHeight = 20f;
            
            scrollPos.y = scrollHeight * realIndex;

            isSelected = false;
            searchToobar = EditorStyles.toolbarSearchField;
            searchText = string.Empty;
            InitTextStyle();
        }

        void initWindowPos()
        {
            if (!initializedPosition)
            {
                Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                position = new Rect(mousePos.x, mousePos.y, position.width, position.height);
                initializedPosition = true;
            }
        }
        
        void OnGUI()
        {
            //initWindowPos();
            
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            GUI.SetNextControlName("Search");
            
            searchText = EditorGUILayout.TextField("", searchText, searchToobar, GUILayout.MinWidth(95));
            EditorGUI.FocusTextInControl("Search");

            if (GUILayout.Button("A", EditorStyles.toolbarButton, GUILayout.Width(32)))
            {
                items.Sort((e1, e2) => string.Compare(e1.name, e2.name, StringComparison.Ordinal));
                scrollPos.y = elementHeight * (selectItem == null ? 0 : items.IndexOf(selectItem));
            }

            GUILayout.EndHorizontal();
            
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            
            //Debug.Log(scrollPos);
            
            int count = items.Count;
            for (int i = 0; i < count; i++)
            {
                PopupItem single = items[i];

                if (!string.IsNullOrEmpty(searchText)
                    && !single.name.ToLower().Contains(searchText.ToLower()))
                    continue;

                Rect rect = single.isSelect
                    ? EditorGUILayout.BeginHorizontal(selectedBackgroundStyle)
                    : EditorGUILayout.BeginHorizontal(normalBackgroundStyle);

                GUILayout.Label(single.name, textStyle);
                GUILayout.FlexibleSpace();

                if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
                {
                    callback?.Invoke(single.name);
                    Debug.Log(single.name);
                    isSelected = true;
                }
                
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (isSelected)
            {
                isSelected = false;
                Close();
            }

            if (focusedWindow != this)
            {
                Close();
            }
        }
        
        const string s_SelectedBg_Pro = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAQklEQVRIDe3SsQkAAAgDQXWN7L+nOMFXdm8dIhzpJPV581l+3T5AYYkkQgEMuCKJUAADrkgiFMCAK5IIBTDgipBoAWXpAJEoZnl3AAAAAElFTkSuQmCC";
        const string s_HightLightBg_Pro = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAQklEQVRIDe3SsQkAAAgDQXXD7L+MOMFXdm8dIhzpJPV581l+3T5AYYkkQgEMuCKJUAADrkgiFMCAK5IIBTDgipBoARFdATMHrayuAAAAAElFTkSuQmCC";
        const string s_SelectedBg_Light = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAQUlEQVRIDe3SsQkAAAgDQXV/yMriBF/ZvXWIcKST1OfNZ/l1+wCFJZIIBTDgiiRCAQy4IolQAAOuSCIUwIArQqIF36EB7diYDg8AAAAASUVORK5CYII=";
        const string s_HightLightBg_Light = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAIAAAD8GO2jAAAAQklEQVRIDe3SsQkAAAgDQXX/ETOMOMFXdm8dIhzpJPV581l+3T5AYYkkQgEMuCKJUAADrkgiFMCAK5IIBTDgipBoAc9YAtQLJ3kPAAAAAElFTkSuQmCC";

        void InitTextStyle()
        {
            if (isInitedStype) return;
            
            textStyle = new GUIStyle(EditorStyles.label);
            textStyle.fixedHeight = elementHeight;
            textStyle.alignment = TextAnchor.MiddleLeft;

            Texture2D selectedBg = new Texture2D(1, 1, TextureFormat.RGB24, false);
            Texture2D hightLightBg = new Texture2D(1, 1, TextureFormat.RGB24, false);
            if (EditorGUIUtility.isProSkin)
            {
                selectedBg.LoadImage(System.Convert.FromBase64String(s_SelectedBg_Pro));
                hightLightBg.LoadImage(System.Convert.FromBase64String(s_HightLightBg_Pro));
            }
            else
            {
                selectedBg.LoadImage(System.Convert.FromBase64String(s_SelectedBg_Light));
                hightLightBg.LoadImage(System.Convert.FromBase64String(s_HightLightBg_Light));
            }
            selectedBackgroundStyle = new GUIStyle();
            selectedBackgroundStyle.normal.background = selectedBg;
            normalBackgroundStyle = new GUIStyle();
            normalBackgroundStyle.hover.background = hightLightBg;

            isInitedStype = true;
        }

        void Update()
        {
            Repaint();
        }
    }
}