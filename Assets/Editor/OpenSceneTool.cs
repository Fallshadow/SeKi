using UnityEditor;
using UnityEditor.SceneManagement;

public static class OpenSceneTool
{
    private const string SceneFolder = "Assets/Scenes";
    private const string SceneExtension = ".unity";
    private const string SceneFullName = SceneFolder + "/{0}" + SceneExtension;

    [MenuItem("OpenSceneTool/Open " + "Entry")]
    public static void OpenEntryScene()
    {

    }

    public static void OpenScene(string sceneName)
    {
        if(EditorApplication.isPlaying)
        {
            return;
        }

    }
}
