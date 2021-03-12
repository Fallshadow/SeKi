using UnityEngine;
using UnityEngine.UI;

namespace ASeKi.Demand
{
    public class CinemachineController_Test : MonoBehaviour
    {
        public CinemachineController_LookAround _controllerLookAround = null;
        public int TurnCameraID = 0;
        public int cameraID = 0;
        public int posID = 0;
        public Button setCameraPos = null;
        public Button setPos = null;

        private void Start()
        {
            setCameraPos.onClick.AddListener(() => { _controllerLookAround.SetFreeLookCameraPos(cameraID,posID);});
            setPos.onClick.AddListener(() => { _controllerLookAround.SetMainCineCamera(TurnCameraID);});
        }
    }
}

