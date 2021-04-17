using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class TestScenePreviewWindow : EditorWindow
{
    private TestScenePreview testScenePreviewHandle = null;
    
    [MenuItem("Tests/自制编辑器/测试预览场景编辑器")]
    public static void ShowExample()
    {
        TestScenePreviewWindow wnd = GetWindow<TestScenePreviewWindow>();
        wnd.titleContent = new GUIContent("TestScenePreviewWindow");
    }

    public void OnEnable()
    {
        testScenePreviewHandle = new TestScenePreview();
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ZDemand/18.TestScenePreview/Editor/CopyGame/TestScenePreviewWindow.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ZDemand/18.TestScenePreview/Editor/CopyGame/TestScenePreviewWindow.uss");
        root.styleSheets.Add(styleSheet);

        IMGUIContainer testModelPreview = root.Q<IMGUIContainer>("IMGUIContainer-TestPreview");
        GUIStyle background = "PreBackgroundSolid";
        testModelPreview.onGUIHandler = () =>
        {
            testScenePreviewHandle.InitPreviewRender();
            testScenePreviewHandle.OnPreviewGUI(testModelPreview.contentRect, background);
        };
    }
}