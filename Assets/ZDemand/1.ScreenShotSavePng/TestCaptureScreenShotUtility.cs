using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ASeKi.Demand
{
    public class TestCaptureScreenShotUtility : MonoBehaviour
    {
        public string myPathFileName = "";
        public Camera captureCamera = null;

        private void Start()
        {
            myPathFileName = Application.dataPath + "/需求示例/截图保存Png/MyScreenShotOne.png";
        }

        public void CaptureScreenShotWithScreenCapture()
        {
            CaptureScreenShotUtility.CaptureScreenShotWithScreenCapture(myPathFileName);
        }
        
        public void CaptureScreenShotWithCamera()
        {
            CaptureScreenShotWithCamera(myPathFileName);
        }
        
        public void CaptureScreenShotWithCamera(string pathFileName)
        {
            int width = 1920, height = 1080;
            RenderTexture rt =  new RenderTexture(width, height, 0);
            CaptureScreenShotUtility.CaptureScreenShotWithCamera(captureCamera,rt,width,height,pathFileName);
            Destroy(rt);
        }
        
        // 使用C#自己的API筛选文件
        public void CaptureScreenShotWithCameraAndFreeSave()
        {
            FreeSelectFolder.SelectSaveImgFolder(CaptureScreenShotWithCamera);
        }
        
        // 使用Unity自己的API筛选文件
        public void CaptureScreenShotWithCameraAndFreeSaveWithEditor()
        {
            string path = FreeSelectFolder.SelectSaveImgFolderForPng();
            if (path.Length != 0)
            {
                CaptureScreenShotWithCamera(path);
            }
        }
    }
}