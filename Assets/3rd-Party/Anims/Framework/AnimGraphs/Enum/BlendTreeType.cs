using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.AnimGraphs
{

    public enum BlendTreeType : ushort
    {
        Simple1D = 0,         
        SimpleDirectional2D = 1,         
        FreeformDirectional2D = 2,  
        FreeformCartesian2D = 3,
        Direct = 4
    }
}
