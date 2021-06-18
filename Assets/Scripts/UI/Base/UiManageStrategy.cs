using System;
using System.Collections.Generic;
using UnityEngine;

namespace ASeKi.ui
{
    public enum UiOpenType
    {
        UOT_COMMON = 0,                     // 普通UI 默认继承UIBASE的
        UOT_FULL_SCREEN = 1,                // 全屏大型Canvas       可以叠加，只能通过操作打开
        UOT_POP_UP = 2,                     // 弹出/置顶类          可以叠加，只能通过操作打开，主要用途还是呈现一些界面上的信息
        UOT_MESSAGE = 3,                    // 一般消息             可以叠加，只能通过操作打开，主要用途还是呈现一些突如其来的信息
        UOT_NOTICE = 4,                     // 通知                 toast、邀请好友这种不遮挡的UI
        UOT_PLOT = 5,                       // 剧情动画UI       
        UOT_IMPORTANTMESSAGE = 6,           // 超级重要UI           比如断线重连
        UOT_PERSPECTIVE = 7,                // 正交UI
        UOT_GUIDE = 8,                      // 引导UI
        UOT_PHOTO = 9,                      // 截屏UI
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

        private LinkedList<UiBase> fullScreenCavases = new LinkedList<UiBase>();
        private LinkedList<UiBase> popUpWindows = new LinkedList<UiBase>();
        private LinkedList<UiBase> commonUis = new LinkedList<UiBase>();
        private LinkedList<UiBase> noticeUis = new LinkedList<UiBase>();
        private LinkedList<UiBase> plotUis = new LinkedList<UiBase>();
        private LinkedList<UiBase> perspectiveCanvases = new LinkedList<UiBase>();
        private LinkedList<UiBase> guideUis = new LinkedList<UiBase>();
        private LinkedList<UiBase> photoUis = new LinkedList<UiBase>();
        // private UiBase defualtUi = null;            // 一般游戏都会有一个默认的UI，比如主城、战斗
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

            debug.PrintSystem.Log($"-----------UiManageStrategy 初始化3D UI-----------", debug.PrintSystem.PrintBy.SunShuChao);
            debug.PrintSystem.Log($"UiManageStrategy 设置3D Camera层级 layer：{cam.gameObject.layer}", debug.PrintSystem.PrintBy.SunShuChao);
            debug.PrintSystem.Log($"UiManageStrategy 设置3D CameraCull cullingMask：{cam.cullingMask}", debug.PrintSystem.PrintBy.SunShuChao);
            debug.PrintSystem.Log($"UiManageStrategy 设置3D Camera位置 localPosition：{cam.transform.localPosition}", debug.PrintSystem.PrintBy.SunShuChao);
            debug.PrintSystem.Log($"-----------END-----------", debug.PrintSystem.PrintBy.SunShuChao);
        }

        // 只有不在队列里面才是真正的关闭
        public bool CheckOpenOrNot(UiBase uiBase)
        {
            return fullScreenCavases.Contains(uiBase) 
                || popUpWindows.Contains(uiBase) 
                || commonUis.Contains(uiBase)
                || noticeUis.Contains(uiBase) 
                || plotUis.Contains(uiBase) 
                || perspectiveCanvases.Contains(uiBase) 
                || guideUis.Contains(uiBase);
        }

        public UiBase CreateUi(UiBase uiPrefab)
        {
            UiBase ui;
            switch(uiPrefab.OpenType)
            {
                case UiOpenType.UOT_FULL_SCREEN:
                case UiOpenType.UOT_COMMON:
                    {
                        ui = UnityEngine.Object.Instantiate(uiPrefab, fullScreenRoot);
                        break;
                    }
                case UiOpenType.UOT_POP_UP:
                    {
                        ui = UnityEngine.Object.Instantiate(uiPrefab, popUpWindowRoot);
                        break;
                    }
                case UiOpenType.UOT_NOTICE:
                    {
                        ui = UnityEngine.Object.Instantiate(uiPrefab, noticeWindowRoot);
                        break;
                    }
                case UiOpenType.UOT_PLOT:
                    {
                        ui = UnityEngine.Object.Instantiate(uiPrefab, plotWindowRoot);
                        break;
                    }
                case UiOpenType.UOT_PERSPECTIVE:
                    {
                        ui = UnityEngine.Object.Instantiate(uiPrefab, perspectiveRoot);
                        break;
                    }
                case UiOpenType.UOT_GUIDE:
                    {
                        ui = UnityEngine.Object.Instantiate(uiPrefab, guideRoot);
                        break;
                    }
                case UiOpenType.UOT_PHOTO:
                    {
                        ui = UnityEngine.Object.Instantiate(uiPrefab, photoRoot);
                        break;
                    }
                default:
                    {
                        return null;
                    }
            }
            return ui;
        }

