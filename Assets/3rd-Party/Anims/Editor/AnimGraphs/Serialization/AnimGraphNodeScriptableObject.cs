using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Framework.AnimGraphs
{
    public class AnimGraphNodeScriptableObject : ScriptableObject, IPreDeserialization, IDeserialization
    {
        public AnimGraphStateScriptableObject stateScriptable;
        public string nodeName;

        public byte[] Deserialization()
        {
            byte[] bytes = new byte[NodeAsset.SizeOf];
            MemoryStream stream = new MemoryStream(bytes);
            BinaryWriter bytesWriter = new BinaryWriter(stream);

            NodeAsset._Data data = new NodeAsset._Data();
            data.nodeName = nodeName.HashCode();
            if (stateScriptable == null)
            {
                Debug.LogError($"State资源丢失 {nodeName}");
                data.motionStateAsset = -1;
            }
            else
            {
                data.motionStateAsset = stateScriptable.stateOffset;
            }

            bytesWriter.WriteStruct(ref data);
            return bytes;
        }

        public void PreDeserialization(BinaryWriter writer)
        {
            if (stateScriptable != null)
            {
                stateScriptable.PreDeserialization(writer);
            }
        }
    }
}
