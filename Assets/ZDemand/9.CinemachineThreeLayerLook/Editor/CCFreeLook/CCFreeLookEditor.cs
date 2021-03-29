using System;
using ASeKi.Extension;
using UnityEditor;
using UnityEngine;
using System.ComponentModel;

namespace CCameraUtility
{
    [CustomEditor(typeof(CCFreeLookCameraPointConfigSO))]
    public class CCFreeLookEditor : Editor
    {
        private CCFreeLookCameraPointConfigSO configSO = null;
        int selectedConfigIndex = 0;

        private void OnEnable()
        {
            configSO = target as CCFreeLookCameraPointConfigSO;
            if (configSO.ccFashionFreeLook.Count == 0)
            {
                foreach (CCFreeLookFashionCameraPoint temp in Enum.GetValues(typeof(CCFreeLookFashionCameraPoint)))
                {
                    configSO.ccFashionFreeLook.Add(new CCFreeLookCameraPos(CCFreeLookScene.FashionScene,(int)temp));
                }
            }
            if (configSO.ccPetFreeLook.Count == 0)
            {
                foreach (CCFreeLookPetCameraPoint temp in Enum.GetValues(typeof(CCFreeLookPetCameraPoint)))
                {
                    configSO.ccPetFreeLook.Add(new CCFreeLookCameraPos(CCFreeLookScene.PetScene,(int)temp));
                }
            }
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("应用场景：");
            configSO.curScene = (CCFreeLookScene)EditorGUILayout.EnumPopup(configSO.curScene);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            switch (configSO.curScene)
            {
                case CCFreeLookScene.FashionScene:
                    for (int index = 0; index < configSO.ccFashionFreeLook.Count; index++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"第{index}个相机");
                        configSO.ccFashionFreeLook[index].CameraIndex = (int)(CCFreeLookFashionCameraPoint)EditorGUILayout.EnumPopup((CCFreeLookFashionCameraPoint)configSO.ccFashionFreeLook[index].CameraIndex, GUILayout.Width(120));
                        configSO.ccFashionFreeLook[index].PosId = (int)(CCFreeLookCameraLayer)EditorGUILayout.EnumPopup((CCFreeLookCameraLayer)configSO.ccFashionFreeLook[index].PosId, GUILayout.Width(120));
                        EditorGUILayout.EndHorizontal();
                    }
                    // LayoutAddAndDelButton(
                    //     () => { configSO.ccFashionFreeLook.Add(new CCFreeLookCameraPos(CCFreeLookScene.FashionScene,configSO.ccFashionFreeLook.Count)); },
                    //     () => { configSO.ccFashionFreeLook.RemoveAt(configSO.ccFashionFreeLook.Count -1);}
                    // );
                    if (GUILayout.Button(new GUIContent("Reset")))
                    {
                        configSO.ccFashionFreeLook.Clear();
                        OnEnable();
                    }
                    break;
                case CCFreeLookScene.PetScene:
                    for (int index = 0; index < configSO.ccPetFreeLook.Count; index++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"第{index}个相机");
                        configSO.ccPetFreeLook[index].CameraIndex = (int)(CCFreeLookPetCameraPoint)EditorGUILayout.EnumPopup((CCFreeLookPetCameraPoint)configSO.ccPetFreeLook[index].CameraIndex, GUILayout.Width(120));
                        configSO.ccPetFreeLook[index].PosId = (int)(CCFreeLookCameraLayer)EditorGUILayout.EnumPopup((CCFreeLookCameraLayer)configSO.ccPetFreeLook[index].PosId, GUILayout.Width(120));
                        EditorGUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button(new GUIContent("Reset")))
                    {
                        configSO.ccPetFreeLook.Clear();
                        OnEnable();
                    }
                    break;
            }
            EditorGUILayout.EndVertical();
            while(configSO.ccSceneMaxLayer.Count <= (int) configSO.curScene)
            {
                configSO.ccSceneMaxLayer.Add(0);
            }
            configSO.ccSceneMaxLayer[(int) configSO.curScene] = EditorGUILayout.IntField(new GUIContent("当前场景应用的层数"), configSO.ccSceneMaxLayer[(int) configSO.curScene]);
            
        }
        // private void LayoutAddAndDelButton(Action add,Action del)
        // {
        //     EditorGUILayout.BeginHorizontal();
        //     EditorGUILayout.LabelField("");
        //     if (GUILayout.Button(EditorGUIUtility.FindTexture("d_Toolbar Plus"), new[] {GUILayout.Width(20), GUILayout.Height(20)}))
        //     {
        //         add?.Invoke();
        //     }
        //
        //     if (GUILayout.Button(EditorGUIUtility.FindTexture("d_Toolbar Minus"), new[] {GUILayout.Width(20), GUILayout.Height(20)}))
        //     {
        //         del?.Invoke();    
        //     }
        //     EditorGUILayout.EndHorizontal();
        // }
    }
}