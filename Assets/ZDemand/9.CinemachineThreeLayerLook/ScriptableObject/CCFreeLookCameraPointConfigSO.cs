using System;
using System.Collections.Generic;
using ASeKi.Extension;
using constants;
using UnityEngine;

namespace CCameraUtility
{
    [Serializable]
    public class CCFreeLookCameraPos
    {
        public CCFreeLookScene ownScene = CCFreeLookScene.FashionScene;
        public int CameraIndex = 0;
        public int PosId = 0;

        public CCFreeLookCameraPos(CCFreeLookScene scene,int cameraIndex)
        {
            ownScene = scene;
            this.CameraIndex = cameraIndex;
        }
    }

    [CreateAssetMenu(fileName = "CCFreeLookCameraPointConfigSO", menuName = "CCFreeLookCameraPointConfigSO")]
    public class CCFreeLookCameraPointConfigSO : ScriptableObject
    {
        public CCFreeLookScene curScene = CCFreeLookScene.FashionScene;
        public List<CCFreeLookCameraPos> ccFashionFreeLook = new List<CCFreeLookCameraPos>();
        public List<CCFreeLookCameraPos> ccPetFreeLook = new List<CCFreeLookCameraPos>();
    }
}

