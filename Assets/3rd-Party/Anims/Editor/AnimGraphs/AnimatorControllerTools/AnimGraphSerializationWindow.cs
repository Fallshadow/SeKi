using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

namespace Framework.AnimGraphs
{
    public partial class AnimGraphSerializationWindow : EditorWindow
    {
        [MenuItem("Rex/AnimGraphs/Serialization Animator Controller",     false,3710000)]
        public static void ShowWindow()
        {
            GetWindow<AnimGraphSerializationWindow>(false, "动画控制器序列化工具");
        }

        private Vector2 scrollPos;

        private AnimatorController controller;
        private AnimatorOverrideController overrideController;
        private DefaultAsset root;
        private Dictionary<string, AnimationClip> overrideMapping = new Dictionary<string, AnimationClip>();

        private HashSet<string> animatorStateHash = new HashSet<string>();

        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            {
                EditorGUI.BeginChangeCheck();
                {
                    controller =
                        EditorGUILayout.ObjectField("动画控制器：", controller, typeof(AnimatorController), false) as
                            AnimatorController;
                }
                if (EditorGUI.EndChangeCheck())
                {
                }

                if (controller == null)
                {
                    EditorGUILayout.EndScrollView();
                    return;
                }

                overrideController =
                    EditorGUILayout.ObjectField("Override动画控制器：", overrideController,
                        typeof(AnimatorOverrideController), false) as AnimatorOverrideController;

                root = EditorGUILayout.ObjectField("输出根目录：", root, typeof(DefaultAsset), false) as DefaultAsset;

                if (GUILayout.Button("开始转换"))
                {
                    createAssets();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        void createAssets()
        {
            try
            {
                AssetDatabase.StartAssetEditing();

                overrideMapping.Clear();
                animatorStateHash.Clear();
                anyTransitionMapping.Clear();
                layerFadeinCacheMap.Clear();

                // 获得数据
                if (overrideController != null)
                {
                    var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                    overrideController.GetOverrides(overrides);
                    foreach (var item in overrides)
                    {
                        var orige = AssetDatabase.GetAssetPath(item.Key);
                        overrideMapping.Add(orige, item.Value);
                    }
                }

                string writeFilePath = Path.Combine(AssetDatabase.GetAssetPath(root), "Controller");
                Directory.CreateDirectory(writeFilePath);

                Directory.CreateDirectory(Path.Combine(writeFilePath, "Parameters"));
                AnimatorControllerParameter[] parameters = controller.parameters;
                string path = Path.Combine(writeFilePath, "Parameters", "Parameters.asset");
                AnimGraphParameterScriptableObject parameterScriptable = CreateInstance<AnimGraphParameterScriptableObject>();

                int parameterCount = parameters.Length;
                
                parameterScriptable.parameters =
                    new AnimGraphParameterScriptableObject.AnimGraphControllerParameter[parameterCount];
                for (int i = 0; i < parameterCount; i++)
                {
                    parameterScriptable.parameters[i] =
                        new AnimGraphParameterScriptableObject.AnimGraphControllerParameter
                        {
                            parameterName = parameters[i].name,
                            parameterNameHash = parameters[i].nameHash,
                            parameterType = (ParameterType) parameters[i].type
                        };
                }

                AssetDatabase.CreateAsset(parameterScriptable, path);

                AnimGraphBlendTreeScriptableObject.GetParameterIndex = parameterScriptable.GetParameterIndex;

                AnimGraphLayersScriptableObject layersScriptable = CreateInstance<AnimGraphLayersScriptableObject>();

                int layerCount = controller.layers.Length;
                
                layersScriptable.layers =
                    new AnimGraphLayersScriptableObject.AnimGraphLayerScriptableObject[layerCount];
                for (int i = 0; i < layerCount; i++)
                {
                    layersScriptable.layers[i] = new AnimGraphLayersScriptableObject.AnimGraphLayerScriptableObject
                    {
                        avatarMask = controller.layers[i].avatarMask,
                        blendingMode = controller.layers[i].blendingMode,
                        layer = i,
                        layerName = controller.layers[i].name
                    };
                    DeserializeLayer(controller.layers[i], i);
                }

                Directory.CreateDirectory(Path.Combine(writeFilePath, "Layers"));
                string layersPath = Path.Combine(writeFilePath, "Layers", "Layers.asset");
                AssetDatabase.CreateAsset(layersScriptable, layersPath);

                EditorUtility.SetDirty(layersScriptable);
                EditorUtility.SetDirty(parameterScriptable);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();

                EditorUtility.DisplayProgressBar("进度提示", "正在施展【黑魔法】加速", 0.6f);

                AssetDatabase.SaveAssets();

                AssetDatabase.Refresh();

                EditorUtility.DisplayProgressBar("进度提示", "正在施展【黑魔法】加速", 0.8f);

                Deserialization();
                AssetDatabase.Refresh();

                EditorUtility.ClearProgressBar();
                EditorUtility.DisplayDialog("提示", "已完成！！！", "点赞");
            }
        }
    }
}