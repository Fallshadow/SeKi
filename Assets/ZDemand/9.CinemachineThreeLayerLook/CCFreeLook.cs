using System.Collections;
using System;
using System.Collections.Generic;
using ASeKi.Extension;
using ASeKi.evt;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

namespace CCameraUtility
{
    [Serializable]
    public sealed class CameraID_PosID_Dictionary : SerializedDictionary<int,int> {}
    
    public class CCFreeLook : MonoBehaviour
    {
        [SerializeField] private CinemachineBrain mainCamera = null;
        [SerializeField] private CinemachineMixingCamera mixingCamera = null;
        [SerializeField] private Transform freelookPosRoot = null;
        [SerializeField] private Transform freelookCameraRoot = null;
        [SerializeField] private CinemachineFreeLook[] freeLookGroup = new CinemachineFreeLook[0];
        [SerializeField] private GameObject[] followObjsY;
        private List<Transform> allfreeLookLookAtTran = new List<Transform>();
        private List<CinemachineFreeLook> allfreeLookGroup = new List<CinemachineFreeLook>();
        private Vector3 cameraRotation;

        [Header("编辑器参数设置")]
        [SerializeField] private float scrollSpeed = 10;
        [SerializeField] private float rotateXSpeed = 2f;
        [SerializeField] private float rotateYSpeed = 5f;
        [SerializeField] private float scrollTime = 1f;
        
        [Header("安卓参数设置")]
        [SerializeField] private float scrollSpeedForPhone = 700;
        [SerializeField] private float rotateXSpeedForPhone = 2.2f;
        [SerializeField] private float rotateYSpeedForPhone = 5.5f;
        [SerializeField] private float scrollTimeForPhone = 0.3f;
        Vector3 oldPos1 = Vector3.zero;
        Vector3 oldPos2 = Vector3.zero;
        
        [Header("通用限制")]
        [SerializeField] private float angleLimit = 13;
        
        [Header("切换相机位置的缓动时间")]
        [SerializeField] private float durTime = 1;
        [SerializeField] private float startCineTime = 0.2f;
        private float durTimeCheck = -1;

        [Header("放大缩小的缓动参数")]
        [SerializeField] private float scrollValue = 0;
        [SerializeField] private Tween scrollTween = null;

        [Header("权重忍受范围")]
        [SerializeField] private float toleRange = 0.01f;

        [Header("TODO:改为byte读取")] 
        public CCFreeLookCameraPointConfigSO AllCameraPosDatas = null;
        private List<CCFreeLookCameraPos> CurCameraPosDatas = null;
        
        private float timer = 0;
        private Tween tweener = null;
        private bool mouseDown = false;
        private bool isAutoMove = false;
        private int maxCameraNum = 3;
        
        static readonly int activePriority = 12;
        static readonly int mixingPriority = 11;
        static readonly int negativePriority = 10;
        
        private void Start()
        {
            cameraRotation = transform.rotation.eulerAngles;
            initCineMachine();
            durTimeCheck = mainCamera.m_DefaultBlend.m_Time;
            ChangeMainCameraLerpTime();
            EventManager.instance.Register<bool>(EventGroup.CAMERA, (short)CameraEvent.THREE_LAYER_LOOK_CINE, CheckMouse);
            EventManager.instance.Register<CCFreeLookScene>(EventGroup.CAMERA, (short)CameraEvent.THREE_LAYER_CUT_SCENE, CutScene);
            StartCoroutine("enableCinemachine");
        }
        
        void OnDestroy()
        {            
            EventManager.instance.Unregister<bool>(EventGroup.CAMERA, (short)CameraEvent.THREE_LAYER_LOOK_CINE, CheckMouse);
            EventManager.instance.Unregister<CCFreeLookScene>(EventGroup.CAMERA, (short)CameraEvent.THREE_LAYER_CUT_SCENE, CutScene);
            CancelInvoke("delayApply");
        }

        public void CutScene(CCFreeLookScene ccFreeLookScene)
        {
            CurCameraPosDatas = AllCameraPosDatas.GetCCFreeLookCameraPosDatas(ccFreeLookScene);
            maxCameraNum = AllCameraPosDatas.ccSceneMaxLayer[(int)ccFreeLookScene];
            freeLookGroup = new CinemachineFreeLook[maxCameraNum];
            for (int index = 0; index < maxCameraNum; index++)
            {
                foreach (var item in CurCameraPosDatas)
                {
                    if (item.PosId == index)
                    {
                        setFreeLookCameraPos(item.CameraIndex,item.PosId);
                        break;
                    }
                }
                ASeKi.debug.PrintSystem.LogError("[CCFreeLook] 当前场景没有为所有的层数配备摄像机，无法初始化，请检查CCFreeLookCameraPointConfigSO");
            }
        }

