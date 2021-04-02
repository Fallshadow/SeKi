using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Framework.AnimGraphs
{

    public unsafe class DeserializationWindow : EditorWindow
    {
        [MenuItem("Rex/AnimGraphs/Deserialization Tool",     false,3300000)]
        public static void ShowWindow()
        {
            GetWindow<DeserializationWindow>(false, "序列化生成");
        }

        private Vector2 scrollPos;
        private DefaultAsset writeFile;
        private DefaultAsset scriptableFile;
        private string fileName;

        public static AnimGraphParameterScriptableObject parameterController;
        private AnimGraphLayersScriptableObject layers; 

        private Dictionary<int, AnimGraphNodeScriptableObject> nodeMapping = new Dictionary<int, AnimGraphNodeScriptableObject>();

        private MemoryStream sheetStream;
        private BinaryWriter sheetWriter;
        private MemoryStream dynStream;
        private BinaryWriter dynWriter;
        private AnimGraphHead sheetHead;

        public void DoWithCode(string fileName, DefaultAsset writeFile, DefaultAsset scriptableFile)
        {
            this.fileName = fileName;
            this.writeFile = writeFile;
            this.scriptableFile = scriptableFile;

            doSerializeToByte();
        }
        
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            {
                EditorGUI.BeginChangeCheck();
                {
                    fileName = EditorGUILayout.TextField("名称", fileName);
                    writeFile = EditorGUILayout.ObjectField("输出目录：", writeFile, typeof(DefaultAsset), false) as DefaultAsset;
                    scriptableFile = EditorGUILayout.ObjectField("序列化目录：", scriptableFile, typeof(DefaultAsset), false) as DefaultAsset;
                }
                if (EditorGUI.EndChangeCheck())
                {
                }
                if (scriptableFile == null || writeFile == null)
                {
                    EditorGUILayout.EndScrollView();
                    return;
                }

                if (GUILayout.Button("序列化输出"))
                {
                    doSerializeToByte();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        void doSerializeToByte()
        {
            PreLoadAnimGraphNode();
            PreLoadAnimGraphParameter();
            PreLoadAnimGraphLayer();

            AssetMapping assetMapping = ScriptableObject.CreateInstance<AssetMapping>();
            var clips = AssetDatabase.FindAssets("t:AnimGraphAnimationClipScriptableObject", new string[] { AssetDatabase.GetAssetPath(scriptableFile) });
            assetMapping.clips = new AssetMapping.AnimationClipAsset[clips.Length];
            for (int i = 0; i < clips.Length; i++)
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimGraphAnimationClipScriptableObject>(AssetDatabase.GUIDToAssetPath(clips[i]));
                assetMapping.clips[i].clip = clip.animationClip;
                assetMapping.clips[i].hash = clip.HashCode;
            }

            assetMapping.avatars = new AssetMapping.AvatarMaskAsset[layers.layers.Length];
            for (int i = 0; i < layers.layers.Length; i++)
            {
                assetMapping.avatars[i].avatarMask = layers.layers[i].avatarMask;
                assetMapping.avatars[i].hash = layers.layers[i].HashCode;
            }

            string path = Path.Combine(AssetDatabase.GetAssetPath(writeFile), fileName + ".asset");
            AssetDatabase.CreateAsset(assetMapping, path);
            EditorUtility.SetDirty(assetMapping);

            InitStreamWork();
            InitHead();
            WriteStateSheet();

            // 写入Parameter数据
            sheetHead.parametersOffset = (int)sheetWriter.BaseStream.Position;
            sheetWriter.Write(parameterController.Deserialization());

            sheetHead.layersOffset = (int)sheetWriter.BaseStream.Position;
            sheetWriter.Write(layers.Deserialization());

            sheetHead.dynamicOffset = (int)sheetWriter.BaseStream.Position;
            sheetWriter.Write(dynStream.ToArray());

            // 重新写入头文件
            sheetWriter.BaseStream.Position = 0;
            sheetWriter.WriteStruct(ref sheetHead);

            var filePath = Path.Combine(AssetDatabase.GetAssetPath(writeFile),fileName+".bytes");
            File.Delete(filePath);
            FileStream stream = File.Create(filePath);
            var bytes = sheetStream.ToArray();
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();
            stream.Close();

            //fixed (byte* p = sheetStream.ToArray())
            //{
            //    AnimGraphController controller = new AnimGraphController(p);

            //    var state = controller.LoadAnimGraphStateAsset("Attack4-2".HashCode());
            //    if (!state.isNull)
            //    {
            //        Debug.Log(state.stateName);
            //        Debug.Log("Attack4-2".HashCode());
            //    }
            //    Debug.Log(controller.GetParameterIndex("TurnType"));
            //    controller.Layers();
            //}

            AssetDatabase.Refresh();
        }
        
        private void WriteStateSheet()
        {
            var idOffsetMap = new Dictionary<int, int>();

            // 预处理
            foreach (var item in nodeMapping)
            {
                item.Value.PreDeserialization(dynWriter);
            }

            // 写入sheet列表
            foreach (var item in nodeMapping)
            {
                var bytes = item.Value.Deserialization();
                var position = (int)sheetWriter.BaseStream.Position;
                sheetWriter.Write(bytes);
                idOffsetMap.Add(item.Key, position);
            }

            // 写mapping
            sheetHead.nodeMappingOffset = (int)sheetWriter.BaseStream.Position;

            int size = (int)System.Type.GetType("System.Collections.HashHelpers").GetMethod("GetPrime", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).Invoke(null, new object[] { idOffsetMap.Count });
            var map = new AnimGraphHashEntry[size];
            var hasValue = new bool[size];
            List<KeyValuePair<int,int>> extraValues = new List<KeyValuePair<int, int>>();
            
            for (int i = 0; i < size; i++)
            {
                map[i].value = -1;
                map[i].next = -1;
            }

            foreach (var item in idOffsetMap)
            {
                var id = item.Key;
                var value = item.Value;
                int slotIndex = (int)(((uint)id) % size);

                if (!hasValue[slotIndex])
                {
                    hasValue[slotIndex] = true;
                    map[slotIndex] = new AnimGraphHashEntry { id = id, value = value, next = -1 };
                }
                else
                {
                    extraValues.Add(item);
                }
            }

            int maxDepth = 1;
            for (int itemIndex = extraValues.Count - 1; itemIndex >= 0; itemIndex--) 
            {
                var item = extraValues[itemIndex];
                var id = item.Key;
                var value = item.Value;
                int slotIndex = (int)(((uint)id) % size);

                int depth = 1;
                while (map[slotIndex].next >= 0)
                {
                    depth++;
                    slotIndex = map[slotIndex].next;
                }
                if (depth > maxDepth) maxDepth = depth;

                int emptySlotIndex = -1;
                for (int j = 0; j < hasValue.Length; j++)
                {
                    if (!hasValue[j])
                    {
                        emptySlotIndex = j;
                        break;
                    }
                }
                if (emptySlotIndex == -1)
                    throw new System.Exception("没有足够的空间保存头部字典");

                hasValue[emptySlotIndex] = true;
                map[emptySlotIndex] = new AnimGraphHashEntry { id = id, value = value, next = -1 };
                var parent = map[slotIndex];
                parent.next = emptySlotIndex;
                map[slotIndex] = parent;
            }

            sheetWriter.Write(size);
            for (int i = 0; i < map.Length; i++)
            {
                var entry = map[i];
                sheetWriter.Write(entry.id);
                sheetWriter.Write(entry.value);
                sheetWriter.Write(entry.next);
            }
        }

        private void InitStreamWork()
        {
            sheetStream = new MemoryStream(2048);
            sheetWriter = new BinaryWriter(sheetStream);

            dynStream = new MemoryStream(2048);
            dynWriter = new BinaryWriter(dynStream);
        }

        private void InitHead()
        {
            sheetHead = default;
            sheetWriter.WriteStruct(ref sheetHead);
        }

        private void PreLoadAnimGraphNode()
        {
            nodeMapping.Clear();
            var scriptableFilePath = AssetDatabase.GetAssetPath(scriptableFile);
            var states = AssetDatabase.FindAssets("t:AnimGraphNodeScriptableObject", new string[] { scriptableFilePath });
            Debug.Log($"AnimGraphNode数量 {states.Length}");

            for (int i = 0; i < states.Length; i++)
            {
                var state = AssetDatabase.LoadAssetAtPath<AnimGraphNodeScriptableObject>(AssetDatabase.GUIDToAssetPath(states[i]));
                if (nodeMapping.ContainsKey(state.nodeName.HashCode()))
                {
                    Debug.LogError($"出现重复的状态名 {state.nodeName} 被抛弃");
                    continue;
                }
                nodeMapping.Add(state.nodeName.HashCode(), state);
            }
        }

        private void PreLoadAnimGraphParameter()
        {
            var parameter = AssetDatabase.FindAssets("t:AnimGraphParameterScriptableObject", new string[] { AssetDatabase.GetAssetPath(scriptableFile) });
            Debug.Log($"AnimGraphParameter数量 {parameter.Length}");
            if (parameter.Length > 0)
            {
                parameterController = AssetDatabase.LoadAssetAtPath<AnimGraphParameterScriptableObject>(AssetDatabase.GUIDToAssetPath(parameter[0]));
                AnimGraphBlendTreeScriptableObject.GetParameterIndex = parameterController.GetParameterIndex;
            }
        }

        private void PreLoadAnimGraphLayer()
        {
            var layer = AssetDatabase.FindAssets("t:AnimGraphLayersScriptableObject", new string[] { AssetDatabase.GetAssetPath(scriptableFile) });
            Debug.Log($"AnimGraphLayersScriptableObject数量 {layer.Length}");
            if (layer.Length > 0)
            {
                layers = AssetDatabase.LoadAssetAtPath<AnimGraphLayersScriptableObject>(AssetDatabase.GUIDToAssetPath(layer[0]));
            }
        }
    }
}