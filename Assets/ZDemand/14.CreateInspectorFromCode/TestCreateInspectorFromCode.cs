using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "TestCreateInspectorFromCode", menuName = "TestCreateInspectorFromCode")]
public class TestCreateInspectorFromCode: ScriptableObject
{
    
}

public static class TestCreateInspectorFromCodeEditor
{
    [MenuItem("Tests/呼叫Unity内部窗口/选择某样东西复制Inspector出来")]
    public static void SelectThingForInspector()
    {
        string guid = AssetDatabase.FindAssets("t:TestCreateInspectorFromCode")[0];
        string path = AssetDatabase.GUIDToAssetPath(guid);
        TestCreateInspectorFromCode code = AssetDatabase.LoadAssetAtPath<TestCreateInspectorFromCode>(path);
        Selection.activeObject = code;
        Selection.SetActiveObjectWithContext(code,null);

        EditorWindow inspectorWindow = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow"));
        Vector2 size = new Vector2( inspectorWindow.position.width, inspectorWindow.position.height );
        inspectorWindow = EditorWindow.Instantiate(inspectorWindow);
        inspectorWindow.minSize = size;
        inspectorWindow.Show();
        inspectorWindow.Focus();
    }
    
    [MenuItem("Tests/呼叫Unity内部窗口/选择某样东西复制Animation出来")]
    public static void SelectThingForAnimation()
    {
        string guid = AssetDatabase.FindAssets("t:AnimationClip")[0];
        string path = AssetDatabase.GUIDToAssetPath(guid);
        AnimationClip code = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
        Selection.activeObject = code;
        Selection.SetActiveObjectWithContext(code,null);

        EditorWindow AnimationWindow = EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.AnimationWindow"));
        Vector2 size = new Vector2( AnimationWindow.position.width, AnimationWindow.position.height );
        AnimationWindow.minSize = size;
        AnimationWindow.Show();
        AnimationWindow.Focus();
    }
}
