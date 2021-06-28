using System;
using UnityEngine;
using UnityEngine.UI;

namespace ASeKi.ui
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public abstract class InteractableUiBase : UiBase
    {
        GraphicRaycaster graphicRaycaster = null;
        protected override void Reset()
        {
            base.Reset();
            graphicRaycaster = GetComponent<GraphicRaycaster>();
            SetInteractable(false);
        }
        public override void OnCreate()
        {
            base.OnCreate();
            graphicRaycaster = GetComponent<GraphicRaycaster>();
            SetInteractable(false);
        }
        public void SetInteractable(bool isInteractable)
        {
            graphicRaycaster.enabled = isInteractable;
        }
        protected override void onShowComplete()
        {
            base.onShowComplete();
            SetInteractable(true);
        }

        protected override void onShow()
        {
            SetInteractable(false);
        }
        protected override void onHideComplete()
        {
            base.onHideComplete();
            SetInteractable(false);
        }
        protected override void onHide()
        {
            SetInteractable(false);
        }
    }
}
