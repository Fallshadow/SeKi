using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Framework.AnimGraphs
{
    public struct ParameterValue
    {
        public ParameterType type;
        public LValue value;
        public int hash;
    }
}