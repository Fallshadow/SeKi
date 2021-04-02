using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Framework.AnimGraphs
{
    [System.Serializable]
    public class AnimGraphTransitionScriptableObject : IDeserialization
    {
        public bool hasExitTime;
        public string destinationState;
        public float exitTime;
        public float duration;
        public float offset;

        public byte[] Deserialization()
        {
            //[FieldOffset(0)]
            //public readonly bool hasExitTime;
            //[FieldOffset(2)]
            //public readonly int destinationState;
            //[FieldOffset(6)]
            //public readonly uint exitTime;
            //[FieldOffset(10)]
            //public readonly uint duration;
            //[FieldOffset(14)]
            //public readonly int offset;

            byte[] bytes = new byte[TransitionAsset.SizeOf];
            MemoryStream stream = new MemoryStream(bytes);
            BinaryWriter bytesWriter = new BinaryWriter(stream);

            TransitionAsset._Data data = new TransitionAsset._Data();

            data.hasExitTime = hasExitTime;
            data.destinationState = destinationState.HashCode();
            data.exitTime = exitTime.ToUintTime();
            data.duration = duration.ToUintTime();
            data.offset = offset;

            bytesWriter.WriteStruct(ref data);
            return bytes;
        }
    }
}