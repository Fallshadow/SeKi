using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class CopyEditor : EditorWindow
{
    private TestEditorScenePreview m_tesPreview = null;
    
    [MenuItem("Tests/自制编辑器/测试编辑器预览场景编辑器")]
    public static void ShowExample()
    {
        CopyEditor wnd = GetWindow<CopyEditor>();
        wnd.titleContent = new GUIContent("CopyEditor");
    }

    public void OnEnable()
    {
        VisualElement root = rootVisualElement;
        var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ZDemand/18.TestScenePreview/Editor/CopyEditor/CopyEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);
        
        IMGUIContainer testModelPreview = root.Q<IMGUIContainer>("IMGUIContainer-TestPreview");
        GUIStyle background = "PreBackgroundSolid";
//        testModelPreview.onGUIHandler = () =>
//        {
//            m_tesPreview
//        };
    }
}