        // 将X号相机安排在X号位置上
        private void setFreeLookCameraPos(int cameraId, int cameraPos)
        {
            if(mixingCamera == null)
            {
                return;
            }
            if(freeLookGroup[cameraPos] != null)
            {
                Destroy(freeLookGroup[cameraPos].gameObject);
            }
            freeLookGroup[cameraPos] = Instantiate(allfreeLookGroup[cameraId], mixingCamera.transform);
            freeLookGroup[cameraPos].gameObject.SetActive(true);
        }
        
        // 切换到XID的相机
        public void ChangeToXIdCamera(int cameraId,Action callBack = null)
        {
            SetAllFreeLookGroupNegative();
            allfreeLookGroup[cameraId].Priority = activePriority;
            if(tweener != null)
            {
                tweener.Kill();
            }
            tweener = DOTween.To(() => timer, x => timer = x, 1, durTime).OnComplete(
                () =>
                {
                    setFreeLookGroupItemByID(cameraId);
                    allfreeLookGroup[cameraId].Priority = negativePriority;
                    applyFreeLookCineMachine = freeLookGroup[CurCameraPosDatas[cameraId].PosId];
                    callBack?.Invoke();
                    Invoke("delayApply", Time.deltaTime);
                }
            );
        }

        #region 切换到某个位置下的相机位置

        // 暂存下
        private CinemachineFreeLook applyFreeLookCineMachine = null;

        #region 直接切换到某个位置的后续补正

        private void delayApply()
        {
            cleanMixingCamera();
            mixingCamera.SetWeight(applyFreeLookCineMachine, 1);
            foreach(var camera in freeLookGroup)
            {
                camera.m_XAxis.Value = applyFreeLookCineMachine.m_XAxis.Value;
                camera.m_YAxis.Value = applyFreeLookCineMachine.m_YAxis.Value;
            }
            isAutoMove = false;

            CinemachineFreeLook tFreeLook = Instantiate(freeLookGroup[1]);
            Destroy(tFreeLook.gameObject);
        }
        
        private void SetAllFreeLookGroupNegative()
        {
            foreach(var camera in allfreeLookGroup)
            {
                camera.Priority = negativePriority;
            }
        }
        
        // 清空混合相机权重
        private void cleanMixingCamera()
        {
            for(int cameraIndex = 0; cameraIndex < mixingCamera.ChildCameras.Length; cameraIndex++)
            {
                mixingCamera.SetWeight(cameraIndex, 0);
            }
        }
        
        #endregion
        
        // TODO: 根据摄像机的ID决定该摄像机应有的位置，这部分应该由配置决定，这边写死下
        private void setFreeLookGroupItemByID(int cameraID)
        {
            setFreeLookCameraPos(cameraID,CurCameraPosDatas[cameraID].PosId);
        }
        
        #endregion

        #region 关于相机的移动

        // 跟随相机的物件运动逻辑
        private void SetFollowObjectRot()
        {
            float delY = transform.rotation.eulerAngles.y - cameraRotation.y;
            float delX = transform.rotation.eulerAngles.x - cameraRotation.x;
            foreach(var obj in followObjsY)
            {
                Vector3 objRotation = obj.transform.rotation.eulerAngles;
                objRotation.y = objRotation.y + delY;
                obj.transform.rotation = Quaternion.Euler(objRotation);
            }
        }
        
