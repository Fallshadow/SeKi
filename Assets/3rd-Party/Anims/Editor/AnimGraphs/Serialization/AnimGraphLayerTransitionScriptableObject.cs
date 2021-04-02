using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [System.Serializable]
    public class AnimGraphLayerTransitionScriptableObject : IDeserialization
    {
        public float duration;
        public LayerTransitionType layerTransitionType;


        public byte[] Deserialization()
        {
            byte[] bytes = new byte[LayerTransitionAsset.SizeOf];
            MemoryStream stream = new MemoryStream(bytes);
            BinaryWriter bytesWriter = new BinaryWriter(stream);

            LayerTransitionAsset._Data data = new LayerTransitionAsset._Data();
            data.duration = duration.ToUintTime();
            data.type = layerTransitionType;

            bytesWriter.WriteStruct(ref data);
            return bytes;
        }
    }
}