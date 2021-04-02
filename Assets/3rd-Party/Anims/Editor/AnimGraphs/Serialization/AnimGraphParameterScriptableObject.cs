using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [System.Serializable]
    public class AnimGraphParameterScriptableObject : ScriptableObject, IDeserialization
    {
        [System.Serializable]
        public class AnimGraphControllerParameter : IDeserialization
        {
            public string parameterName;
            public int parameterNameHash;
            public ParameterType parameterType;

            public byte[] Deserialization()
            {
                byte[] bytes = new byte[ParameterAsset.SizeOf];
                MemoryStream stream = new MemoryStream(bytes);
                BinaryWriter bytesWriter = new BinaryWriter(stream);

                ParameterAsset._Data data = new ParameterAsset._Data();
                data.nameHash = parameterNameHash;
                data.parameterType = parameterType;

                bytesWriter.WriteStruct(ref data);
                return bytes;
            }
        }

        public AnimGraphControllerParameter[] parameters;

        public int GetParameterIndex(string name)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].parameterName == name)
                    return i;
            }
            Debug.LogError($"nameHash = {name} 未找到!");
            return -1;
        }

        public byte[] Deserialization()
        {
            byte[] bytes = new byte[UnsafeUtility.SizeOf<int>() + parameters.Length* ParameterAsset.SizeOf];
            MemoryStream stream = new MemoryStream(bytes);
            BinaryWriter bytesWriter = new BinaryWriter(stream);

            bytesWriter.Write(parameters.Length);
            for (int i = 0; i < parameters.Length; i++)
            {
                bytesWriter.Write(parameters[i].Deserialization());
            }

            return bytes;
        }
    }
}