using System.IO;
using UnityEditor;
using UnityEngine;

namespace ASeKi.Demand
{
    public static class CaptureScreenUtility
    {
        public static void CaptureFullSceenAndSave(Camera captureCamera,TextureFormat format = TextureFormat.ARGB32)
        {
            string filePath = EditorUtility.SaveFilePanel("Save Png", "", "MyPng", "png");
            capture(captureCamera, filePath, format);
        }
        
        private static void capture(Camera captureCamera,string path,TextureFormat format = TextureFormat.ARGB32)
        {
            int width = Screen.width, height = Screen.height;
            RenderTexture rt = new RenderTexture(width, height, 0);
            captureCamera.targetTexture = rt;
            captureCamera.Render();
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(width, height, format, false);
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();
            captureCamera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
        }
    }
}