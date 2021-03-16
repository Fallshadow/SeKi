using System.IO;
using UnityEditor;
using UnityEngine;

namespace ASeKi.Demand
{
    public class TestCaptureScreenUtility : MonoBehaviour
    {

        public void CaptureFullSceenAndSave()
        {
            CaptureScreenUtility.CaptureFullSceenAndSave(Camera.main);
        }
    }
}