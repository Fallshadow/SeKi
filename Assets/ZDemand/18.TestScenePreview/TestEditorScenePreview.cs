using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TestEditorScenePreview
{
    private PreviewRenderUtility previewUtility
    {
        get
        {
            if (m_PreviewUtility == null)
            {
                m_PreviewUtility = new PreviewRenderUtility();
                m_PreviewUtility.camera.fieldOfView = 30.0f;
                m_PreviewUtility.camera.allowHDR = false;
                m_PreviewUtility.camera.allowMSAA = false;
                m_PreviewUtility.ambientColor = new Color(.1f, .1f, .1f, 0);
                m_PreviewUtility.lights[0].intensity = 1.4f;
                m_PreviewUtility.lights[0].transform.rotation = Quaternion.Euler(40f, 40f, 0);
                m_PreviewUtility.lights[1].intensity = 1.4f;
            }
            return m_PreviewUtility;
        }
    }
    private PreviewRenderUtility m_PreviewUtility;
    
    #region PreviewObj
    
    private Texture2D m_FloorTexture;
    private Mesh m_FloorPlane;
    
    #endregion
    
    
    public void DoAvatarPreview(Rect rect, GUIStyle background)
    {
        Init();
        
        if (Event.current.type == EventType.Repaint)
        {
            DoRenderPreview(rect, background);
            previewUtility.EndAndDrawPreview(rect);
        }
        
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

    public void DoRenderPreview(Rect previewRect, GUIStyle background)
    {
        previewUtility.BeginPreview(previewRect, background);
        previewUtility.Render(false);
        
    }
    
    
}
