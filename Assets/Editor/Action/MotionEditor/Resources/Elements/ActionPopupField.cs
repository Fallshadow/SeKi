using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ASeKi.action
{
    public class ActionPopupField
    {
        VisualElement choices = null;
        Label labelTitle = null;
        Button popupField = null;
        Action<string> callback = null;
        
        public void Init(string title, List<string> names, string defaultStr, Action<string> cb)
        {
            labelTitle.text = title;
            Button popupField = null;
            Button btn = new Button();
            btn.text = string.IsNullOrEmpty(defaultStr) && !names.IsNullOrEmpty() ? names.First() : defaultStr;

            btn.clicked += () =>
            {
                PopupWindow window = ScriptableObject.CreateInstance<PopupWindow>();
                window.Init(names.IndexOf(btn.text), names, select =>
                {
                    callback?.Invoke(select);
                    btn.text = select;
                    Debug.Log("Current Select" + select);
                });
                window.titleContent = new GUIContent("请选择:");
                window.Show();
                Vector2 curPos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                Rect r = new Rect(curPos.x, curPos.y, window.position.width, window.position.height);
                window.AutoFixPos(r);
            };
            popupField = btn;
            choices.Add(popupField);
            callback = cb;
        }
        
        public void Set(Label label, VisualElement ve)
        {
            labelTitle = label;
            choices = ve;
        }

        public void Reset(List<string> list, string defaultStr)
        {
            choices.Remove(popupField);
            popupField = null;
            Init(labelTitle.text, list, defaultStr, callback);
        }

        public void ChangeLabel(string str)
        {
            popupField.text = str;
        }

        public void ChangeTitle(string title)
        {
            labelTitle.text = title;
        }
    }
}