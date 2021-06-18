using UnityEngine;

namespace ASeKi.ui
{
    public abstract class FullScreenCanvasBase : InteractableUiBase
    {
        public override bool bDynamicUnload => true;
        public override UiOpenType OpenType { get { return UiOpenType.UOT_FULL_SCREEN; } }

        protected virtual void SetCanvasTransform()
        {
            RectTransform rt = transform as RectTransform;
            // UiManager.instance.SetUiAdaptation(rt);  适配
        }
    }
}