        // 主体控制逻辑
        private void Update()
        {
            SetFollowObjectRot();
            cameraRotation = transform.rotation.eulerAngles;

#if(UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
            var mouse_x = 0f;
            var mouse_y = 0f;
            if(Input.touchCount == 1)
            {
                Touch phoneTouch = Input.touches[0];
                mouse_x = -phoneTouch.deltaPosition.x;
                mouse_y = -phoneTouch.deltaPosition.y;
            }

            if(mouseDown && !isAutoMove)
            {
                moveMixingCameraGroup(mouse_x / 12.5f * rotateXSpeedForPhone, mouse_y / 1250f * rotateYSpeedForPhone);
            }
            else if(!isAutoMove)
            {
                moveMixingCameraGroup(0, 0);
            }

            if(Input.touchCount > 1)
            {
                Touch touch1 = Input.touches[0];
                Touch touch2 = Input.touches[1];
                Vector3 tempPos1 = touch1.position;
                Vector3 tempPos2 = touch2.position;
                if(touch2.phase == TouchPhase.Began)
                {
                    oldPos1 = tempPos1;
                    oldPos2 = tempPos2;
                    return;
                }
                float disTemp = Vector2.Distance(tempPos1, tempPos2);
                float disOld = Vector2.Distance(oldPos1, oldPos2);
                float del = disTemp - disOld;//正表示放大，负表示缩小
                if(del != 0 && !isAutoMove)
                {
                    float scrollNum = del;
                    scrollNum *= Time.deltaTime / 6250 * scrollSpeedForPhone;
                    debug.PrintSystem.Log(scrollNum.ToString());
                    settingScrollWeightDur(scrollNum);
                }
                oldPos1 = tempPos1;
                oldPos2 = tempPos2;
            }
#endif

#if UNITY_EDITOR
            // 旋转操作
            var mouse_x = -Input.GetAxis("Mouse X");
            var mouse_y = -Input.GetAxis("Mouse Y");
            if(mouseDown && !isAutoMove)
            {
                moveMixingCameraGroup(mouse_x * (rotateXSpeed / 10000f), mouse_y * (rotateYSpeed / 1000000f));
            }
            else if(!isAutoMove)
            {
                moveMixingCameraGroup(0, 0);
            }
            // 缩放操作
            if(Input.GetAxis("Mouse ScrollWheel") != 0 && !isAutoMove)
            {
                float scrollNum = Input.GetAxis("Mouse ScrollWheel");
                scrollNum *= Time.deltaTime * scrollSpeed;
                ASeKi.debug.PrintSystem.Log(scrollNum.ToString());
                settingScrollWeightDur(scrollNum);
            }
#endif
            if(scrollValue != 0)
            {
                settingScrollWeight(-scrollValue);
            }
            ChangeMainCameraLerpTime();
        }
        
        // 根据输入的X、Y移动此时在混合相机中拥有权重的虚拟相机
        private void moveMixingCameraGroup(float x, float y)
        {
            float XValue = 0;
            float YValue = 0.5f;
            foreach(var freeLookCamera in freeLookGroup)
            {
                if(freeLookCamera == null)
                {
                    continue;
                }

                if(mixingCamera.GetWeight(freeLookCamera) != 0)
                {
                    freeLookCamera.m_XAxis.m_InputAxisValue = x;
                    XValue = freeLookCamera.m_XAxis.Value;
                    freeLookCamera.m_YAxis.m_InputAxisValue = y;
                    YValue = freeLookCamera.m_YAxis.Value;
                }
            }
            foreach(var freeLookCamera in freeLookGroup)
            {
                if(freeLookCamera == null)
                {
                    continue;
                }

                if(mixingCamera.GetWeight(freeLookCamera) == 0)
                {
                    freeLookCamera.m_XAxis.Value = XValue;
                    freeLookCamera.m_YAxis.Value = YValue;
                }
            }
        }
        
        // 配置滚轮移动权重
        private void settingScrollWeight(float value)
        {
            for(int cIndex = 0; cIndex < freeLookGroup.Length; cIndex++)
            {
                float weight = mixingCamera.GetWeight(freeLookGroup[cIndex]);
                if(weight == 1)
                {
                    if(value > 0)
                    {
                        // 最外层不可能向外
                        if(cIndex == freeLookGroup.Length - 1)
                        {
                            return;
                        }
                        float distance = Vector3.Distance(freeLookGroup[cIndex].transform.position, freeLookGroup[cIndex + 1].transform.position);
                        mixingCamera.SetWeight(freeLookGroup[cIndex], Mathf.Clamp01(weight - value / distance));
                        mixingCamera.SetWeight(freeLookGroup[cIndex + 1], Mathf.Clamp01(mixingCamera.GetWeight(freeLookGroup[cIndex + 1]) + value / distance));
                    }
                    if(value < 0)
                    {
                        // 最内层不可能向内
                        if(cIndex == 0)
                        {
                            return;
                        }
                        float distance = Vector3.Distance(freeLookGroup[cIndex - 1].transform.position, freeLookGroup[cIndex].transform.position);
                        mixingCamera.SetWeight(freeLookGroup[cIndex], Mathf.Clamp01(weight + value / distance));
                        mixingCamera.SetWeight(freeLookGroup[cIndex - 1], Mathf.Clamp01(mixingCamera.GetWeight(freeLookGroup[cIndex - 1]) - value / distance));
                    }
                    return;
                }
                if(weight > 0)
                {
                    float distance = Vector3.Distance(freeLookGroup[cIndex + 1].transform.position, freeLookGroup[cIndex].transform.position);
                    mixingCamera.SetWeight(freeLookGroup[cIndex], Mathf.Clamp01(weight - value / distance));
                    mixingCamera.SetWeight(freeLookGroup[cIndex + 1], Mathf.Clamp01(mixingCamera.GetWeight(freeLookGroup[cIndex + 1]) + value / distance));
                    return;
                }
            }
        }
        
