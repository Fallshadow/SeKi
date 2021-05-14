using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BuiltInResourcesWindow_2_Mono : MonoBehaviour
{
    [SerializeField] 
    public static List<string> texName = new List<string> ();
    [SerializeField] 
    public List<Texture> texure=new List<Texture>();
    public List<string> name = new List<string> ();
    void OnEnable ()
    {
#if UNITY_EDITOR
        name=texName;
        for (int i = 0; i < texName.Count; i++) {
            texure.Add(EditorGUIUtility.Load (texName[i]) as Texture);
        }
#endif
    }


    //[InitializeOnLoadMethod]
    static void GetBultinAssetNames ()
    {
        var flags = BindingFlags.Static | BindingFlags.NonPublic;
        var info = typeof(EditorGUIUtility).GetMethod ("GetEditorAssetBundle", flags);
        var bundle = info.Invoke (null, new object[0]) as AssetBundle;
        foreach (var n in bundle.GetAllAssetNames()) {
            if (n.IndexOf(".png")>=0) {
                texName.Add (n);
            }
            Debug.Log (n);
        }
    }
}