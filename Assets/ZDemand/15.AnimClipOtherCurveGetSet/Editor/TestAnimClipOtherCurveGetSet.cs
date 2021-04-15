using System.Collections;
using System.Collections.Generic;
using MarkerMetro.Unity.WinLegacy.Reflection;
using UnityEditor;
using UnityEngine;
using BindingFlags = System.Reflection.BindingFlags;

public static class TestAnimClipOtherCurveGetSet
{
    [MenuItem("Tests/工具/给选中的动画加曲线")]
    public static void AddIKCurve()
    {
        var objs = Selection.objects;
        if (objs == null || objs.Length == 0)
        {
            return;
        }

        foreach (var obj in objs)
        {
            if (obj is AnimationClip clip)
            {
                EditorCurveBinding binding = EditorCurveBinding.FloatCurve(string.Empty, typeof(Animator), "IKWeight");
                AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Linear(0, 1, 2, 3));
            }
                
            EditorUtility.SetDirty(obj);
        }
            
        AssetDatabase.SaveAssets();
    }
    
    [MenuItem("Tests/呼叫Unity内部窗口/选择某样东西呼出游戏中的Animationwindow")]
    public static void TestEditorAnimation()
    {
        var objs = Selection.objects;
        if (objs == null || objs.Length == 0 || objs.Length > 1)
        {
            return;
        }
        foreach (var obj in objs)
        {
            if (obj is AnimationClip clip)
            {
                AnimationWindowHandler.EditAnimationClip(obj as AnimationClip);
            }
        }
    }
}

public static class AnimationWindowHandler
{
    private static System.Type type = null;
            
    public static System.Type Type => (type == null)
        ? type = System.Type.GetType("UnityEditor.AnimationWindow, UnityEditor")
        : type;

    public static EditorWindow GetWindow()
    {
        return EditorWindow.GetWindow(Type);
    }
            
    public static bool EditAnimationClip(AnimationClip animationClip)
    {
        var method = Type.GetMethod("EditAnimationClip", BindingFlags.Instance | BindingFlags.Public);
        if (method != null) return (bool) method.Invoke(GetWindow(), new object[]{ animationClip });

        return false;
    }
}
