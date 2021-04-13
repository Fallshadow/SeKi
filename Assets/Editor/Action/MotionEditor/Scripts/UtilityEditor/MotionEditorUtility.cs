using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ASeKi.action
{
    public static class MotionEditorUtility
    {
        public static EditorUtilityStyle Style = new EditorUtilityStyle();

        // 将完全路径转换为资源路径
        public static string GetAssetPath(string path)
        {
            return "Assets" + path.Remove(0, Application.dataPath.Length);
        }
        
        // 输入规则
        public static bool VerifyStringValid(string input, out string errorDescription)
        {
            if (input.Length == 0)
            {
                errorDescription = "标题不能为空";
                return false;
            }
            string regEx = "[`~!@#$%^&*()+=|{}':;',\\[\\].<>/?~！@#￥%……&*（）——+|{}【】‘；：”“’。，、？.]";
            for (int i = 0; i < regEx.Length; i++)
            {
                if (input.Contains(regEx[i]))
                {
                    errorDescription = @"标题不能包含[`~!@#$%^&*()+=|{}':;',\\[\\].<>/?~！@#￥%……&*（）——+|{}【】‘；：”“’。，、？]";

                    return false;
                }
            }
            if (input.Length > 30)
            {
                errorDescription = "标题文字长度超过30个字符";
                return false;
            }
            errorDescription = "";
            return true;
        }
        
        // 获取当前窗口的中心位置
        public static Rect GetEditorMainWindowPos()
        {
            var containerWinType = System.AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject))
                .Where(t => t.Name == "ContainerWindow").FirstOrDefault();
            if (containerWinType == null)
                throw new System.MissingMemberException(
                    "Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            var showModeField = containerWinType.GetField("m_ShowMode",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (showModeField == null || positionProperty == null)
                throw new System.MissingFieldException(
                    "Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            var windows = Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (var win in windows)
            {
                var showmode = (int) showModeField.GetValue(win);
                if (showmode == 4) // main window
                {
                    var pos = (Rect) positionProperty.GetValue(win, null);
                    return pos;
                }
            }

            throw new System.NotSupportedException(
                "Can't find internal main window. Maybe something has changed inside Unity");
        }
        
        public static string GetMaskValue(int maskValue, Type enumType)
        {
            if (maskValue == 0)
            {
                return "Default";
            }
            
            string[] allValue = GetMaskValues(maskValue, enumType);
            if (allValue == null || allValue.Length == 0)
            {
                return null;
            }
            
            string result = "";
            for (var index = 0; index < allValue.Length; index++)
            {
                if (index == 0) continue;
                var maskStr = allValue[index];
                if (index == allValue.Length - 1)
                {
                    result += $"{maskStr}";
                }
                else
                {
                    result += $"{maskStr} | ";
                }
            }
            return result;
        }
        
        public static string[] GetMaskValues(int maskValue, Type enumType)
        {
            FieldInfo[] allFields = enumType.GetFields().Where(x=>x.FieldType == enumType).ToArray();
            List<string> options = new List<string>();

            for (var index = 0; index < allFields.Length; index++)
            {
                var field = allFields[index];
                var attributes = field.GetCustomAttributes(false);
                if (attributes.Length == 0) continue;
                var ret = attributes.ToList().Find(x => x is ActionGroupAttr);
                if (ret == null) continue;
                var attr = ret as ActionGroupAttr;
                options.Add(attr.GroupTag);
            }

            List<string> result = new List<string>();
            for (int i = 0; i < options.Count; i++)
            {
                string op = options[i];
                int value = i == 0 ? 0 : 1 << (i-1);

                if ( (maskValue & value) == value)
                {
                    result.Add(op);
                }
            }
            return result.Count > 0 ? result.ToArray() : null;
        }
    }
    
    struct GUILabelWidthOverride : IDisposable
    {
        private float lableWidth;
        
        public GUILabelWidthOverride(float width)
        {
            lableWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
        }

        public void Dispose()
        {
            EditorGUIUtility.labelWidth = lableWidth;
        }
    }
    
    
}