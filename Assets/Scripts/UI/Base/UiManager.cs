using UnityEngine.EventSystems;
using UnityEngine;
using System;
using System.Collections.Generic;

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

        public Camera UiCamera { get => uiCamera; }
        public Camera PhotoCamera { get => photoCamera; }
        public Camera PerspectiveCamera { get => perspectiveCamera; }

        [SerializeField] private Camera uiCamera = null;
        // [SerializeField] private EventSystem eventSystem = null;
        [SerializeField] private Camera photoCamera = null; //专门用于拍照相机
        [SerializeField] private Camera perspectiveCamera = null;

        #endregion

        #region 适配相关
        public Vector4 retractionSize { get; private set; } // Ui最终适配区域 = 安全区域 + 特殊修正 + 刘海区域。表示左右上下的缩进边距 z=下边距 w=上边距

        #endregion

        #region 界面管理相关

        private Dictionary<Type, UiBase> loadedUiDict = new Dictionary<Type, UiBase>();
        List<Type> destroyUis = new List<Type>();           // 等待多少秒之后删除掉

        public UiBase CreateUi(Type uiType)
        {
            if(loadedUiDict.TryGetValue(uiType, out UiBase ui))
            {
                return ui;
            }

            getUiAsset<UiBase>(uiType, out ui);
            if(ui == null)
            {
                debug.PrintSystem.Log($"没有得到UI资源{uiType}");
                return null;
            }

            ui = manageStrategy.CreateUi(ui);
            ui.OnCreate();
            loadedUiDict[uiType] = ui;
            return ui;
        }

        public UiBase CreateUi<T>() where T : UiBase
        {
            Type uiType = typeof(T);
            return CreateUi(uiType) as T;
        }

        public UiBase OpenUi(Type type, Action completeCb = null)
        {
            loadedUiDict.TryGetValue(type, out UiBase ui);
            if(ui == null)
            {
                ui = CreateUi(type);
            }

            manageStrategy.OpenUi(ui, completeCb);
            if(ui.OpenType == UiOpenType.UOT_FULL_SCREEN)
            {
                // SetEdgeMask(); TODO:要不要黑边？
            }
            return ui;
        }

        public T OpenUi<T>(Action completeCb = null) where T : UiBase
        {
            return OpenUi(typeof(T), completeCb) as T;
        }

        public void DestoryUi<T>() where T : UiBase
        {
            Type t = typeof(T);
            if(loadedUiDict.TryGetValue(t, out UiBase item))
            {
                item.OnRuin();
                Destroy(item.gameObject);
                unloadAsset(item);
                loadedUiDict.Remove(t);
            }
        }

        public void DestroyAllUi()
        {
            manageStrategy.Clear();
            destroyUis.Clear();
            foreach(KeyValuePair<Type, UiBase> kvPair in loadedUiDict)
            {
                if(kvPair.Value == null)
                {
                    continue;
                }

                if(kvPair.Value.IsDontDestroy)
                {
                    continue;
                }

                destroyUis.Add(kvPair.Key);
                kvPair.Value.OnRuin();
                Destroy(kvPair.Value.gameObject);
                unloadAsset(kvPair.Value);
            }

            for(int i = 0, count = destroyUis.Count; i < count; ++i)
            {
                loadedUiDict.Remove(destroyUis[i]);
            }
            unloadAllAssets();
        }

        // 尝试获取UI
        public Transform GetUi(Type uiType)
        {
            if(loadedUiDict.TryGetValue(uiType, out UiBase loadedUi))
            {
                return loadedUi.transform;
            }
            else
            {
                return null;
            }
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
            showDebugUI();
            //每隔30帧检测一次
            const int frameInterval = 30;
            if(UnityEngine.Time.frameCount % frameInterval != 0)
            {
                return;
            }
            destroyUis.Clear();
            //ui关闭后，如果没用的话20s后才卸载
            const float delayClose = 20f;
            foreach(KeyValuePair<Type, UiBase> kvPair in loadedUiDict)
            {
                if(kvPair.Value == null)
                {
                    continue;
                }

                if(kvPair.Value.IsDontDestroy)
                {
                    continue;
                }

                if(!kvPair.Value.bDynamicUnload)
                {
                    continue;
                }

                if(kvPair.Value.LastCloseTime == null)
                {
                    continue;
                }

                if(Time.time - kvPair.Value.LastCloseTime.Value < delayClose)
                {
                    continue;
                }

                if(manageStrategy.CheckOpenOrNot(kvPair.Value))
                {
                    continue;
                }

                destroyUis.Add(kvPair.Key);
                kvPair.Value.OnRuin();
                Destroy(kvPair.Value.gameObject);
                unloadAsset(kvPair.Value);
            }
            for(int i = 0, count = destroyUis.Count; i < count; ++i)
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
    }
}