        // 根据滚轮/手指输入确定滑动权重值
        private void settingScrollWeightDur(float value)
        {
            if(scrollTween != null)
            {
                scrollTween.Kill();
            }
#if(UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
            scrollTween = DOTween.To(() => scrollValue, x => scrollValue = x, value, scrollTimeForPhone / 2).OnComplete(
                () =>
                {
                    scrollTween = DOTween.To(() => scrollValue, x => scrollValue = x, 0, scrollTimeForPhone / 2).OnComplete(
                        () =>
                        {
                            scrollValue = 0;
                        });
                });
#endif

#if UNITY_EDITOR
            scrollTween = DOTween.To(() => scrollValue, x => scrollValue = x, value, scrollTime / 2).OnComplete(
                () =>
                {
                    scrollTween = DOTween.To(() => scrollValue, x => scrollValue = x, 0, scrollTime / 2).OnComplete(
                        () =>
                        {
                            scrollValue = 0;
                        });
                });
#endif
        }
        
        #endregion

        #region 关于初始化配置

        // 配置相机缓动的时间
        private void ChangeMainCameraLerpTime()
        {
            if(durTimeCheck != durTime)
            {
                mainCamera.m_DefaultBlend.m_Time = durTime;
                durTimeCheck = durTime;
            }
        }

        // 初始化各个相机参数：应用角度限制、设置相机的lookat和follow、调节混合相机的权重
        private void initCineMachine()
        {
            allfreeLookGroup.Clear();
            allfreeLookLookAtTran.Clear();
            foreach (Transform trans in freelookCameraRoot.transform)
            {
                allfreeLookGroup.Add(trans.GetComponent<CinemachineFreeLook>());
            }
            foreach (Transform trans in freelookPosRoot.transform)
            {
                allfreeLookLookAtTran.Add(trans);
            }
            foreach(var camera in allfreeLookGroup)
            {
                ChangeOrbitByCenterRadius(camera, angleLimit, camera.m_Orbits[1].m_Radius, 0);
            }
            for(int cameraIndex = 0; cameraIndex < allfreeLookGroup.Count; cameraIndex++)
            {
                allfreeLookGroup[cameraIndex].m_LookAt = allfreeLookLookAtTran[cameraIndex];
                allfreeLookGroup[cameraIndex].m_Follow = allfreeLookLookAtTran[cameraIndex];
            }
            mixingCamera.Priority = mixingPriority;
        }

        // 应用角度限制
        private void ChangeOrbitByCenterRadius(CinemachineFreeLook camera, float angle, float radius, float height)
        {
            float cosValue = Mathf.Cos(angle * Mathf.PI / 180);
            float sinValue = Mathf.Sin(angle * Mathf.PI / 180);
            camera.m_Orbits[1].m_Height = height;
            camera.m_Orbits[1].m_Radius = radius;
            camera.m_Orbits[0].m_Height = sinValue * radius;
            camera.m_Orbits[0].m_Radius = cosValue * radius;
            camera.m_Orbits[2].m_Height = -sinValue * radius;
            camera.m_Orbits[2].m_Radius = cosValue * radius;
            camera.m_XAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            camera.m_XAxis.m_MaxSpeed = 1;
            camera.m_YAxis.m_SpeedMode = AxisState.SpeedMode.InputValueGain;
            camera.m_YAxis.m_MaxSpeed = 1;
        }

        // 接受按下输入
        private void CheckMouse(bool mouseDown)
        {
            this.mouseDown = mouseDown;
        }
        
        // 防止其他虚拟相机顶替
        IEnumerator enableCinemachine()
        {
            yield return new WaitForSeconds(startCineTime);
            mainCamera.enabled = true;
            yield return null;
        }
        
        #endregion
        
#if UNITY_EDITOR
        private static int maxCameraNumEditor = 5;
        [SerializeField] private Color[] freeLookColors = new Color[maxCameraNumEditor];
        [SerializeField] private float radius = 0.5f;

        private void OnDrawGizmos()
        {
            for(int cameraIndex = 0; cameraIndex < freeLookGroup.Length; cameraIndex++)
            {
                if(freeLookGroup[cameraIndex] == null)
                {
                    continue;
                }
                Gizmos.color = freeLookColors[cameraIndex];
                Gizmos.DrawLine(freeLookGroup[cameraIndex].transform.position, freeLookGroup[cameraIndex].m_LookAt.position);
                Gizmos.DrawSphere(freeLookGroup[cameraIndex].transform.position, radius);
                Gizmos.DrawSphere(freeLookGroup[cameraIndex].m_LookAt.position, radius);
            }
        }
#endif
    }
}

