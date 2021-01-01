using UnityEditor;
using UnityEditor.SceneManagement;

public static class OpenSceneTool
{
    private const string SceneFolder = "Assets/Scenes";
    private const string SceneExtension = ".unity";
    private const string SceneFullName = SceneFolder + "/{0}" + SceneExtension;

    private const string Entry = "Entry";
    private const string Test = "Test";


    [MenuItem("OpenSceneTool/Open " + Entry)]
    public static void OpenEntryScene()
    {
        OpenScene(Entry);
    }

    [MenuItem("OpenSceneTool/Open " + Test)]
    public static void OpenTestScene()
    {
        OpenScene(Test);
    }

    public static void OpenScene(string sceneName,bool isFullPath = false)
    {
        if(EditorApplication.isPlaying)
        {
            return;
        }

        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            if(isFullPath)
            {
                EditorSceneManager.OpenScene(sceneName);
            }
            else
            {
                // 保存了场景
                string scenePath = string.Format(SceneFullName, sceneName);
                EditorSceneManager.OpenScene(scenePath);
            }
        }
        else
        {
            // 遗弃了修改，是不是要做点啥
        }
    }
}
