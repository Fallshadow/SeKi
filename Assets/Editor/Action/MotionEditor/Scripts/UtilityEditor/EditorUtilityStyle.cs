using UnityEditor;
using UnityEngine;

namespace ASeKi.action
{
    public class EditorUtilityStyle
    {         
        public GUIStyle TipBlueStyle = null;
        public GUIStyle ErrorRedStyle = null;
        public GUIStyle WarningYellowStyle = null;
        
        private Texture2D m_GridTexture;
        public Texture2D GridTextureUE4 => 
            m_GridTexture == null ? 
                m_GridTexture = Resources.Load<Texture2D>("Styles/grid-texture") : m_GridTexture;
            
        public EditorUtilityStyle()
        {
            TipBlueStyle = new GUIStyle(EditorGUIUtility.GetBuiltinSkin(EditorGUIUtility.isProSkin ? EditorSkin.Scene : EditorSkin.Inspector).FindStyle("TextField"))
            {
                wordWrap = true,
                normal =
                {
                    textColor = new Color(102f / 255f, 153f / 255f, 255f / 255f)
                }
            };
                
            ErrorRedStyle = new GUIStyle()
            {
                wordWrap = true,
                normal = {textColor = Color.red}
            };

            WarningYellowStyle = new GUIStyle()
            {
                wordWrap = true,
                normal = {textColor = new Color(1f, 0.71f, 0.17f) }
            };
        }
    }
}
