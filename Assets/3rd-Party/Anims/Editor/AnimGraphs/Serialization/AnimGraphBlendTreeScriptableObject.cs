using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [System.Serializable]
    public class AnimGraphBlendTreeScriptableObject : AnimGraphMotionScriptableObject
    {
        public static Func<string, int> GetParameterIndex;

        public BlendTreeType blendType;
        public string blendParameterX;
        public string blendParameterY;

        public List<AnimGraphChildMotionScriptableObject> childMotions;
        [NonSerialized]
        public int childMotionsOffset;

        public BlendTree tree;
        
        public int HashCode
        {
            get
            {
                //var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
                //ulong heigh = Convert.ToUInt64(guid.Substring(0, 16), 16);
                //ulong low = Convert.ToUInt64(guid.Substring(16, 16), 16);
                //MarkleTree mt = new MarkleTree(2);
                //mt.Add(heigh.GetHashCode());
                //mt.Add(low.GetHashCode());
                //var hash = mt.HashCode;
                //mt.Dispose();
                //return hash;
                return AssetHelper.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
            }
        }

        public override float GetDuration()
        {
            int count = 0;
            float avgDuration = 0;
            if (childMotions == null || childMotions.Count == 0) {
                return avgDuration;
            }
            else
            {
                for (int i = 0; i < childMotions.Count; i++)
                {
                    var childMotion = childMotions[i];
                    if (childMotion.motion == null)
                    {
                        continue;
                    }
                    avgDuration += childMotion.motion.GetDuration();
                    count++;
                }
            }
            return avgDuration / count;
        }

        public override void PreDeserialization(BinaryWriter writer)
        {
            //create from tree
            //hack
            isLooping = tree.isLooping;
            blendType = (BlendTreeType)tree.blendType;
            blendParameterX = tree.blendParameter;
            blendParameterY = tree.blendParameterY;
            
            if(childMotions == null || childMotions.Count == 0)
            {
                childMotionsOffset = -1;
            }
            else
            {
                // 预处理ChildMotion数组
                for (int i = 0; i < childMotions.Count; i++)
                {
                    var childMotion = childMotions[i];
                    if (childMotion.motion == null)
                    {
                        Debug.LogError($"资源引用丢失!");
                        continue;
                    }
                    childMotion.PreDeserialization(writer);
                }

                // 生成childMotion数组
                childMotionsOffset = (int)writer.BaseStream.Position;
                writer.Write(childMotions.Count);
                for (int i = 0; i < childMotions.Count; i++)
                {
                    var motion = childMotions[i];
                    writer.Write(motion.Deserialization(tree.children[i], blendType));
                }
            }

            // 生成当前BlendTree
            BlendTreeAsset._Data data = new BlendTreeAsset._Data();
            motionOffset = (int)writer.BaseStream.Position;

            data.motionType = MotionType.BlendTree;
            data.isLooping = isLooping;
            //data.avgDuration = GetDuration().ToUintTime();
            data.avgDuration = tree.averageDuration.ToUintTime();
            
            data.blendType = blendType;

            switch (blendType)
            {
                case BlendTreeType.Simple1D:
                    data.blendParameterX = (sbyte)GetParameterIndex.Invoke(blendParameterX);
                    data.blendParameterY = -1;
                    break;
                case BlendTreeType.FreeformCartesian2D:
                    data.blendParameterX = (sbyte)GetParameterIndex.Invoke(blendParameterX);
                    data.blendParameterY = (sbyte)GetParameterIndex.Invoke(blendParameterY);
                    break;
                default:
                    throw new System.Exception($"未实现分支逻辑 {blendType}");
            }
            data.childMotionsOffset = childMotionsOffset;

            writer.WriteStruct(ref data);


            
        }

    }
}