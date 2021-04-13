using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ASeKi.action
{
    public class CreateMotionStateWindow : EditorWindow
    {
        public Action<string, string> Finish;    // 创建状态的实际回调
        
        private string inputStateName = "NewState";
        private string resultStateName;
        private int weaponSelect;
        private string[] weaponList = new string[6] {"Normal", "Sword", "Hammer", "DualBlade", "BowGun", "Spear"};    // 待选择的武器类型
        private string resourcePath;             // 动画在unity的资源路径
        private UnityEngine.Object selectRes;    // 临时变量动画资源
        private AnimationClip clipRes;           // 真实动画资源Clip
        private BlendTree blendTreeRes;          // 真实动画资源blendTree
        private Vector2 scrollPosition;          
        
        private GUIContent folderContent;
        private GUIContent warningContent;

        

        private void OnEnable()
        {
            folderContent = EditorGUIUtility.IconContent("Folder Icon");
            warningContent = EditorGUIUtility.IconContent("Warning");
            warningContent.text = "Your name is Not Valid! Plz check! tips: ";
        }

        void OnGUI()
        {
            using (new EditorGUILayout.ScrollViewScope(scrollPosition, false, false))
            {

                #region 输入状态名称及检测

                using (new GUILabelWidthOverride(120))
                {
                    weaponSelect = EditorGUILayout.Popup("With Weapon : ", weaponSelect, weaponList);
                    inputStateName = EditorGUILayout.TextField($"New StateName:  {weaponSelect}_", inputStateName);
                }
                resultStateName = $"{weaponSelect}_{inputStateName}";
                bool isValid = verifyName(out string errorDescription);
                EditorGUILayout.LabelField("Please Enter You State Name!");
                if (!isValid)
                {
                    EditorGUILayout.BeginVertical();
                    EditorGUILayout.LabelField(warningContent, MotionEditorUtility.Style.WarningYellowStyle);
                    EditorGUILayout.LabelField($"{errorDescription}", MotionEditorUtility.Style.ErrorRedStyle);
                    EditorGUILayout.EndVertical();
                }
                
                #endregion

                #region 选择资源并持有

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(folderContent, GUILayout.Width(24), GUILayout.Height(24)))
                {
                    var clipPaths =
                        StandaloneFileBrowser.OpenFilePanel("Create Motion From Clip Or BlendTree:",
                            Application.dataPath + "/ResourceRex/Humman/Male/WeaponAnimations",
                            new[] {new ExtensionFilter("anim"), new ExtensionFilter("blendtree"),},
                            true);
                    
                    if (clipPaths == null || clipPaths.Length == 0)
                    {
                        Debug.Log("You select Nothing");
                        return;
                    }
                    
                    //Unity only return one path garbage!!
                    resourcePath = MotionEditorUtility.GetAssetPath(clipPaths.First());
                    selectRes = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(resourcePath);
                }

                EditorGUI.BeginChangeCheck();
                selectRes = EditorGUILayout.ObjectField(selectRes, typeof(UnityEngine.Object), GUILayout.Height(24));
                if (EditorGUI.EndChangeCheck())
                {
                    if (selectRes is BlendTree tree)
                    {
                        blendTreeRes = tree;
                    }
                    else if (selectRes is AnimationClip clip)
                    {
                        clipRes = clip;
                    }
                    else
                    {
                        selectRes = null;
                    }
                }
                
                EditorGUILayout.EndHorizontal();

                #endregion
                
                if (GUILayout.Button("OK"))
                {
                    if (string.IsNullOrEmpty(resourcePath))
                    {
                        EditorUtility.DisplayDialog("Unable to save prefab", $"Please Assign a resource(clip or blendtree) first", "Close");
                        return;
                    }

                    if (!verifyName(out string des))
                    {
                        EditorUtility.DisplayDialog("Unable to save prefab", $"Please specify a valid name. {des}", "Close");
                        return;
                    }

                    OnClickOk();
                    GUIUtility.ExitGUI();
                }
            }
        }

        bool verifyName(out string errorDescription)
        {
            return MotionEditorUtility.VerifyStringValid(inputStateName, out errorDescription);
        }

        void OnClickOk()
        {
            resultStateName = resultStateName.Trim();
            Finish?.Invoke(resultStateName, resourcePath);
            Debug.Log($"Current StateName is {resultStateName}");
            Close();
        }
    }
}