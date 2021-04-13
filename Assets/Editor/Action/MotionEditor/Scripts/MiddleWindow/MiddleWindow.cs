using System.Collections;
using System.Collections.Generic;
using ASeKi;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ASeKi.action
{
    public class MiddleWindow : IElementWindow
    {
        private VisualElement meRoot = null;
        private MotionEditor parent = null;
        
        private DefaultAsset curDataFolder;
        
        public MiddleWindow(VisualElement visualElement,MotionEditor motionEditor)
        {
            meRoot = visualElement;
            parent = motionEditor;
        }
        
        public void OnEnable()
        {
            // 初始化各个内部控件
            IMGUIContainer imguiPreview = meRoot.Q<IMGUIContainer>("IMGUIContainer-Preview");
            initImguiPreview(imguiPreview);
        }

        public void OnDisable()
        {
        
        }

        public void Update()
        {
        
        }

        public void OnGUI()
        {
        
        }

        #region MotionPreview 预览动作相关

        private MotionPreview motionPreviewHandle;
        
        private void initImguiPreview(IMGUIContainer previewIMGUI)
        {
            if (motionPreviewHandle == null)
            {
                motionPreviewHandle = new MotionPreview();
            }

            previewIMGUI.onGUIHandler = () => 
            { 
                GUIStyle background = "PreBackgroundSolid";
                motionPreviewHandle.OnPreviewGUI(previewIMGUI.contentRect, background);
                
            };
        }

        #endregion
        
        public DefaultAsset GetCurFolder()
        {
            return curDataFolder;
        }
    }
}
