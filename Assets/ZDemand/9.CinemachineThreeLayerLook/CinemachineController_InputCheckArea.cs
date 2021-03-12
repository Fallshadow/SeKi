using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ASeKi.Demand
{
    public class CinemachineController_InputCheckArea : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
    {
        public void OnPointerDown(PointerEventData eventData)
        {
            evt.EventManager.instance.Send<bool>(evt.EventGroup.CAMERA, (short)evt.CameraEvent.THREE_LAYER_LOOK_CINE, true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            evt.EventManager.instance.Send<bool>(evt.EventGroup.CAMERA, (short)evt.CameraEvent.THREE_LAYER_LOOK_CINE, false);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
