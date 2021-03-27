using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class TestUiBuilder : EditorWindow
{
    [MenuItem("Window/UIElements/TestUiBuilder")]
    public static void ShowExample()
    {
        TestUiBuilder wnd = GetWindow<TestUiBuilder>();
        wnd.titleContent = new GUIContent("TestUiBuilder");
    }

    public void OnEnable()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

        // Import UXML
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ZDemand/4.UiBuilder/Editor/TestUiBuilder/TestUiBuilder.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);

        // A stylesheet can be added to a VisualElement.
        // The style will be applied to the VisualElement and all of its children.
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ZDemand/4.UiBuilder/Editor/TestUiBuilder/TestUiBuilderUSS.uss");
        VisualElement labelWithStyle = new Label("Hello World! With Style");
        labelWithStyle.styleSheets.Add(styleSheet);
        root.Add(labelWithStyle);
        root.styleSheets.Add(styleSheet);
    }
}