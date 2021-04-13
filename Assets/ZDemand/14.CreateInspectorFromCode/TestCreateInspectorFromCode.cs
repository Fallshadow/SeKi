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
    [MenuItem("Tests/SelectThingForInspector")]
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
}
