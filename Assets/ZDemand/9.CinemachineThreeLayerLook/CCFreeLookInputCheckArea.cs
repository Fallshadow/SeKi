using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ASeKi.Demand
{
    public class CCFreeLookInputCheckArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            evt.EventManager.instance.Send<bool>(evt.EventGroup.CAMERA, (short)evt.CameraEvent.THREE_LAYER_LOOK_CINE, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            evt.EventManager.instance.Send<bool>(evt.EventGroup.CAMERA, (short)evt.CameraEvent.THREE_LAYER_LOOK_CINE, false);
        }
    }

}
