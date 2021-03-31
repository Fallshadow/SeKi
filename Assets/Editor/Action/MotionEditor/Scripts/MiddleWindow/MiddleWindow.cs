using System.Collections;
using System.Collections.Generic;
using ASeKi;
using UnityEngine;
using UnityEngine.UIElements;

namespace ASeKi.action
{
    public class MiddleWindow : IElementWindow
    {
        private VisualElement parent = null;
        private MotionEditor meRoot = null;
        
        public MiddleWindow(VisualElement visualElement,MotionEditor motionEditor)
        {
            parent = visualElement;
            meRoot = motionEditor;
        }
        
        public void OnEnable()
        {
            // 初始化各个内部控件
            IMGUIContainer imguiPreview = parent.Q<IMGUIContainer>("IMGUIContainer-Preview");
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
    }
}
