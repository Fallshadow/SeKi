using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace Framework.AnimGraphs
{
    public interface IPreDeserialization
    {
        void PreDeserialization(BinaryWriter writer);
    }
}