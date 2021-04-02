using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [System.Serializable]
    public class AnimGraphAnimationClipScriptableObject : AnimGraphMotionScriptableObject
    {
        public AnimationClip animationClip;
        [HideInInspector] public float duration;
        public int HashCode
        {
            get
            {
                if (animationClip == null)
                    return 0;
                //var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(animationClip));
                //ulong heigh = Convert.ToUInt64(guid.Substring(0, 16), 16);
                //ulong low = Convert.ToUInt64(guid.Substring(16, 16), 16);
                //MarkleTree mt = new MarkleTree(2);
                //mt.Add(heigh.GetHashCode());
                //mt.Add(low.GetHashCode());
                //var hash = mt.HashCode;
                //mt.Dispose();
                //return hash;
                return AssetHelper.AssetPathToGUID(AssetDatabase.GetAssetPath(animationClip));
            }
        }

        public int ClipNameHashCode
        {
            get
            {
                if (animationClip == null)
                    return 0;
                
                return animationClip.name.HashCode();
            }
        }
        
        public override float GetDuration()
        {
            if (animationClip == null) return 0;
            return animationClip.length;
        }

        public override void PreDeserialization(BinaryWriter writer)
        {
            AnimationClipAsset._Data data = new AnimationClipAsset._Data();
            motionOffset = (int)writer.BaseStream.Position;
            data.motionType = MotionType.AnimationClip;
            data.isLooping = isLooping;
            data.avgDuration = GetDuration().ToUintTime();
            data.asset = HashCode;
            data.nameHash = ClipNameHashCode;
            
            writer.WriteStruct(ref data);
        }
    }
}