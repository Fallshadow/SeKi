using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;


namespace Framework.AnimGraphs
{

    public partial class AnimGraphSerializationWindow
    {
        private void DeserializeLayer(AnimatorControllerLayer layer,int layerIndex)
        {
            Debug.Log($"Layer Name：{layer.name}");
            DeserializeAnimatorStateMachine(layer.stateMachine, layerIndex);

        }
    }
}