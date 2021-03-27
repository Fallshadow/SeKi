using System;
using ASeKi.Extension;
using UnityEditor;
using UnityEngine;
using System.ComponentModel;
using constants;

[CustomEditor(typeof(ASeKi.data.CCFreeLookCameraPointConfigSO))]
public class CCFreeLookEditor : Editor
{
    private ASeKi.data.CCFreeLookCameraPointConfigSO configSO = null;
    private SerializedProperty configFashionData = null;
    private SerializedProperty configPetData = null;
    private SerializedProperty sceneEnum = null;
    
    int selectedConfigIndex = 0;
    private void OnEnable()
    {
        configSO = target as ASeKi.data.CCFreeLookCameraPointConfigSO;
        configFashionData = serializedObject.FindProperty("dictCCFashionFreeLook");
        configPetData = serializedObject.FindProperty("dictCCPetFreeLook");
        sceneEnum = serializedObject.FindProperty("curScene");
       // configSO.ccFashionFreeLook.Add(0,0);
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(sceneEnum, new GUIContent("应用场景："));
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginVertical(GUI.skin.box);
        switch (configSO.curScene)
        {
            case CCFreeLookScene.FashionScene:
                for (int index = 0; index < configSO.ccFashionFreeLook.Count; index++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"第0号元素：",GUILayout.Width(120));
                    //EditorGUILayout.("这里的数字对应场景里的0~n号相机",configSO.ccFashionFreeLook.Keys[index]);
                    EditorGUILayout.EndHorizontal();
                    
                }
                break;
            case CCFreeLookScene.PetScene:
                
                break;
        }
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }
}
