using UnityEngine.EventSystems;
using UnityEngine;
using System;
using System.Collections.Generic;
using act.UIRes;

namespace ASeKi.ui
{
    public partial class UiManager : SingletonMonoBehavior<UiManager>
    {
        public const int VisibleUiLayer = 5;        // unity默认UI层
        public const int InvisibleUiLayer = 6;      // unity默认不可更改的层，用作UI的隐藏显示
        public const int perspectiveLayer = 7;      // unity默认不可更改的层，用作UI的正交场景

        #region debug显示
        [SerializeField] private GameObject debugRoot = null;
        [SerializeField] private UnityEngine.UI.Text Fps = null;
        #endregion

        #region 相机相关
        public EventSystem UiCameraEventSystem => uiCameraEventSystem;
        public Camera UiCamera => uiCamera;
        public Camera PhotoCamera => photoCamera;
        public Camera PerspectiveCamera => perspectiveCamera;
        
        [SerializeField] private EventSystem uiCameraEventSystem = null;
        [SerializeField] private Camera uiCamera = null;
        [SerializeField] private Camera photoCamera = null; // 专门用于拍照相机
        [SerializeField] private Camera perspectiveCamera = null;

        #endregion

        #region 适配相关
        public Vector4 retractionSize { get; private set; } // Ui最终适配区域 = 安全区域 + 特殊修正 + 刘海区域。表示左右上下的缩进边距 z=下边距 w=上边距

        #endregion

        #region 界面通用管理相关

        private Dictionary<UiAssetIndex, UiBase> loadedUiDict = new Dictionary<UiAssetIndex, UiBase>();
        List<UiAssetIndex> destroyUis = new List<UiAssetIndex>();           // 等待多少秒之后删除掉

        /// <summary>
        /// 创建UI
        /// </summary>
        /// <param name="uiType">C#类型 system.type</param>
        /// <returns>UiBase</returns>
        public UiBase CreateUi(Type uiType)
        {
            UiAssetIndex uiAssetIndex = GetUiAssetIndex(uiType);
            loadedUiDict.TryGetValue(uiAssetIndex, out UiBase ui);
            if (ui != null)
            {
                return ui;
            }

            getUiViaType<UiBase>(uiType, out ui);
            if(ui == null)
            {
                debug.PrintSystem.Log($"[UI Open]没有得到UI资源{uiType}");
                return null;
            }

            ui = manageStrategy.CreateUi(ui);
            ui.OnCreate();
            loadedUiDict[uiAssetIndex] = ui;
            return ui;
        }

        public UiBase CreateUi<T>() where T : UiBase
        {
            Type uiType = typeof(T);
            return CreateUi(uiType) as T;
        }

        public void OpenUi(Type type, Action completeCb = null)
        {
            UiAssetIndex uiAssetIndex = GetUiAssetIndex(type);
            loadedUiDict.TryGetValue(uiAssetIndex, out UiBase ui);
            if (ui == null)
            {
                ui = CreateUi(type);
            }

            manageStrategy.OpenUi(ui, completeCb);
            if (ui.OpenType == UiOpenType.UOT_FULL_SCREEN)
            {
                // SetEdgeMask();
            }
        }

        public void OpenUi<T>(Action completeCb = null) where T : UiBase
        {
            Type uiType = typeof(T);
            OpenUi(typeof(T), completeCb);
        }


        /// <summary>
        /// 销毁所有UI，但是除了参数里的UI
        /// </summary>
        /// <param name="reopenOne">不销毁的UI</param>
        public void DestroyAllUi(Type reopenOne = null)
        {
            StopAllCoroutines();
            UiBase ui = null;
            UiAssetIndex uiAssetIndex = UiAssetIndex.NONE;
            if (null != reopenOne)
            {
                uiAssetIndex = GetUiAssetIndex(reopenOne);
                loadedUiDict.TryGetValue(uiAssetIndex, out ui);
            }

            manageStrategy.Clear(ui);
            destroyUis.Clear();
            foreach (KeyValuePair<UiAssetIndex, UiBase> kvPair in loadedUiDict)
            {
                if (kvPair.Value == null)
                {
                    continue;
                }

                if (kvPair.Value.IsDontDestroy || kvPair.Key == uiAssetIndex)
                {
                    continue;
                }

                destroyUis.Add(kvPair.Key);
                kvPair.Value.OnRuin();
                Destroy(kvPair.Value.gameObject);
                unloadAsset(kvPair.Key);
            }
            
            for (int i = 0, count = destroyUis.Count; i < count; ++i)
            {
                loadedUiDict.Remove(destroyUis[i]);
            }
            unloadAllAssets();
            SetCamera(true);

            if (null != reopenOne)
            {
                OpenUi(reopenOne);
            }
        }

        
        public void  SetCamera(bool isActive)
        {
            uiCamera.enabled = isActive;
            uiCameraEventSystem.enabled = isActive;
        }
        
