using UnityEngine;
using UnityEngine.UI;

namespace ASeKi.Demand
{
    public class CinemachineController_Test : MonoBehaviour
    {
        public CCFreeLook _controllerLookAround = null;
        public InputField TurnCameraID = null;
        public InputField cameraID = null;
        public InputField posID = null;
        public Button setCameraPos = null;
        public Button setPos = null;

        private void Start()
        {
            setCameraPos.onClick.AddListener(() => { _controllerLookAround.SetFreeLookCameraPos(int.Parse(cameraID.text),int.Parse(posID.text));});
            setPos.onClick.AddListener(() => { _controllerLookAround.ChangeToXIdCamera(int.Parse(TurnCameraID.text));});
        }
    }
}

