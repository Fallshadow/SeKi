using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ASeKi.action
{
    public static class EditorInternalHacker
    {
        public static class EditorUtility
        {
            private static System.Type type = null;
            public static System.Type Type => type == null ? type = System.Type.GetType( "UnityEditor.EditorUtility, UnityEditor" ) : type;
        
            public static GameObject InstantiateForAnimatorPreview(Object obj)
            {
                return (GameObject) Type.InvokeMember(
                    "InstantiateForAnimatorPreview",
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                    null,
                    null,
                    new object[] {obj}
                );
            }
        }
        
        public static class AnimatorType
        {
            private static System.Type type = null;
            public static System.Type Type => type == null ? type = System.Type.GetType("UnityEngine.Animator, UnityEngine") : type;
        }
        
        public static Vector3 bodyPositionInternal(this Animator animator)
        {
            var pro = AnimatorType.Type.GetProperty("bodyPositionInternal", BindingFlags.Instance | BindingFlags.NonPublic);
            return pro == null ? Vector3.zero : (Vector3) pro.GetValue(animator);
        }
    }
    
}

