using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.Demand
{
    public class TestFreeSelectFolder : MonoBehaviour
    {
        public void SelectSaveImgFolder()
        {
            FreeSelectFolder.SelectSaveImgFolder(null);
        }
        
        public void SelectSavePngFolderWithEditor()
        {
            FreeSelectFolder.SelectSaveImgFolderForPng();
        }
    }
}