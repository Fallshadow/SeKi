using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Mathematics;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [System.Serializable]
    public class AnimGraphStateScriptableObject : ScriptableObject , IPreDeserialization, IDeserialization
    {
        public int layer;
        public string stateName;
        public bool writeDefaultValues;
        public bool iKOnFeet;
        public float speed = 1f;
        public List<AnimGraphTransitionScriptableObject> transitions;
        [NonSerialized]
        public int transitionsOffset;
        public AnimGraphTransitionScriptableObject anyTransition;
        [NonSerialized]
        public int anyTransitionsOffset;
        public AnimGraphMotionScriptableObject motion;

        public AnimGraphLayerTransitionScriptableObject fadeInLayerTransition;
        public AnimGraphLayerTransitionScriptableObject fadeOutLayerTransition;
        [NonSerialized]
        public int fadeInLayerTtransOffset;
        [NonSerialized]
        public int fadeOutLayerTtransOffset;


        [NonSerialized]
        public int stateOffset;

        public byte[] Deserialization()
        {
            byte[] bytes = new byte[MotionStateAsset.SizeOf];
            MemoryStream stream = new MemoryStream(bytes);
            BinaryWriter bytesWriter = new BinaryWriter(stream);

            MotionStateAsset._Data data = new MotionStateAsset._Data();
            data.layerIndex = layer;
            data.stateName = stateName.HashCode();
            data.writeDefaultValues = writeDefaultValues;
            data.iKOnFeet = iKOnFeet;
            data.transitionOffset = transitionsOffset;
            data.anyTransitionOffset = anyTransitionsOffset;
            data.speed = speed;
            
            if (motion == null)
            {
                Debug.LogError($"资源引用丢失 {stateName}");
                data.motionOffset = -1;
            }
            else
            {
                data.motionOffset = motion.motionOffset;
            }

            data.fadeInLayerTtransOffset = fadeInLayerTtransOffset;
            data.fadeOutLayerTtransOffset = fadeOutLayerTtransOffset;

            bytesWriter.WriteStruct(ref data);
            return bytes;
        }

        public void PreDeserialization(BinaryWriter writer)
        {
            // 生成transitions数组
            if(transitions == null || transitions.Count == 0)
            {
                transitionsOffset = -1;
            }
            else
            {
                transitionsOffset = (int)writer.BaseStream.Position;
                writer.Write(transitions.Count);
                for (int i = 0; i < transitions.Count; i++)
                {
                    var trans = transitions[i];
                    writer.Write(trans.Deserialization());
                }
            }

            if(anyTransition == null)
            {
                anyTransitionsOffset = -1;
            }
            else
            {
                anyTransitionsOffset = (int)writer.BaseStream.Position;
                writer.Write(anyTransition.Deserialization());
            }


            if (fadeInLayerTransition == null)
                fadeInLayerTtransOffset = -1;
            else
            {
                fadeInLayerTtransOffset = (int)writer.BaseStream.Position;
                writer.Write(fadeInLayerTransition.Deserialization());
            }

            if (fadeOutLayerTransition == null)
                fadeOutLayerTtransOffset = -1;
            else
            {
                fadeOutLayerTtransOffset = (int)writer.BaseStream.Position;
                writer.Write(fadeOutLayerTransition.Deserialization());
            }

            if (motion != null)
            {
                motion.PreDeserialization(writer);
            }

            stateOffset = (int)writer.BaseStream.Position;
            writer.Write(Deserialization());
        }
    }
}