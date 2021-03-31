using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace ASeKi.action
{
    public class MotionPreview
    {
        private PreviewRenderUtility previewUtility;    // unity中的绘制IMGUI的
        private WeaponType currentWeaponType = WeaponType.WT_ALL;

        // 预览摄像机的绘制层 Camera.PreviewCullingLayer
        // 为了防止引擎更改，可以通过反射获取，这里直接写值
        private const int kPreviewCullingLayer = 31;
        
        public void OnPreviewGUI(Rect r, GUIStyle background)
        {
            initPreview();
            doCameraControl(r);

            if (Event.current.type != EventType.Repaint)
                return;

            doPreviewWindow(r, background);
        }

        void doCameraControl(Rect r)
        {
            // TODO:控制摄像头
        }
        
        #region 渲染工作
        private Camera camera => previewUtility.camera;
        private Material shadowMaskMaterial;
        private Material shadowPlaneMaterial;
        private float boundingVolumeScale;
        private Animator animator => previewInstance == null ? null : previewInstance.GetComponentInChildren<Animator>();
        
        private void drawFloor(RenderTexture shadowMap, Matrix4x4 shadowMatrix, Vector3 floorPos)
        {
            // Texture offset - negative in order to compensate the floor movement.
            Vector2 textureOffset = -new Vector2(floorPos.x, floorPos.z);

            //Draw Floor
            Material mat = floorMaterial;
            Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 10);

            //mat.mainTextureOffset = textureOffset * kFloorScale * 0.08f;
            mat.SetTexture("_ShadowTexture", shadowMap);
            mat.SetMatrix("_ShadowTextureMatrix", shadowMatrix);
            mat.SetVector("_Alphas", new Vector4(0.5f, 0.3f, 0, 0));
            mat.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Background;

            //            if (floorShow == null)
            //            {
            //                floorShow = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //                floorShow.GetComponentInChildren<MeshRenderer>().sharedMaterial = mat;
            //            }

            Graphics.DrawMesh(floorPlane, matrix, mat, kPreviewCullingLayer, camera, 0);
        }
        
        void doPreviewWindow(Rect r, GUIStyle background)
        {
            var probe = RenderSettings.ambientProbe;
            previewUtility.camera.cameraType = CameraType.SceneView;
            previewUtility.BeginPreview(r, background);
            
//            updateRoot();
//            
//            setupPreviewLightingAndFx(probe);
//            
            var bodyPosition = animator.bodyPositionInternal();
//            
//            Vector3 floorPos = referenceInstance.transform.position; // why
            Vector3 floorPos = previewInstance.transform.position;

            RenderTexture shadowMap = RenderPreviewShadowmap(previewUtility.lights[0], boundingVolumeScale / 2, bodyPosition, floorPos, out var shadowMatrix);
            
//            updateCamera();
//
//            //Draw Preview Target
//            SetPreviewCharacterEnabled(true, true);

            previewUtility.Render();

//            SetPreviewCharacterEnabled(false, false);
            
            drawFloor(shadowMap, shadowMatrix, floorPos);
            
            var clearMode = previewUtility.camera.clearFlags;
            previewUtility.camera.clearFlags = CameraClearFlags.Nothing;

            previewUtility.Render(false);
            previewUtility.camera.clearFlags = clearMode;
            
            RenderTexture.ReleaseTemporary(shadowMap);
            
            drawSceneHandles();

            previewUtility.EndAndDrawPreview(r);
        }
        
        private RenderTexture RenderPreviewShadowmap(Light light, float scale, Vector3 center, Vector3 floorPos, out Matrix4x4 outShadowMatrix)
        {
            Assert.IsTrue(Event.current.type == EventType.Repaint);
            
            var cam = previewUtility.camera;
            cam.orthographic = true;
            cam.orthographicSize = scale * 2.0f;
            cam.transform.rotation = light.transform.rotation;
            cam.transform.position = center - light.transform.forward * (scale * 5.5f);

            CameraClearFlags oldFlags = cam.clearFlags;
            cam.clearFlags = CameraClearFlags.SolidColor;
            Color oldColor = cam.backgroundColor;
            cam.backgroundColor = new Color(0, 0, 0, 0);
            
            const int kShadowSize = 256;
            RenderTexture oldRT = cam.targetTexture;

            RenderTexture rt = RenderTexture.GetTemporary(kShadowSize, kShadowSize, 16);;
            rt.isPowerOfTwo = true;
            rt.wrapMode = TextureWrapMode.Clamp;
            rt.filterMode = FilterMode.Bilinear;
            cam.targetTexture = rt;
            RenderTexture.active = rt;
            
            //SetPreviewCharacterEnabled(true, false);
            previewUtility.camera.Render();
            
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
            
            Matrix4x4 texMatrix = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity,
                new Vector3(0.5f, 0.5f, 0.5f));
            outShadowMatrix = texMatrix * cam.projectionMatrix * cam.worldToCameraMatrix;

            cam.orthographic = false;
            cam.clearFlags = oldFlags;
            cam.backgroundColor = oldColor;
            cam.targetTexture = oldRT;
            RenderTexture.active = oldRT;
            return rt;
        }
        
        void drawSceneHandles()
        {
            Assert.IsTrue(Event.current.type == EventType.Repaint);
            
            Handles.SetCamera(previewUtility.camera);

//            using (new HandlesColorOverride(new Color(1f, 0.71f, 0.17f, 0.5f)))
//            {
//                drawWeaponInstanceCollider(weaponInstance);
//                drawWeaponInstanceCollider(weaponInstance2);
//            }

            previewUtility.Render();
        }
        #endregion
        
        #region 创建工作

        private void initPreview()
        {
            if (previewUtility != null) return;
            previewUtility = new PreviewRenderUtility(true);

//            previewUtility.camera.fieldOfView = 30.0f;
//            previewUtility.camera.allowHDR = true;
//            previewUtility.camera.allowMSAA = true;
//            previewUtility.camera.nearClipPlane = 0.1f;
//            previewUtility.camera.farClipPlane = 10000f;
//            previewUtility.camera.cullingMask = 1 << kPreviewCullingLayer;
//            previewUtility.camera.renderingPath = RenderingPath.Forward;
//            
//            previewUtility.ambientColor = new Color(0.7f, 0.7f, 0.7f, 0);
//            previewUtility.lights[0].intensity = 1.15f;
//            previewUtility.lights[0].transform.rotation = Quaternion.Euler(40f, 160f, 0);
//            previewUtility.lights[1].intensity = 1f;
//            previewUtility.lights[1].transform.rotation = Quaternion.Euler(40f, 40f, 0);

            createPreviewInstances();

            // floor
            createFloor();
        }

        #region 创建场景需要的实例
        
        private GameObject defaultAvatar;          // 仅仅创建出来用
        private GameObject previewInstance;        // 真实使用的实例
        
        private void createPreviewInstances()
        {
            destroyPreviewInstances();

            currentWeaponType = WeaponType.WT_ALL;

            if (previewInstance == null)
            {
                verifyAvatarPrefab();
                instantiateAvatar(defaultAvatar);
                
                //caculate Bounds // TODO:创建的bound是什么，有什么意义？
//                previewBounds = new Bounds(previewInstance.transform.position, Vector3.zero);
//                MotionEditorUtility.GetRenderableBoundsRecurse(ref previewBounds, previewInstance);
//                boundingVolumeScale = Mathf.Max(previewBounds.size.x, Mathf.Max(previewBounds.size.y, previewBounds.size.z));
            }
            // TODO:创建的都是什么，有什么意义？

//            GameObject root = (GameObject) EditorGUIUtility.Load("Avatar/root.fbx");


//            if (directionInstance == null)
//            {
//                GameObject directionGO = (GameObject) EditorGUIUtility.Load("Avatar/arrow.fbx");
//                directionInstance = instantiateObjectInPreviewScene(directionGO);
//                directionInstance.transform.localScale *= 1.3f;
//            }
//
//            if (referenceInstance == null)
//            {
//                GameObject referenceGO = (GameObject) EditorGUIUtility.Load("Avatar/dial_flat.prefab");
//                referenceInstance = instantiateObjectInPreviewScene(referenceGO);
//                referenceInstance.transform.localScale *= 1f;
//            }
//
//            if (pivotInstance == null)
//            {
//                pivotInstance = instantiateObjectInPreviewScene(root);
//                pivotInstance.transform.localScale *= 0.3f;
//            }
//
//            if (rootInstance == null)
//            {
//                rootInstance = instantiateObjectInPreviewScene(root);
//                rootInstance.transform.localScale *= 0.3f;
//            }
        }

        // 删除物件预防reset混乱
        private void destroyPreviewInstances()
        {
            if (previewInstance != null)
            {
                Object.DestroyImmediate(previewInstance);
            }

            previewInstance = null;
        }
        
        // 验证赋值avatar
        private void verifyAvatarPrefab()
        {
            if (defaultAvatar == null)
            {
                // setupDefaultAvatar(); TODO:从游戏中读取avatar
                defaultAvatar = EditorGUIUtility.Load("Avatar/DefaultAvatar.fbx") as GameObject;
            }
        }
        
        // 针对实例化avatar
        private void instantiateAvatar(GameObject res)
        {
            previewInstance = EditorInternalHacker.EditorUtility.InstantiateForAnimatorPreview(res);
            previewUtility.AddSingleGO(previewInstance);
            setupSkin(res);
            setCustomAvatarMaterial(previewInstance);
        }
        
        // 设置皮肤
        private void setupSkin(GameObject res)
        {
            var skins = res.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var skin in skins)
            {
                skin.quality = SkinQuality.Bone4;
                //skin.forceMatrixRecalculationPerRender = true;
                skin.sharedMesh.RecalculateNormals();
                skin.sharedMesh.RecalculateTangents();
                skin.sharedMesh.RecalculateBounds();
            }
        }
        
        // 设置材质
        private void setCustomAvatarMaterial(GameObject res)
        {
            if (res == null) return;
            if (!res.name.Contains("DefaultAvatar")) return;// TODO:意义？
            var transforms = res.GetComponentsInChildren<Transform>().ToList();
            var skinnedMeshRenderer = transforms.Find(x => x.name.Contains("Unity_Body_Mesh")).GetComponent<SkinnedMeshRenderer>();
            var mats = skinnedMeshRenderer.sharedMaterials;
            mats[1] = Resources.Load<Material>("Styles/bodyMaterial");
            skinnedMeshRenderer.sharedMaterials = mats;
//            skinnedMeshRenderer.sharedMesh.RecalculateNormals();
//            skinnedMeshRenderer.sharedMesh.RecalculateNormals();
//            skinnedMeshRenderer.sharedMesh.RecalculateBounds();
        }

        #endregion

        #region 创建视图背景板
        
        private Mesh floorPlane;
        private Material floorMaterial;
        private Texture2D floorTexture;
        private const float kFloorScale = 35f;
        
        private void createFloor()
        {
            if (floorPlane == null)
            {
                floorPlane = Resources.GetBuiltinResource(typeof(Mesh), "New-Plane.fbx") as Mesh;
            }

            // 有的时候刷新会导致贴图没有了，所以每次刷新都重新创建一下
            floorTexture = MotionEditorUtility.Style.GridTextureUE4;

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

        #endregion
        
        #endregion
    }
}