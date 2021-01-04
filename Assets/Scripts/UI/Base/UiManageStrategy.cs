using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.ui
{
    public enum UiOpenType
    {
        UOT_COMMON = 0,
        UOT_FULL_SCREEN = 1,    // 全屏大型Canvas  可以叠加，只能通过操作打开
        UOT_POP_UP = 2,         // 弹出/置顶类     可以叠加，只能通过操作打开
        UOT_NOTICE = 3,         // 通知            可以叠加但是无需等待动画播放完毕，通过服务端通知
        UOT_PLOT = 4,           // 剧情动画UI       
        UOT_PERSPECTIVE = 5,    // 正交UI
        UOT_GUIDE = 6,          // 引导UI
        UOT_PHOTO = 7,          // 截屏UI
    }

    public class UiManageStrategy
    {
        public RectTransform MainRoot { get { return fullScreenRoot; } }

        private RectTransform fullScreenRoot = null;
        private RectTransform popUpWindowRoot = null;
        private RectTransform noticeWindowRoot = null;
        private RectTransform plotWindowRoot = null;
        private RectTransform perspectiveRoot = null;
        private RectTransform guideRoot = null;
        private RectTransform photoRoot = null;

        public UiManageStrategy(RectTransform[] roots)
        {
            fullScreenRoot = roots[0];
            popUpWindowRoot = roots[1];
            noticeWindowRoot = roots[2];
            plotWindowRoot = roots[3];
            perspectiveRoot = roots[4];
            guideRoot = roots[5];
            photoRoot = roots[6];
            //FrontMask = popUpWindowRoot.GetComponentInChildren<UiFullScreenMask>();
            //FrontMask.Initialize();
        }

        // 初始化3D UI（因其层级特殊性、需要与自动适配的MainRoot对齐）
        public void InitPerspectiveRoot(Camera cam)
        {
            perspectiveRoot.gameObject.layer = UiManager.perspectiveLayer;
            cam.gameObject.layer = UiManager.perspectiveLayer;
            cam.cullingMask = (int)Mathf.Pow(2, UiManager.perspectiveLayer);

            perspectiveRoot.sizeDelta = MainRoot.sizeDelta;
            perspectiveRoot.localScale = MainRoot.localScale;
            var distance = (0.5f * perspectiveRoot.sizeDelta.y) / (Mathf.Tan(cam.fieldOfView * 0.5f * Mathf.Deg2Rad));
            cam.transform.localPosition = new Vector3(0f, 0f, -distance);
            cam.gameObject.SetActive(false);

            debug.PrintSystem.Log($"-----------UiManageStrategy 初始化3D UI-----------", debug.PrintSystem.PrintBy.sunshuchao);
            debug.PrintSystem.Log($"UiManageStrategy 设置3D Camera层级 layer：{cam.gameObject.layer}", debug.PrintSystem.PrintBy.sunshuchao);
            debug.PrintSystem.Log($"UiManageStrategy 设置3D CameraCull cullingMask：{cam.cullingMask}", debug.PrintSystem.PrintBy.sunshuchao);
            debug.PrintSystem.Log($"UiManageStrategy 设置3D Camera位置 localPosition：{cam.transform.localPosition}", debug.PrintSystem.PrintBy.sunshuchao);
            debug.PrintSystem.Log($"-----------END-----------", debug.PrintSystem.PrintBy.sunshuchao);
        }
    }
}