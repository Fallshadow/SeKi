using System;
using System.Collections;
using UnityEngine;
using System.Collections;
using System.IO;

namespace ASeKi.Demand
{
    /*
    Usage:
    1. Attach this script to your chosen camera's game object.
    2. Set that camera's Clear Flags field to Solid Color.
    3. Use the inspector to set frameRate and framesToCapture
    4. Choose your desired resolution in Unity's Game window (must be less than or equal to your screen resolution)
    5. Turn on "Maximise on Play"
    6. Play your scene. Screenshots will be saved to YourUnityProject/Screenshots by default.
    */

    public class CamerCaptureWithTransparentBackGround : MonoBehaviour
    {
        #region public fields

        [Tooltip("A folder will be created with this base name in your project root")]
        public string folderBaseName = "Screenshots";

//        [Tooltip("How many frames should be captured per second of game time 游戏时间一秒内捕获多少帧")]
//        public int frameRate = 24;
//
//        [Tooltip("How many frames should be captured before quitting")]
//        public int framesToCapture = 24;

        #endregion

        #region private fields

        private string folderName = "";
        private GameObject whiteCamGameObject;
        private Camera whiteCam;
        private GameObject blackCamGameObject;
        private Camera blackCam;
        private Camera mainCam;
        private float originalTimescaleTime;
        private bool done = false;
        private int screenWidth;
        private int screenHeight;
        private Texture2D textureBlack;
        private Texture2D textureWhite;
        private Texture2D textureTransparentBackground;

        #endregion

        void Awake()
        {
            mainCam = gameObject.GetComponent<Camera>();
            CreateBlackAndWhiteCameras();
            CreateNewFolderForScreenshots();
            CacheAndInitialiseFields();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(captureFrame());
            }
        }

        IEnumerator captureFrame()
        {
            yield return new WaitForEndOfFrame();
            RenderCamToTexture(blackCam, textureBlack);
            RenderCamToTexture(whiteCam, textureWhite);
            CalculateOutputTexture();
            SavePng();
        }

        void RenderCamToTexture(Camera cam, Texture2D tex)
        {
            cam.enabled = true;
            cam.Render();
            WriteScreenImageToTexture(tex);
            cam.enabled = false;
        }
        
        void WriteScreenImageToTexture(Texture2D tex)
        {
            tex.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
            tex.Apply();
        }

        void CalculateOutputTexture()
        {
            Color color;
            for (int y = 0; y < textureTransparentBackground.height; ++y)
            {
                // each row
                for (int x = 0; x < textureTransparentBackground.width; ++x)
                {
                    // each column
                    float alpha = textureWhite.GetPixel(x, y).r - textureBlack.GetPixel(x, y).r;
                    alpha = 1.0f - alpha;
                    if (alpha == 0)
                    {
                        color = Color.clear;
                    }
                    else
                    {
                        color = textureBlack.GetPixel(x, y) / alpha;
                    }

                    color.a = alpha;
                    textureTransparentBackground.SetPixel(x, y, color);
                    Debug.Log($"textureWhite.GetPixel(x, y).r: {textureWhite.GetPixel(x, y).r};" +
                              $"textureBlack.GetPixel(x, y).r: {textureBlack.GetPixel(x, y).r};" +
                              $"alpha: {alpha};" +
                              $"color: {color}");
                }
            }
        }
        
        void SavePng()
        {
            string name = string.Format("{0}/{1:D04} shot.png", folderName);
            var pngShot = textureTransparentBackground.EncodeToPNG();
            File.WriteAllBytes(name, pngShot);
        }

        // 复制主相机，生成两个黑白相机
        void CreateBlackAndWhiteCameras()
        {
            whiteCamGameObject = (GameObject) new GameObject();
            whiteCamGameObject.name = "White Background Camera";
            whiteCam = whiteCamGameObject.AddComponent<Camera>();
            whiteCam.CopyFrom(mainCam);
            whiteCam.backgroundColor = Color.white;
            whiteCamGameObject.transform.SetParent(gameObject.transform, true);

            blackCamGameObject = (GameObject) new GameObject();
            blackCamGameObject.name = "Black Background Camera";
            blackCam = blackCamGameObject.AddComponent<Camera>();
            blackCam.CopyFrom(mainCam);
            blackCam.backgroundColor = Color.black;
            blackCamGameObject.transform.SetParent(gameObject.transform, true);
        }

        void CreateNewFolderForScreenshots()
        {
            // Find a folder name that doesn't exist yet. Append number if necessary.
            folderName = folderBaseName;
            int count = 1;
            while (System.IO.Directory.Exists(folderName))
            {
                folderName = folderBaseName + count;
                count++;
            }

            System.IO.Directory.CreateDirectory(folderName); // Create the folder
        }
        
        void CacheAndInitialiseFields()
        {
            originalTimescaleTime = Time.timeScale;
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            textureBlack = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
            textureWhite = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
            textureTransparentBackground = new Texture2D(screenWidth, screenHeight, TextureFormat.ARGB32, false);
        }
    }
}