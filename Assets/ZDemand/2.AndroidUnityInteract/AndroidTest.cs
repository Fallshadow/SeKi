using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Android;

namespace ASeKi.Demand
{
    public class AndroidTest : MonoBehaviour
    {

        AndroidJavaClass unity;
        AndroidJavaObject currentActivity;
        AndroidJavaClass phoneInfoMgr;
        public Text wifi;
        public Button request;
        public Button call;

        private void Awake()
        {
            Initialize();
        }

        private void Start()
        {
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
        request.onClick.AddListener(CallPhoneRequest);
        call.onClick.AddListener(CallPhone);
#endif
        }

        private void Update()
        {
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
        // wifi.text = GetWifi().ToString();        
#endif
        }

        public void Initialize()
        {
#if UNITY_ANDROID && !UNITY_EDITOR_WIN
            unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            phoneInfoMgr = new AndroidJavaClass("com.example.phoneinfohelper.PhoneInfoMgr");
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

        public void CallPhoneRequest()
        {
            if(!Permission.HasUserAuthorizedPermission("android.permission.CALL_PHONE"))
            {
                Permission.RequestUserPermission("android.permission.CALL_PHONE");
            }
        }

        public void CallPhone()
        {
            phoneInfoMgr.CallStatic("CallPhone", "18846047017");
        }
#endif
    }
}