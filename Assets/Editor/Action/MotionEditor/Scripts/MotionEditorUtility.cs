using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.action
{
    public static class MotionEditorUtility
    {
        public static MotionEditorStyle Style = new MotionEditorStyle();

        public class MotionEditorStyle
        {            
            private Texture2D m_GridTexture;
            public Texture2D GridTextureUE4 => m_GridTexture == null
                ? m_GridTexture = Resources.Load<Texture2D>("Styles/grid-texture")
                : m_GridTexture;
        }
    }
}