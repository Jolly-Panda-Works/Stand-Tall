using JollyPanda.LastFlag.Handlers;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JollyPanda.LastFlag.EnviromentModule
{
    public class CylinderRotator : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [Tooltip("Degrees of rotation per pixel of finger/mouse movement (frame-rate independent).")]
        public float rotationDegreesPerPixel = 1.6f;
        public float mouseSensitivity = 1f;
        public float touchSensitivity = 0.3f;
        public float smoothTime = 0.1f;

        [Header("Reset Settings")]
        public float resetDuration = 1f;

        private Vector2 lastMousePosition;
        private bool pointerStartedOnUI;
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
            Informant.OnLose += Disable;
            Informant.OnWaveStart += Enable;
            Informant.OnPause += SwitchActive;
        }

        private void OnDisable()
        {
            Informant.OnChangeUIPage -= SwitchActive;
            Informant.OnFinish -= Disable;
            Informant.OnLose -= Disable;
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
                // a touch that starts on a button (pause, ...) must not rotate the cylinder
                pointerStartedOnUI = IsPointerOverUI();
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButton(0) && !pointerStartedOnUI)
            {
                Vector2 currentMousePosition = Input.mousePosition;
                Vector2 delta = currentMousePosition - lastMousePosition;

                float sensitivity = Input.touchCount > 0 ? touchSensitivity : mouseSensitivity;
                // delta is already "pixels per frame", so no Time.deltaTime here
                targetRotationY += -delta.x * rotationDegreesPerPixel * sensitivity;
                lastMousePosition = currentMousePosition;
            }
        }

        private static bool IsPointerOverUI()
        {
            if (EventSystem.current == null)
                return false;

            if (Input.touchCount > 0)
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);

            return EventSystem.current.IsPointerOverGameObject();
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
