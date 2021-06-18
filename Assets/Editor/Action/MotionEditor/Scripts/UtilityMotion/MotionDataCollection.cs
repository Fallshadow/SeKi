using System.Collections.Generic;
using ASeKi.action;
using constants;
using Framework.AnimGraphs;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace ASeKi.action
{
    // 动作数据收集者
    public static class MotionDataCollection
    {
        // Init Motion Data List
        public static List<MotionData> GetMotionByRoleType(ActionRoleType type, bool withNormal)
        {
            List<MotionData> datas = new List<MotionData>();
            
            // MotionDataListSO motionDataSetting = Resources.Load<MotionDataListSO>(ResourcesPathSetting.ScriptableObject + ActionSettingIndexMapping.PLAYER_MOTION_DATA_LIST_PATH);
            
            // bool ret = motionDataSetting.GetMotionData(type, withNormal, ref datas);

            return datas;
        }

        public const string Path = "Assets/Data/CharacterMotions";

        public static Dictionary<string, AnimationClip> GetAllClips()
        {
            Dictionary<string, AnimationClip> result = new Dictionary<string, AnimationClip>();
            
            var statePaths = AssetDatabase.FindAssets($"t:{typeof(AnimGraphStateScriptableObject)}", new[] { Path });

            string startKey = null;

            foreach (var statePath in statePaths)
            {
                var state = AssetDatabase.LoadAssetAtPath<AnimGraphStateScriptableObject>(AssetDatabase.GUIDToAssetPath(statePath));
                
                if (state.motion is AnimGraphBlendTreeScriptableObject blendTree)
                {
                    getBlendTreeClip(blendTree.tree, ref result);
                }

                if (state.motion is AnimGraphAnimationClipScriptableObject clip)
                {
                    if (clip.animationClip == null)
                    {
                        Debug.Log($" Clip Null StateName:{state.stateName}");
                        continue;
                    }
                    
                    if (!result.ContainsKey(clip.name))
                    {
                        result.Add(clip.animationClip.name, clip.animationClip);
                    }
                    else
                    {
                        //Debug.Log($"Current state {state.stateName} With Same clip name{clip.name}");
                    }
                }
            }

            return result;
        }

        static void getBlendTreeClip(BlendTree tree, ref Dictionary<string, AnimationClip> collections)
        {
            if (tree.children == null || tree.children.Length == 0)
            {
                return;
            }
            
            foreach (var childMotion in tree.children)
            {
                if (childMotion.motion is AnimationClip clip)
                {
                    if (!collections.ContainsKey(clip.name))
                    {
                        collections.Add(clip.name, clip);
                    }
                    else
                    {
                        //Debug.Log($"Current Blend Tree {tree.name} With Same clip name{clip.name}");
                    }
                }

                if (childMotion.motion is BlendTree childTree)
                {
                    getBlendTreeClip(childTree, ref collections);
                }
            }
        }

        public static AnimationClip[] GetClips(this BlendTree tree)
        {
            List<AnimationClip> ret = new List<AnimationClip>();

            GetClips(tree, ref ret);

            return ret.ToArray();
        }
        
        public static void GetClips(BlendTree tree, ref List<AnimationClip> ret)
        {
            if (tree.children == null || tree.children.Length == 0)
            {
                return;
            }
            
            foreach (var childMotion in tree.children)
            {
                if (childMotion.motion is AnimationClip clip)
                {
                    ret.Add(clip);
                }

                if (childMotion.motion is BlendTree childTree)
                {
                    GetClips(childTree, ref ret);
                }
            }
        }
        
        public static Dictionary<string, UnityEditor.Animations.BlendTree> GetBlendtrees(ActionRoleType type, bool withNormal)
        {
            Dictionary<string, UnityEditor.Animations.BlendTree> result = new Dictionary<string, UnityEditor.Animations.BlendTree>();
            
            var statePaths = AssetDatabase.FindAssets($"t:{typeof(AnimGraphStateScriptableObject)}", new[] { Path });

            string startKey = null;
                
            switch (type)
            {
                case ActionRoleType.ART_PLAYER_SWORD:
                case ActionRoleType.ART_PLAYER_HAMMER:
                case ActionRoleType.ART_PLAYER_DUALBLADE:
                case ActionRoleType.ART_PLAYER_BOWGUN:
                {
                    startKey = $"{(int)type + 1}_";
                }
                    break;
                case ActionRoleType.ART_PLAYER_SPEAR:
                {
                    startKey = "5_";
                }
                    break;
                case ActionRoleType.ART_PLAYER_NONE:
                case ActionRoleType.ART_PLAYER_NORMAL:
                {
                    startKey = "0_";
                }
                    break;
                default:
                    debug.PrintSystem.Log($"Unknown Type {type}");
                    break;
            }

            if (startKey == null)
            {
                return null;
            }
            
            foreach (var statePath in statePaths)
            {
                var state = AssetDatabase.LoadAssetAtPath<AnimGraphStateScriptableObject>(AssetDatabase.GUIDToAssetPath(statePath));

                bool checkResult = (withNormal && state.name.StartsWith("0_")) || state.name.StartsWith(startKey);

                if (!checkResult)
                {
                    continue;
                }
                
                if (state.motion is AnimGraphBlendTreeScriptableObject blendTree)
                {
                    result.Add(state.name, blendTree.tree);
                    continue;
                }

                // if (state.motion is AnimGraphAnimationClipScriptableObject clip)
                // {
                //     result.Add(state.name, clip.animationClip);
                //     continue;
                // }
            }

            return result;
        }
    }
}
 
