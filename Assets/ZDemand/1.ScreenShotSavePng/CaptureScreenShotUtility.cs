using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ASeKi.Demand
{
    public static class CaptureScreenShotUtility
    {
        public static void CaptureScreenShotWithScreenCapture(string pathFileName)
        {
            ScreenCapture.CaptureScreenshot(pathFileName);
        }
        
        public static void CaptureScreenShotWithCamera(Camera captureCamera,RenderTexture rt,int width,int height, string pathFileName)
        {
            captureCamera.targetTexture = rt;
            captureCamera.Render();
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();
            captureCamera.targetTexture = null;
            RenderTexture.active = null;
            byte[] bytes = screenShot.EncodeToPNG();
            File.WriteAllBytes(pathFileName,bytes);
        }
    }
}
