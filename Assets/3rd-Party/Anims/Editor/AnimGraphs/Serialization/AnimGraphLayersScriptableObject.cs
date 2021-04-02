using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [System.Serializable]
    public class AnimGraphLayersScriptableObject : ScriptableObject, IDeserialization
    {
        [System.Serializable]
        public class AnimGraphLayerScriptableObject : IDeserialization
        {
            public int layer;
            public AvatarMask avatarMask;
            public AnimatorLayerBlendingMode blendingMode;
            public string layerName;

            public int HashCode
            {
                get
                {
                    if (avatarMask == null)
                        return 0;
                    //var guid = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(avatarMask));
                    //ulong heigh = Convert.ToUInt64(guid.Substring(0, 16), 16);
                    //ulong low = Convert.ToUInt64(guid.Substring(16, 16), 16);
                    //MarkleTree mt = new MarkleTree(2);
                    //mt.Add(heigh.GetHashCode());
                    //mt.Add(low.GetHashCode());
                    //var hash = mt.HashCode;
                    //mt.Dispose();
                    //return hash;
                    return AssetHelper.AssetPathToGUID(AssetDatabase.GetAssetPath(avatarMask));
                }
            }

            public byte[] Deserialization()
            {
                byte[] bytes = new byte[LayerAsset.SizeOf];
                MemoryStream stream = new MemoryStream(bytes);
                BinaryWriter bytesWriter = new BinaryWriter(stream);

                LayerAsset._Data data = new LayerAsset._Data();
                data.layer = layer;
                data.avatarMaskAsset = HashCode;
                data.blendingMode = (LayerBlendingMode)blendingMode;
                data.layerHash = layerName.HashCode();

                bytesWriter.WriteStruct(ref data);
                return bytes;
            }
        }

        public AnimGraphLayerScriptableObject[] layers;

        public byte[] Deserialization()
        {
            byte[] bytes = new byte[UnsafeUtility.SizeOf<int>() + layers.Length * LayerAsset.SizeOf];
            MemoryStream stream = new MemoryStream(bytes);
            BinaryWriter bytesWriter = new BinaryWriter(stream);

            bytesWriter.Write(layers.Length);
            for (int i = 0; i < layers.Length; i++)
            {
                bytesWriter.Write(layers[i].Deserialization());
            }

            return bytes;
        }
    }
}