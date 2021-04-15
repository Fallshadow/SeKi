using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class AnimClipAddCurveEditor : EditorWindow
{
    private AnimationClip curAnimationClip = null;
    
    [MenuItem("Tests/自制编辑器/动画文件加入IK等曲线工具")]
    public static void ShowExample()
    {
        AnimClipAddCurveEditor wnd = GetWindow<AnimClipAddCurveEditor>();
        wnd.titleContent = new GUIContent("AnimClipAddCurveEditor");
    }

    public void OnEnable()
    {
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ZDemand/15.AnimClipOtherCurveGetSet/Editor/AnimClipAddCurveEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ZDemand/15.AnimClipOtherCurveGetSet/Editor/AnimClipAddCurveEditor.uss");
        root.styleSheets.Add(styleSheet);

        ObjectField animationClip = root.Q<ObjectField>("ObjectField-AnimationClip");
        curAnimationClip = animationClip.value as AnimationClip;
        animationClip.objectType = typeof(AnimationClip);
        animationClip.value = null;
        animationClip.RegisterValueChangedCallback(evt =>
        {
            Object obj = evt.newValue;
            if (obj is AnimationClip)
            {
                curAnimationClip = obj as AnimationClip;
            }
            else
            {
                curAnimationClip = null;
                Debug.Log("当前动画文件为空！");
            }
        });
        
        IMGUIContainer curvesImguiContainer = root.Q<IMGUIContainer>("IMGUIContainer-CurveWindow");
        curvesImguiContainer.onGUIHandler = () =>
        {
            // AnimationCurve testCurve = EditorGUILayout.CurveField("", new AnimationCurve());

        };
    }
}