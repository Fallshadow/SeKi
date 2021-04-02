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
        private AnimGraphAnimationClipScriptableObject DeserializeAnimationClip(AnimationClip animationClip)
        {
            string writeFilePath = Path.Combine(AssetDatabase.GetAssetPath(root), "Controller");
            string path = Path.Combine(writeFilePath, "Animations", animationClip.name + ".asset");
            AnimGraphAnimationClipScriptableObject animationClipScriptable = null;
            animationClipScriptable = AssetDatabase.LoadAssetAtPath<AnimGraphAnimationClipScriptableObject>(path);
            if(animationClipScriptable == null)
            {
                animationClipScriptable = ScriptableObject.CreateInstance<AnimGraphAnimationClipScriptableObject>();
                Directory.CreateDirectory(Path.Combine(writeFilePath, "Animations"));
                AssetDatabase.CreateAsset(animationClipScriptable, path);
            }

            overrideMapping.TryGetValue(AssetDatabase.GetAssetPath(animationClip), out AnimationClip overrideClip);

            if(overrideClip != null)
            {
                animationClipScriptable.animationClip = overrideClip;
            }
            else
            {
                animationClipScriptable.animationClip = animationClip;
            }

            animationClipScriptable.isLooping = animationClip.isLooping;
            animationClipScriptable.duration = animationClip.length;
            EditorUtility.SetDirty(animationClipScriptable);
            return animationClipScriptable;
        }
    }
}
