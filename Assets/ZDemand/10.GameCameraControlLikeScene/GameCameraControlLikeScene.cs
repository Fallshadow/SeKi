using System;
using UnityEngine;
using DG.Tweening;

namespace ASeKi.Demand
{
    public class GameCameraControlLikeScene : MonoBehaviour
    {
        public Camera ControlCamera
        {
            get => controlCamera != null ? controlCamera : Camera.main;
            set => controlCamera = value;
        }

        private Camera controlCamera;

        public float CameraSpeed
        {
            get => Mathf.Clamp(cameraSpeed, 0.01f, 2.0f);
            set => cameraSpeed = value;
        }
        
        private float cameraSpeed = 1;
        
        [SerializeField] private float cameraPosSpeedMax = 10;
        [SerializeField] private float cameraPosSpeedMultiply = 0.1f;
        [SerializeField] private float cameraEurSpeedMultiply = 0.1f;
        private Tween upTween = null;
        private Tween downTween = null;
        private float upInput = 0;
        private float downInput = 0;
        
        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                bool doubleMultiply = Input.GetKey(KeyCode.LeftShift);
                controlSpeen();
                controlRot();
                controlByHoriAndVert(doubleMultiply);
                controlUpDown(doubleMultiply);
            }
            else
            {
                resetUpDown();
            }
        }

        private void addCameraPos(float x = 0, float y = 0, float z = 0, bool maxSpeed = false)
        {
            ControlCamera.transform.Translate((maxSpeed ? cameraPosSpeedMax : cameraPosSpeedMultiply * CameraSpeed) * new Vector3(x, y, z));
        }

        private void addCameraRot(float x = 0, float y = 0, float z = 0)
        {
            Transform controlCameraTransform = ControlCamera.transform;
            Vector3 beforeRot = controlCameraTransform.eulerAngles;
            Vector3 delRot = new Vector3(x, y, z);
            controlCameraTransform.eulerAngles = beforeRot + cameraEurSpeedMultiply * delRot;
        }

        private void controlSpeen()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Math.Abs(scrollInput) > 0)
            {
                CameraSpeed += scrollInput;
            }
        }
        
        private void controlRot()
        {
            float xInput = Input.GetAxis("Mouse X");
            float yInput = Input.GetAxis("Mouse Y");
            if (Math.Abs(xInput) > 0 || Math.Abs(yInput) > 0)
            {
                addCameraRot(-yInput, xInput);
            }
        }

        private void controlByHoriAndVert(bool doubleMultiply)
        {
            float hInput = Input.GetAxis("Horizontal");
            float vInput = Input.GetAxis("Vertical");
            
            if (Math.Abs(hInput) > 0 || Math.Abs(vInput) > 0)
            {
                addCameraPos(hInput, z: vInput, maxSpeed: doubleMultiply);
            }
        }
        
        private void controlUpDown(bool doubleMultiply)
        {
            if (Input.GetKey(KeyCode.Q))
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    downTween?.Kill();
                    downInput = 0;
                }
                else
                {
                    downTween = DOTween.To(() => downInput, x => downInput = x, -1, 1);
                }
            }
            else
            {
                downInput = 0;
            }
            if (Input.GetKey(KeyCode.E))
            {
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    upTween?.Kill();
                    upInput = 0;
                }
                else
                {
                    upTween = DOTween.To(() => upInput, x => upInput = x, 1, 1);
                }
            }
            else
            {
                upInput = 0;
            }
            addCameraPos(y: upInput + downInput, maxSpeed: doubleMultiply);
        }

        private void resetUpDown()
        {
            upInput = 0;
            downInput = 0;
            upTween.Kill();
            downTween.Kill();
        }
        
    }
}