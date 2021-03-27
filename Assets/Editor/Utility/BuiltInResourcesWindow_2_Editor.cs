using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuiltInResourcesWindow_2_Mono))] 
public class BuiltInResourcesWindow_2_Editor : Editor 
{
    public override void OnInspectorGUI() 
    {
        BuiltInResourcesWindow_2_Mono test = (BuiltInResourcesWindow_2_Mono) target;
        string a;
        for (int i = 0; i < test.texure.Count; i++) {
            if (test.texure [i]) {
                a  = test.texure [i].name;
            } else
                a = "";
            test.texure[i]=EditorGUILayout.ObjectField(i+":"+a,test.texure[i],typeof(Texture),true) as Texture;
        }
    }
}