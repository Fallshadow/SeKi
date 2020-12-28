using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

// 编辑器的场景管理器
public class UnityAPITestEditorSceneManager
{
    [MenuItem("UnityStudy/Editor/SceneManagement/EditorSceneManager/Prop_DefaultSceneCullingMask")]
    public static void Prop_DefaultSceneCullingMask()
    {
        Debug.Log("所有摄像机绘制的场景剔除蒙版。默认情况下，所有场景都以该消隐蒙版开始。");
        Debug.Log(EditorSceneManager.DefaultSceneCullingMask);
    }

    [MenuItem("UnityStudy/Editor/SceneManagement/EditorSceneManager/Prop_loadedSceneCount")]
    public static void Prop_loadedSceneCount()
    {
        Debug.Log("当前加载的场景数。");
        Debug.Log(EditorSceneManager.loadedSceneCount);
    }
    
    [MenuItem("UnityStudy/Editor/SceneManagement/EditorSceneManager/Prop_playModeStartScene")]
    public static void Prop_playModeStartScene()
    {
        Debug.Log("启动播放模式时加载此SceneAsset。 如果将此属性设置为SceneAsset，则当您启动播放模式时，Unity将加载此SceneAsset而不是当前在编辑器中打开的场景。");
        Debug.Log(EditorSceneManager.playModeStartScene);
    }

    [MenuItem("UnityStudy/Editor/SceneManagement/EditorSceneManager/PropTest_playModeStartScene")]
    public static void PropTest_playModeStartScene()
    {
        Debug.Log("将此属性设置为SceneAsset，当您启动播放模式时，Unity将加载'Assets/Scenes/Entry.unity'而不是当前在编辑器中打开的场景。");
        SceneAsset myWantedStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/Entry.unity");
        if(myWantedStartScene == null)
        {
            Debug.Log("Assets/Scenes/Entry.unity没了呜呜");
        }
        else
        {
            EditorSceneManager.playModeStartScene = myWantedStartScene;
            Debug.Log(EditorSceneManager.playModeStartScene);
            Debug.Log("修改成了，你试试");
        }
    }

    [MenuItem("UnityStudy/Editor/SceneManagement/EditorSceneManager/Prop_preventCrossScene")]
    public static void Prop_preventCrossScene()
    {
        Debug.Log(@"此设置的默认值为“ true”。这意味着默认情况下，您无法在Unity编辑器中创建跨场景引用。
                    禁用此设置意味着：
                    1）允许将引用从一个场景中的GameObject拖动到另一个场景中的另一个GameObject的Component字段。
                    2）对象选择器（检查器中大多数可分配字段旁边的小目标图标）列出了所有打开的场景中的选项，而不仅仅是GameObject自己的场景。
                    3）将GameObjects从一个场景拖到另一个场景会导致跨场景引用。发生这种情况时，不再记录警告。");
        Debug.Log(EditorSceneManager.preventCrossSceneReferences);
    }

    [MenuItem("UnityStudy/Editor/SceneManagement/EditorSceneManager/StaticMethod_SaveCurrentModifiedScenesIfUserWantsTo")]
    public static void StaticMethod_SaveCurrentModifiedScenesIfUserWantsTo()
    {
        Debug.Log("如果场景有改变，询问下是否要保存场景");
        Debug.Log(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo());
    }
}
