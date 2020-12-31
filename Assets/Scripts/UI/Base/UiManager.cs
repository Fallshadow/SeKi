using UnityEngine.EventSystems;
using UnityEngine;

namespace ASeKi.ui
{

    public partial class UiManager : SingletonMonoBehavior<UiManager>
    {
        public const int VisibleUiLayer = 5;        // unity默认UI层
        public const int InvisibleUiLayer = 6;      // unity默认不可更改的层，用作UI的隐藏显示
        public const int perspectiveLayer = 7;      // unity默认不可更改的层，用作UI的正交场景

        #region 相机相关

        public Camera UiCamera { get => uiCamera; }
        public Camera PhotoCamera { get => photoCamera; }
        public Camera PerspectiveCamera { get => perspectiveCamera; }


        [SerializeField] private Camera uiCamera = null;
        [SerializeField] private EventSystem eventSystem = null;
        [SerializeField] private Camera photoCamera = null; //专门用于拍照相机
        [SerializeField] private Camera perspectiveCamera = null;

        #endregion

        #region 适配相关
        #endregion


        [SerializeField] private RectTransform[] canvasRoots = null;

        private UiManageStrategy manageStrat = null;

        protected override void init()
        {
            manageStrat = new UiManageStrategy(canvasRoots);
        }

        private void Start()
        {
            // TODO: 启用动态加载
            manageStrat.InitPerspectiveRoot(perspectiveCamera);
        }

    }
}