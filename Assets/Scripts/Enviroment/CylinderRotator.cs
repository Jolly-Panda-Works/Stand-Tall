using JollyPanda.LastFlag.Handlers;
using System;
using UnityEngine;

namespace JollyPanda.LastFlag.EnviromentModule
{
    public class CylinderRotator : MonoBehaviour
    {
        [Header("Rotation Settings")]
        public float rotationSpeed = 100f;
        public float mouseSensitivity = 1f;
        public float touchSensitivity = 0.3f;
        public float smoothTime = 0.1f;

        [Header("Reset Settings")]
        public float resetDuration = 1f;

        private Vector2 lastMousePosition;
        private bool isActive = false;
        private Quaternion defaultRotation;

        private float targetRotationY = 0f;
        private float currentRotationY = 0f;
        private float rotationVelocity = 0f;

        private bool isResetting = false;
        private float resetTimer = 0f;
        private Quaternion startRotation;

        private void Awake()
        {
            defaultRotation = transform.rotation;
            currentRotationY = transform.eulerAngles.y;
            targetRotationY = currentRotationY;
        }

        private void OnEnable()
        {
            Informant.OnChangeUIPage += SwitchActive;
            Informant.OnFinish += Disable;
            Informant.OnWaveStart += Enable;
            Informant.OnPause += SwitchActive;
        }

        private void OnDisable()
        {
            Informant.OnChangeUIPage -= SwitchActive;
            Informant.OnFinish -= Disable;
            Informant.OnWaveStart -= Enable;
            Informant.OnPause -= SwitchActive;
        }

        private void SwitchActive(bool isActive)
        {
            this.isActive = !isActive;
        }

        private void Enable(int obj)
        {
            isActive = true;
        }

        private void Disable()
        {
            isActive = false;
        }

        private void SwitchActive(PageType type)
        {
            switch (type)
            {
                case PageType.HUD:
                    isActive = true;
                    break;
                case PageType.Home:
                    isActive = false;
                    ResetRotation();
                    break;
            }
        }

        private void Update()
        {
            if (isResetting)
            {
                resetTimer += Time.deltaTime;
                float t = Mathf.Clamp01(resetTimer / resetDuration);
                transform.rotation = Quaternion.Slerp(startRotation, defaultRotation, t);

                if (t >= 1f)
                {
                    isResetting = false;
                    currentRotationY = defaultRotation.eulerAngles.y;
                    targetRotationY = currentRotationY;
                }

                return;
            }

            if (!isActive)
                return;

            HandleInput();
            SmoothRotate();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0))
            {
                Vector2 currentMousePosition = Input.mousePosition;
                Vector2 delta = currentMousePosition - lastMousePosition;

                float sensitivity = Input.touchCount > 0 ? touchSensitivity : mouseSensitivity;
                float rotationDelta = -delta.x * rotationSpeed * sensitivity * Time.deltaTime;

                targetRotationY += rotationDelta;
                lastMousePosition = currentMousePosition;
            }
        }

        private void SmoothRotate()
        {
            currentRotationY = Mathf.SmoothDampAngle(currentRotationY, targetRotationY, ref rotationVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0f, currentRotationY, 0f);
        }

        public void ResetRotation()
        {
            isResetting = true;
            resetTimer = 0f;
            startRotation = transform.rotation;
        }
    }
}
