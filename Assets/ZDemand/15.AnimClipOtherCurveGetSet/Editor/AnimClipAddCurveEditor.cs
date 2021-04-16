using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class AnimClipAddCurveEditor : EditorWindow
{
    private AnimationClip curAnimationClip = null;
    private List<AnimationCurve> curves = new List<AnimationCurve>();
    private List<EditorCurveBinding> bindings = new List<EditorCurveBinding>();
    
    [MenuItem("Tests/自制编辑器/动画文件加入IK等曲线工具")]
    public static void ShowExample()
    {
        AnimClipAddCurveEditor wnd = GetWindow<AnimClipAddCurveEditor>();
        wnd.titleContent = new GUIContent("AnimClipAddCurveEditor");
    }

    public void OnEnable()
    {
        // 初始化
        VisualElement root = rootVisualElement;
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/ZDemand/15.AnimClipOtherCurveGetSet/Editor/AnimClipAddCurveEditor.uxml");
        VisualElement labelFromUXML = visualTree.CloneTree();
        root.Add(labelFromUXML);
        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/ZDemand/15.AnimClipOtherCurveGetSet/Editor/AnimClipAddCurveEditor.uss");
        root.styleSheets.Add(styleSheet);
        // 动画文件塞入
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
                refreshCurves(curAnimationClip);
            }
            else
            {
                curAnimationClip = null;
                Debug.LogError("当前动画文件为空！");
            }
        });
        // imgui动画文件控制
        IMGUIContainer curvesImguiContainer = root.Q<IMGUIContainer>("IMGUIContainer-CurveWindow");
        curvesImguiContainer.onGUIHandler = () =>
        {
            // AnimationCurve testCurve = EditorGUILayout.CurveField("", new AnimationCurve());
            EditorGUILayout.BeginVertical();
            
            for (int index = 0; index < bindings.Count; index++)
            {
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField(bindings[index].propertyName);
                curves[index] = EditorGUILayout.CurveField(curves[index]);
                
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        };
        // 保存按钮
        Button saveBtn = root.Q<Button>("Button-Save-Clip");
        saveBtn.clicked += writeBackToAnimationClip;
    }

    private void refreshCurves(AnimationClip clip)
    {
        if (curAnimationClip == null)
        {
            Debug.LogError("当前动画文件为空！");
            return;
        }
        Undo.RecordObject(clip,"Clip");
        bindings.Clear();
        curves.Clear();
        EditorCurveBinding[] curveBindings = AnimationUtility.GetCurveBindings(clip);
        foreach (var curveBinding in curveBindings)
        {
            bindings.Add(curveBinding);
            curves.Add(AnimationUtility.GetEditorCurve(clip, curveBinding));
        }
    }

    private void writeBackToAnimationClip()
    {
        if (curAnimationClip == null)
        {
            Debug.LogError("当前动画文件为空！");
            return;
        }
        
        for (int index = 0; index < bindings.Count; index++)
        {
            AnimationUtility.SetEditorCurve(curAnimationClip, bindings[index], curves[index]);
        }
        
        EditorUtility.SetDirty(curAnimationClip);
        AssetDatabase.SaveAssets();
    }
}