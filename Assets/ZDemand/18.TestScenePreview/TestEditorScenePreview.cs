using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TestEditorScenePreview
{
    private Texture2D m_FloorTexture;
    private Mesh m_FloorPlane;
    
    public void DoAvatarPreview(Rect rect, GUIStyle background)
    {
        Init();
    }

    public void Init()
    {
        if (m_FloorPlane == null)
        {
            m_FloorPlane = Resources.GetBuiltinResource(typeof(Mesh), "New-Plane.fbx") as Mesh;
        }
        
        if (m_FloorTexture == null)
        {
            m_FloorTexture = (Texture2D)EditorGUIUtility.Load("Avatar/Textures/AvatarFloor.png");
        }
    }
    
    
}
