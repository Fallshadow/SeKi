using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class TestScenePreview
{
    private PreviewRenderUtility previewRenderUtility;
    private Vector2 m_Drag;
    private GameObject rootGO;
    private Mesh floorPlane;
    private float boundingVolumeScale = 1;
    public void OnPreviewGUI(Rect r, GUIStyle background)
    {
        if (r == default) return;
        
        m_Drag = Drag2D(m_Drag, r);
        if (Event.current.type == EventType.Repaint)
        { 
            previewRenderUtility.BeginPreview(r, background);   
            rootGO.transform.position = Vector2.zero;
            //调整相机位置与角度
            previewRenderUtility.camera.transform.position = Vector2.zero;
            previewRenderUtility.camera.transform.rotation = Quaternion.Euler(new Vector3(-m_Drag.y, -m_Drag.x, 0));
            previewRenderUtility.camera.transform.position = previewRenderUtility.camera.transform.forward * -6f;
            // 相机渲染
            previewRenderUtility.Render();
            Vector3 floorPos = rootGO.transform.position;     
            RenderTexture shadowMap = RenderPreviewShadowmap(previewRenderUtility.lights[0], boundingVolumeScale / 2, Vector3.one, floorPos, out var shadowMatrix);

            drawFloor(shadowMap, shadowMatrix, floorPos);
            // 结束并绘制
            previewRenderUtility.EndAndDrawPreview(r); 
        }
    }

    private RenderTexture RenderPreviewShadowmap(Light light, float scale, Vector3 center, Vector3 floorPos,
            out Matrix4x4 outShadowMatrix)
        {
            Assert.IsTrue(Event.current.type == EventType.Repaint);

            // Set ortho camera and position it
            var cam = previewRenderUtility.camera;
            cam.orthographic = true;
            cam.orthographicSize = scale * 2.0f;
            //cam.nearClipPlane = 1 * scale;
            //cam.farClipPlane = 25 * scale;
            cam.transform.rotation = light.transform.rotation;
            cam.transform.position = center - light.transform.forward * (scale * 5.5f);

            // Clear to black
            CameraClearFlags oldFlags = cam.clearFlags;
            cam.clearFlags = CameraClearFlags.SolidColor;
            Color oldColor = cam.backgroundColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);

            // Create render target for shadow map
            const int kShadowSize = 256;
            RenderTexture oldRT = cam.targetTexture;

            RenderTexture rt = RenderTexture.GetTemporary(kShadowSize, kShadowSize, 16);;
            rt.isPowerOfTwo = true;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.filterMode = FilterMode.Bilinear;
            cam.targetTexture = rt;
            RenderTexture.active = rt;
            
            // Enable character and render with camera into the shadowmap

                Renderer[] componentsInChildren = rootGO.GetComponentsInChildren<Renderer>();
                foreach (var renderer in componentsInChildren)
                {
                    renderer.enabled = true;
                }
                previewRenderUtility.camera.Render();

            // Draw a quad, with shader that will produce white color everywhere
            // where something was rendered (via inverted depth test)
            RenderTexture.active = rt;
            GL.PushMatrix();
            GL.LoadOrtho();
            shadowMaskMaterial.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Vertex3(0, 0, -99.0f);
            GL.Vertex3(1, 0, -99.0f);
            GL.Vertex3(1, 1, -99.0f);
            GL.Vertex3(0, 1, -99.0f);
            GL.End();

            // Render floor with black color, to mask out any shadow from character
            // parts that are under the preview plane
            GL.LoadProjectionMatrix(cam.projectionMatrix);
            GL.LoadIdentity();
            GL.MultMatrix(cam.worldToCameraMatrix);
            shadowPlaneMaterial.SetPass(0);
            GL.Begin(GL.QUADS);
            float sc = kFloorScale * scale;
            GL.Vertex(floorPos + new Vector3(-sc, 0, -sc));
            GL.Vertex(floorPos + new Vector3(sc, 0, -sc));
            GL.Vertex(floorPos + new Vector3(sc, 0, sc));
            GL.Vertex(floorPos + new Vector3(-sc, 0, sc));
            GL.End();

            GL.PopMatrix();
            
            // Shadowmap sampling matrix, from world space into shadowmap space
            Matrix4x4 texMatrix = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity,
                new Vector3(0.5f, 0.5f, 0.5f));
            outShadowMatrix = texMatrix * cam.projectionMatrix * cam.worldToCameraMatrix;

            // Restore previous camera parameters
            cam.orthographic = false;
            cam.clearFlags = oldFlags;
            cam.backgroundColor = oldColor;
            cam.targetTexture = oldRT;
            RenderTexture.active = oldRT;
            
            return rt;
        }
    public void InitPreviewRender()
    {
        if (previewRenderUtility != null)
        {
            return;
        }
        previewRenderUtility = new PreviewRenderUtility();
        rootGO = (GameObject)EditorGUIUtility.Load("Avatar/root.fbx");
        previewRenderUtility.AddSingleGO(rootGO);
        createFloor();
    }

    private Texture2D floorTexture = null;
    private Material floorMaterial = null;
    private const float kFloorScale = 35f;
    private Material shadowMaskMaterial;
    private Material shadowPlaneMaterial;
    private void createFloor()
    {
        if (floorPlane == null)
        {
            floorPlane = Resources.GetBuiltinResource(typeof(Mesh), "New-Plane.fbx") as Mesh;
        }

        //有的时候刷新会导致贴图没有了，所以每次刷新都重新创建一下
        floorTexture = Resources.Load<Texture2D>("Styles/grid-texture");

        if (floorMaterial == null)
        {
            Shader shader = EditorGUIUtility.LoadRequired("Previews/PreviewPlaneWithShadow.shader") as Shader;
            floorMaterial = new Material(shader)
            {
                mainTexture = floorTexture,
                mainTextureScale = Vector2.one * kFloorScale,
                hideFlags = HideFlags.HideAndDontSave
            };
        }
            
        if (shadowMaskMaterial == null)
        {
            Shader shader = EditorGUIUtility.LoadRequired("Previews/PreviewShadowMask.shader") as Shader;
            shadowMaskMaterial = new Material(shader);
            shadowMaskMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        if (shadowPlaneMaterial == null)
        {
            Shader shader = EditorGUIUtility.LoadRequired("Previews/PreviewShadowPlaneClip.shader") as Shader;
            shadowPlaneMaterial = new Material(shader);
            shadowPlaneMaterial.hideFlags = HideFlags.HideAndDontSave;
        }
    }
    
    // 预览摄像机的绘制层 Camera.PreviewCullingLayer
    // 为了防止引擎更改，可以通过反射获取，这里直接写值
    private const int kPreviewCullingLayer = 31;
    private Camera camera => previewRenderUtility.camera;
    
    void drawFloor(RenderTexture shadowMap, Matrix4x4 shadowMatrix, Vector3 floorPos)
    {
        Vector2 textureOffset = -new Vector2(floorPos.x, floorPos.z);

        Material mat = floorMaterial;
        Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 10);

        mat.SetTexture("_ShadowTexture", shadowMap);
        mat.SetMatrix("_ShadowTextureMatrix", shadowMatrix);
        mat.SetVector("_Alphas", new Vector4(0.5f, 0.3f, 0, 0));
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Background;
        
        Graphics.DrawMesh(floorPlane, matrix, mat, kPreviewCullingLayer, camera, 0);
    }
    
    static int sliderHash = "Slider".GetHashCode();
    public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
    {
        int id = GUIUtility.GetControlID(sliderHash, FocusType.Passive);
        Event evt = Event.current;
        switch (evt.GetTypeForControl(id))
        {
            case EventType.MouseDown:
                if (position.Contains(evt.mousePosition) && position.width > 50)
                {
                    GUIUtility.hotControl = id;
                    evt.Use();
                    EditorGUIUtility.SetWantsMouseJumping(1);
                }
                break;
            case EventType.MouseDrag:
                if (GUIUtility.hotControl == id)
                {
                    scrollPosition -= evt.delta * (evt.shift ? 3 : 1) / Mathf.Min(position.width, position.height) * 140.0f;
                    evt.Use();
                    GUI.changed = true;
                }
                break;
            case EventType.MouseUp:
                if (GUIUtility.hotControl == id)
                    GUIUtility.hotControl = 0;
                EditorGUIUtility.SetWantsMouseJumping(0);
                break;
        }
        return scrollPosition;
    }
}
