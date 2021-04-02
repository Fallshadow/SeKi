using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

namespace Framework.AnimGraphs
{

    public partial class AnimGraphSerializationWindow
    {
        private AnimGraphBlendTreeScriptableObject DeserializeBlendTree(string blendTreeName, BlendTree blendTree, int deep = 0)
        {
            AnimGraphBlendTreeScriptableObject blendTreeScriptable = null;
            string writeFilePath = Path.Combine(AssetDatabase.GetAssetPath(root), "Controller");
            string path = Path.Combine(writeFilePath, "BlendTree", blendTreeName + ".asset");
            blendTreeScriptable = AssetDatabase.LoadAssetAtPath<AnimGraphBlendTreeScriptableObject>(path);
            if(blendTreeScriptable == null)
            {
                blendTreeScriptable = ScriptableObject.CreateInstance<AnimGraphBlendTreeScriptableObject>();
                Directory.CreateDirectory(Path.Combine(writeFilePath, "BlendTree"));
                AssetDatabase.CreateAsset(blendTreeScriptable, path);
            }

            blendTreeScriptable.isLooping = blendTree.isLooping;
            blendTreeScriptable.blendParameterX = blendTree.blendParameter;
            blendTreeScriptable.blendParameterY = blendTree.blendParameterY;
            blendTreeScriptable.blendType = (BlendTreeType)(int)blendTree.blendType;
            
            if (blendTree != null)
            {
                if (!Directory.Exists(Path.Combine(writeFilePath, "BlendTree", "Data")))
                {
                    Directory.CreateDirectory(Path.Combine(writeFilePath, "BlendTree", "Data"));
                }

                BlendTree btCopy = UnityEngine.Object.Instantiate(blendTree);
                var btPath = Path.Combine(writeFilePath, "BlendTree", "Data", blendTreeName + ".asset");
                AssetDatabase.CreateAsset(btCopy, btPath);
                blendTreeScriptable.tree = AssetDatabase.LoadAssetAtPath<BlendTree>(btPath);
            }
            
            blendTreeScriptable.childMotions = new List<AnimGraphChildMotionScriptableObject>();
            int count = blendTree.children.Length;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    string name = string.Format("{0}_{1}_{2}", blendTreeName, i, deep);
                    DeserializeChildMotion(name, blendTree.children[i], blendTreeScriptable, blendTreeScriptable.blendType, deep);
                }
            }

            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    //string name = string.Format("{0}_{1}_{2}_blendTree", blendTreeName, i, deep);

                    if (!(blendTreeScriptable.tree.children[i].motion is BlendTree)) continue;
                    
                    string childName = $"{blendTreeName}_{i}_{deep}.asset";
                        
                    var btp = Path.Combine(writeFilePath, "BlendTree", "Data", childName);
                        
                    //btp.Replace('\\', '/');
                    //Debug.Log(btp);
                        
                    var childTree = AssetDatabase.LoadAssetAtPath<BlendTree>(btp);

                    var childs = blendTreeScriptable.tree.children;
                    childs[i].motion = childTree;
                    blendTreeScriptable.tree.children = childs;
                }
            }


            EditorUtility.SetDirty(blendTreeScriptable);
            return blendTreeScriptable;
        }
    }
}