        public void OpenUi(UiBase ui, Action completeCb)
        {
            switch(ui.OpenType)
            {
                case UiOpenType.UOT_COMMON:
                    {
                        openCommonUi(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_FULL_SCREEN:
                    {
                        openFullScreenCanvas(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_POP_UP:
                    {
                        openPopUpWindow(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_NOTICE:
                    {
                        openNoticeUi(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_PLOT:
                    {
                        openPlotUi(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_PERSPECTIVE:
                    {
                        openPerspectiveCanvas(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_GUIDE:
                    {
                        openGuideUi(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_PHOTO:
                    {
                        openPhotoUi(ui, completeCb);
                        break;
                    }
            }
        }

        public void CloseUi(UiBase ui, Action completeCb)
        {
            switch(ui.OpenType)
            {
                case UiOpenType.UOT_COMMON:
                    {
                        closeCommonUi(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_FULL_SCREEN:
                    {
                        closeFullScreenCanvas(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_POP_UP:
                    {
                        closePopUpWindow(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_NOTICE:
                    {
                        closeNoticeUi(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_PLOT:
                    {
                        closePlotUi(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_PERSPECTIVE:
                    {
                        closePerspectiveCanvas(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_GUIDE:
                    {
                        closeGuideUi(ui, completeCb);
                        break;
                    }
                case UiOpenType.UOT_PHOTO:
                    {
                        closePhotoUi(ui, completeCb);
                        break;
                    }
            }
        }

        public void Clear(UiBase remainOne = null)
        {
            // 根据不销毁的UI决定保留哪些Mask，后续有需要再进行处理
            // if(null == remainOne || !(remainOne.OpenType == UiOpenType.UOT_POP_UP))
            // {
            //     FrontMask.SetFrontMask(0f);
            // }
            // if (null == remainOne || !(remainOne.OpenType == UiOpenType.UOT_IMPORTANT_POP_UP))
            // {
            //     ImportantMessageMask.SetFrontMask(0f);
            // }
            // if (null == remainOne || !(remainOne.OpenType == UiOpenType.UOT_MESSAGE_POP_UP))
            // {
            //     MessageMask.SetFrontMask(0f);
            // }
            
            popUpWindowRoot.gameObject.SetActive(false);
            fullScreenCavases.Clear();
            popUpWindows.Clear();
            commonUis.Clear();
            noticeUis.Clear();
            perspectiveCanvases.Clear();
        }

        #region CommonUi
        private void openCommonUi(UiBase ui, Action completeCb)
        {
            commonUis.AddLast(ui);
            ui.transform.SetParent(fullScreenRoot);
            ui.transform.SetAsLastSibling();
            ui.Open(onCommonUiClose, completeCb);
        }

        private void closeCommonUi(UiBase ui, Action completeCb)
        {
            ui.Close(completeCb);
        }

        private void onCommonUiClose(UiBase ui)
        {
            commonUis.Remove(ui);
        }
        #endregion

        #region FullScreen
        private void openFullScreenCanvas(UiBase ui, Action completeCb)
        {
            if(fullScreenCavases.Contains(ui))
            {
                debug.PrintSystem.LogWarning($"[MainCanvasRoot] UI has already open. UI: {ui.name}");
                completeCb?.Invoke();
                return;
            }

            LinkedListNode<UiBase> lastNode = fullScreenCavases.Last;
            if(lastNode == null || lastNode.Value.State != UiState.Show && lastNode.Value.State != UiState.ShowingAnim)
            {
                fullScreenCavases.AddLast(ui);
                ui.transform.SetParent(fullScreenRoot);
                ui.transform.SetAsFirstSibling();
                // UiManager.instance.SetEdgeMask();
                ui.Open(onFullScreenCanvasClose, completeCb);
                return;
            }

            lastNode.Value.Hide(() => openFullScreenCanvas(ui, completeCb));
        }

        private void onFullScreenCanvasClose(UiBase ui)
        {
            debug.PrintSystem.Assert(fullScreenCavases.Count != 0, "fullScreenCavases count can't equal 0");
            bool isOpenLast = (ui == fullScreenCavases.Last.Value);
            fullScreenCavases.Remove(ui);
            if(fullScreenCavases.Count == 0 || !isOpenLast)
            {
                evt.EventManager.instance.Send(evt.EventGroup.UI, (short)evt.UiEvent.ALL_FULL_SCREEN_CANVAS_CLOSED);
                return;
            }
            else if(fullScreenCavases.Count == 1 && fullScreenCavases.Last.Value is UiBase)//有一个默认开启的TownCanvas
            {
                evt.EventManager.instance.Send(evt.EventGroup.UI, (short)evt.UiEvent.ALL_FULL_SCREEN_CANVAS_CLOSED_BUT_DEFUALT);
            }

            fullScreenCavases.Last.Value.transform.SetAsFirstSibling();
            // UiManager.instance.SetEdgeMask();
            fullScreenCavases.Last.Value.Show();
        }

        private void closeFullScreenCanvas(UiBase ui, Action completeCb)
        {
            if(!fullScreenCavases.Contains(ui))
            {
                debug.PrintSystem.LogWarning($"[MainCanvasRoot] UI is not open. UI: {ui.name}");
                completeCb?.Invoke();
                return;
            }

            ui.Close(completeCb);
        }

        #endregion

        #region PopUp
        private void openPopUpWindow(UiBase ui, Action completeCb)
        {
            if(popUpWindows.Contains(ui))
            {
                debug.PrintSystem.LogWarning($"[PopUpWindow] UI has already open. UI: {ui.name}");
                completeCb?.Invoke();
                return;
            }

            //FrontMask.SetFrontMask(DefaultFrontMaskAlpha, true); // TODO: Read setting in windows.
            //FrontMask.transform.SetAsLastSibling();
            popUpWindowRoot.gameObject.SetActive(true);
            if(popUpWindows.Count == 0 || popUpWindows.Last.Value.State == UiState.Hide)
            {
                popUpWindows.AddLast(ui);
                ui.transform.SetParent(popUpWindowRoot);
                ui.transform.SetAsLastSibling();
                ui.Open(onPopUpWindowClose, completeCb);
                return;
            }
            popUpWindows.Last.Value.Hide(() => openPopUpWindow(ui, completeCb));
        }

        private void onPopUpWindowClose(UiBase ui)
        {
            popUpWindows.Remove(ui);
            if(popUpWindows.Count == 0)
            {
                // FrontMask.SetFrontMask(0f);
                popUpWindowRoot.gameObject.SetActive(false);
            }
            else
            {
                popUpWindows.Last.Value.transform.SetAsLastSibling();
                popUpWindows.Last.Value.Show();
            }
        }

        private void closePopUpWindow(UiBase ui, Action completeCb)
        {
            ui.Close(completeCb);
        }
        #endregion

        #region Notice
        private void openNoticeUi(UiBase ui, Action completeCb)
        {
            noticeUis.AddLast(ui);
            ui.transform.SetParent(noticeWindowRoot);
            ui.transform.SetAsFirstSibling();
            ui.Open(onCommonUiClose, completeCb);
        }

        private void onNoticeUiClose(UiBase ui)
        {
            noticeUis.Remove(ui);
        }

        private void closeNoticeUi(UiBase ui, Action completeCb)
        {
            ui.Close(completeCb);
        }
        #endregion

        #region plot windows
        private void openPlotUi(UiBase ui, Action completeCb)
        {
            ui.transform.SetParent(plotWindowRoot);
            plotUis.AddLast(ui);
            ui.transform.SetAsLastSibling();
            ui.Open(onPlotUiClose, completeCb);
        }

        private void onPlotUiClose(UiBase ui)
        {
            plotUis.Remove(ui);
        }

        private void closePlotUi(UiBase ui, Action completeCb)
        {
            ui.Close(completeCb);
        }
        #endregion

        #region perspective canvas 
        private void openPerspectiveCanvas(UiBase ui, Action completeCb)
        {
            if(perspectiveCanvases.Contains(ui))
            {
                debug.PrintSystem.LogWarning($"[perspectiveCanvasRoot] UI has already open. UI: {ui.name}");
                completeCb?.Invoke();
                return;
            }

            if(perspectiveCanvases.Count == 0 || perspectiveCanvases.Last.Value.State == UiState.Hide)
            {
                perspectiveCanvases.AddLast(ui);
                ui.transform.SetParent(perspectiveRoot);
                ui.transform.SetAsFirstSibling();
                // UiManager.instance.SetEdgeMask();
                ui.Open(onPerspectiveCanvasClose, completeCb);

                return;
            }

            perspectiveCanvases.Last.Value.Hide(() => openPerspectiveCanvas(ui, completeCb));
        }

        private void closePerspectiveCanvas(UiBase ui, Action completeCb)
        {
            if(!perspectiveCanvases.Contains(ui))
            {
                debug.PrintSystem.LogWarning($"[perspectiveCanvasRoot] UI is not open. UI: {ui.name}");
                completeCb?.Invoke();
                return;
            }

            ui.Close(completeCb);
        }

        private void onPerspectiveCanvasClose(UiBase ui)
        {
            bool isOpenLast = (ui == perspectiveCanvases.Last.Value);
            perspectiveCanvases.Remove(ui);
            if(perspectiveCanvases.Count == 0 || !isOpenLast)
            {
                return;
            }
            perspectiveCanvases.Last.Value.transform.SetAsFirstSibling();
            // UiManager.instance.SetEdgeMask();
            perspectiveCanvases.Last.Value.Show();
        }


        #endregion

        #region guide canvas
        private void openGuideUi(UiBase ui, Action completeCb)
        {
            ui.transform.SetParent(guideRoot);
            guideUis.AddLast(ui);
            ui.transform.SetAsLastSibling();
            // UiManager.instance.SetEdgeMask();
            ui.Open(onGuideCanvasClose, completeCb);
        }

        private void onGuideCanvasClose(UiBase ui)
        {
            bool isOpenLast = (ui == guideUis.Last.Value);
            guideUis.Remove(ui);
            if(guideUis.Count == 0 || !isOpenLast)
            {
                return;
            }
            guideUis.Last.Value.transform.SetAsFirstSibling();
            // UiManager.instance.SetEdgeMask();
            guideUis.Last.Value.Show();
        }
        private void closeGuideUi(UiBase ui, Action completeCb)
        {
            ui.Close(completeCb);
        }
        #endregion

        #region photo canvas
        private void openPhotoUi(UiBase ui, Action completeCb)
        {
            ui.transform.SetParent(photoRoot);
            photoUis.AddLast(ui);
            ui.transform.SetAsLastSibling();
            ui.Open(onPhotoCanvasClose, completeCb);
        }

        private void onPhotoCanvasClose(UiBase ui)
        {
            bool isOpenLast = (ui == photoUis.Last.Value);
            photoUis.Remove(ui);
            if(photoUis.Count == 0 || !isOpenLast)
            {
                return;
            }
            photoUis.Last.Value.transform.SetAsFirstSibling();
            // UiManager.instance.SetEdgeMask();
            photoUis.Last.Value.Show();
        }

        private void closePhotoUi(UiBase ui, Action completeCb)
        {
            ui.Close(completeCb);
        }
        #endregion
    }
}