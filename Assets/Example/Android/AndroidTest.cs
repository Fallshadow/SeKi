using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;

public class AndroidTest : MonoBehaviour
{

    AndroidJavaClass unity;
    AndroidJavaObject currentActivity;
    AndroidJavaClass phoneInfoMgr;
    public Text wifi;
    private void Awake()
    {
        Initialize();
    }
    private void Update()
    {
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
        wifi.text = GetWifi().ToString();        
#endif
    }

    public void Initialize()
    {
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
            unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            phoneInfoMgr = new AndroidJavaClass("com.ssc.fortomorrowtest.MainTest");
            phoneInfoMgr.CallStatic("Init", currentActivity);
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR_WIN
        public float GetSignal()
        {
            return phoneInfoMgr.CallStatic<int>("GetSignal");
        }

        public int GetWifi()
        {
            return phoneInfoMgr.CallStatic<int>("getWifiInfo");
        }
#endif
}
