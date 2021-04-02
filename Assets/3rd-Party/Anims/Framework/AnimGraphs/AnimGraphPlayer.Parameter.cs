using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Framework.AnimGraphs
{
    public unsafe sealed partial class AnimGraphPlayer
    {
        private void InitParameter()
        {
            var array = controller.GetParametersAsset();
            parameters = new ParameterValue[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                var parameter = array[i];
                ref ParameterValue v = ref parameters[i];
                v.type = parameter.parameterType;
                v.hash = parameter.nameHash;
                v.value = default;
            }
        }
    }
}