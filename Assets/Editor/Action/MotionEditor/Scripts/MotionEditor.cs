using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace ASeKi.action
{
    public class MotionEditor : EditorWindow
    {
        public static EditorWindow motionEditorWindows = null;
    
        private const string RES_ROOT_PATH = "Assets/Editor/Action/MotionEditor/Resources/";

        [MenuItem("SeKi/Action/Open Motion Editor #&m", false, 1500000)]
        public static void OpenWindow()
        {
            motionEditorWindows = GetWindow<MotionEditor>();
            Texture icon = AssetDatabase.LoadAssetAtPath<Texture>($"{RES_ROOT_PATH}/Icon/motion-icon.png");
            motionEditorWindows.titleContent = new GUIContent("Motion Editor", icon);
        }

        public void OnEnable()
        {
            // TODO:AssetDatabase.LoadAssetAtPath<Texture> 拿到编辑器需要的图片
            // TODO:拿到编辑器需要的数据并进行初步操作（查空、按需排序等）
            
            VisualElement root = rootVisualElement;
            VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/Action/MotionEditor/Resources/MotionEditor.uxml");
            StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/Action/MotionEditor/Resources/MotionEditor.uss");
            VisualElement myTree = visualTree.CloneTree();
            root.Add(myTree);
            myTree.styleSheets.Add(styleSheet);
            
            VisualElement middleWindow = myTree.Q<VisualElement>("ME-Middle");
            initMiddleWindow(middleWindow);
        }

        private void initMiddleWindow(VisualElement visualElement)
        {
            MiddleWindow middleWindow = new MiddleWindow(visualElement,this);
            middleWindow.OnEnable();
        }
    }
}
