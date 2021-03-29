using ASeKi.evt;
using CCameraUtility;
using UnityEngine;
using UnityEngine.UI;

namespace ASeKi.Demand
{
    public class CinemachineController_Test : MonoBehaviour
    {
        public CCFreeLook _controllerLookAround = null;
        public CCFreeLookScene curScene = CCFreeLookScene.FashionScene;
        public InputField TurnCameraID = null;
        public Button setScene = null;
        public Button setPos = null;

        private void Start()
        {
            setScene.onClick.AddListener(() => { EventManager.instance.Send<CCFreeLookScene>(EventGroup.CAMERA, (short) CameraEvent.THREE_LAYER_CUT_SCENE, curScene); });
            setPos.onClick.AddListener(() => { _controllerLookAround.ChangeToXIdCamera(int.Parse(TurnCameraID.text)); });
        }
    }
}