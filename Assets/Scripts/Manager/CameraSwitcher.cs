using Cinemachine;
using JollyPanda.LastFlag.Handlers;
using System;
using UnityEngine;

namespace JollyPanda.LastFlag.Manager
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cameraHome;
        [SerializeField] private CinemachineVirtualCamera cameraGame;

        private void Start()
        {
            ActivateHomeCamera();
        }

        private void OnEnable()
        {
            Informant.OnChangeUIPage += Switch;
        }

        private void OnDisable()
        {
            Informant.OnChangeUIPage += Switch;
        }

        private void Switch(PageType type)
        {
            switch (type)
            {
                case PageType.HUD:
                    ActivateGameCamera();
                    break;
                case PageType.Home:
                    ActivateHomeCamera();
                    break;
            }
        }

        public void ActivateHomeCamera()
        {
            cameraHome.Priority = 10;
            cameraGame.Priority = 5;
        }

        public void ActivateGameCamera()
        {
            cameraHome.Priority = 5;
            cameraGame.Priority = 10;
        }
    }
}