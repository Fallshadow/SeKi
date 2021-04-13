using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [Serializable]
    public abstract class AnimGraphMotionScriptableObject : ScriptableObject, IPreDeserialization
    {
        public bool isLooping;
        [NonSerialized]
        public int motionOffset;

        public abstract void PreDeserialization(BinaryWriter writer);

        public abstract float GetDuration();
    }
}