        public static UiAssetIndex GetUiAssetIndex(Type type)
        {
            Type attrType = typeof(BindingResourceAttribute);
            BindingResourceAttribute attr = Attribute.GetCustomAttribute(type, attrType) as BindingResourceAttribute;
            return attr.AssetId;
        }

        #endregion

        [SerializeField] private RectTransform[] canvasRoots = null;

        private UiManageStrategy manageStrategy = null;

        protected override void init()
        {
            manageStrategy = new UiManageStrategy(canvasRoots);
        }

        private void Start()
        {
            // TODO: 启用动态加载
            manageStrategy.InitPerspectiveRoot(perspectiveCamera);
        }

        private void Update()
        {
            // m_uiPropupQueue.Operate();
            showFPS();
            // 每隔30帧检测一次
            const int frameInterval = 30;
            // ui关闭后，如果没用的话20s后才卸载
            const float delayClose = 20f;
            if (UnityEngine.Time.frameCount % frameInterval != 0)
            {
                return;
            }
            destroyUis.Clear();
            foreach (KeyValuePair<UiAssetIndex, UiBase> kvPair in loadedUiDict)
            {
                if (kvPair.Value == null)
                {
                    continue;
                }

                if (kvPair.Value.IsDontDestroy)
                {
                    continue;
                }

                if (!kvPair.Value.bDynamicUnload)
                {
                    continue;
                }

                if (kvPair.Value.LastCloseTime == null)
                {
                    continue;
                }

                if (Time.time - kvPair.Value.LastCloseTime.Value < delayClose)
                {
                    continue;
                }

                if (manageStrategy.CheckOpenOrNot(kvPair.Value))
                {
                    continue;
                }

                destroyUis.Add(kvPair.Key);
                kvPair.Value.OnRuin();
                Destroy(kvPair.Value.gameObject);
                unloadAsset(kvPair.Key);
            }

            for (int i = 0, count = destroyUis.Count; i < count; ++i)
            {
                loadedUiDict.Remove(destroyUis[i]);
            }

        }

        void showDebugUI()
        {
            debugRoot.SetActive(debug.DebugConfig.instance.ShowDebugUI);
            showFPS();
        }

        void showFPS()
        {
            if(debug.DebugConfig.instance.ShowFps)
            {
                Fps.text = $"FPS: {(1f / Time.smoothDeltaTime):F0}";
            }
            else
            {
                Fps.text = "";
            }
        }
        
        #region 一些功能API
        
        public bool IsUiOpen<T>() where T : UiBase
        {
            T ui = GetUi<T>();
            if(null == ui)
            {
                return false;
            }
            else
            {
                return ui.IsOpen;
            }
        }
        
        /// <summary>
        /// 删除掉UI
        /// </summary>
        /// <param name="ui"></param>
        public void DestroyUi(UiBase ui)
        {
            Type uiType = ui.GetType();
            UiAssetIndex uiAssetIndex = GetUiAssetIndex(uiType);
            loadedUiDict.TryGetValue(uiAssetIndex, out UiBase loadedUi);
            if (ui == loadedUi)
            {
                loadedUiDict.Remove(uiAssetIndex);
            }

            ui.OnRuin();
            Destroy(ui.gameObject);
            unloadAsset(uiAssetIndex);
        }
        
        /// <summary>
        /// 获取UI类
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetUi<T>() where T : UiBase
        {
            Type uiType = typeof(T);
            UiAssetIndex uiAssetIndex = GetUiAssetIndex(uiType);
            if (loadedUiDict.TryGetValue(uiAssetIndex, out UiBase loadedUi))
            {
                return loadedUi as T;
            }
            else
            {
                return null;
            }
        }
        
        /// <summary>
        /// 获取UI的transform
        /// </summary>
        /// <param name="uiType"></param>
        /// <returns></returns>
        public Transform GetUiTransform(Type uiType)
        {
            UiAssetIndex uiAssetIndex = GetUiAssetIndex(uiType);
            if (loadedUiDict.TryGetValue(uiAssetIndex, out UiBase loadedUi))
            {
                return loadedUi.transform;
            }
            else
            {
                return null;
            }
        }

        #endregion
        
    }
}