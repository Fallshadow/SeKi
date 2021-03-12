using System.IO;
using UnityEngine;

namespace ASeKi.Demand
{
    public class TransparentPNGByCamera : MonoBehaviour
    {
        public Camera captureCamera;

        // Start is called before the first frame update
        private void captureTransparent()
        {
            int width = 1920, height = 1080;
            RenderTexture rt = new RenderTexture(width, height, 0);
            captureCamera.targetTexture = rt;
            captureCamera.Render();
            RenderTexture.active = rt;
            Texture2D screenShot = new Texture2D(width, height, TextureFormat.ARGB32, false);
            screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenShot.Apply();
            captureCamera.targetTexture = null;
            RenderTexture.active = null;
//        Color color;
//        for (int y = 0; y < height; ++y)
//        {
//            // each row
//            for (int x = 0; x < width; ++x)
//            {
//                color = Color.clear;
//                screenShot.SetPixel(x, y, color);
//            }
//        }
            byte[] bytes = screenShot.EncodeToPNG();
            File.WriteAllBytes("ScreenShot.png", bytes);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                captureTransparent();
            }
        }
    }
}