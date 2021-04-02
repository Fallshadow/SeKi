using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.Animations;
using UnityEngine;

namespace Framework.AnimGraphs
{
    [System.Serializable]
    public class AnimGraphChildMotionScriptableObject : IPreDeserialization,IDeserialization
    {
        public Vector2 position;
        public AnimGraphMotionScriptableObject motion;
        public float timeScale;

        public byte[] Deserialization()
        {
            byte[] bytes = new byte[ChildMotionAsset.SizeOf];
            MemoryStream stream = new MemoryStream(bytes);
            BinaryWriter bytesWriter = new BinaryWriter(stream);

            ChildMotionAsset._Data data = new ChildMotionAsset._Data();
            data.postion = position;
            data.timeScale = timeScale;
            if(motion == null)
            {
                data.motionOffset = -1;
            }
            else
            {
                data.motionOffset = motion.motionOffset;
            }
            
            bytesWriter.WriteStruct(ref data);
            
            return bytes;
        }

        //hack create from tree
        public byte[] Deserialization(ChildMotion childMotion, BlendTreeType blendType)
        {
            switch (blendType)
            {
                case BlendTreeType.Simple1D:
                    position = new Vector2(childMotion.threshold, 0);
                    break;
                case BlendTreeType.FreeformCartesian2D:
                    position = new Vector2(childMotion.position.x, childMotion.position.y);
                    break;
            }
            
            //position = childMotion.position;
            timeScale = childMotion.timeScale;
            
            return Deserialization();
        }
        
        public void PreDeserialization(BinaryWriter writer)
        {
            // 因为是数组对象，所以不自己写入内存，而只是调度它的引用对象的预处理
            motion.PreDeserialization(writer);
        }
    }
}