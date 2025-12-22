using JollyPanda.LastFlag.Handlers;
using System;
using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{

    public class Player : MonoBehaviour
    {
        [SerializeField] private GunSelector gunSelector;
        [SerializeField] private PlayerAnimationController animationController;
        [SerializeField] private PlayerSoundController soundController;

        private void OnEnable()
        {
            gunSelector.OnGunChange += GunChanged;
            gunSelector.OnGunFire += GunFire;
            Informant.OnLose += Losing;
            Informant.OnChangeUIPage += ChoicePlayerAnimation;
            Informant.OnPause += PauseAnything;
            Informant.OnFinish += Stop;
            Informant.OnWaveStart += Informant_OnWaveStart;
        }

        private void OnDisable()
        {
            gunSelector.OnGunChange -= GunChanged;
            gunSelector.OnGunFire -= GunFire;
            Informant.OnLose -= Losing;
            Informant.OnChangeUIPage -= ChoicePlayerAnimation;
            Informant.OnPause -= PauseAnything;
            Informant.OnFinish -= Stop;
            Informant.OnWaveStart -= Informant_OnWaveStart;
        }

        private void GunChanged(Gun activeGun)
        {
        }

        private void GunFire()
        {
            soundController.PlayFireSound();
        }

        private void Losing()
        {
            gunSelector.DisableGun();
            animationController.PlayDefeat();
            soundController.PlayDefeatSound();
        }

        private void ChoicePlayerAnimation(PageType type)
        {
            switch (type)
            {
                case PageType.HUD:
                    animationController.PlayShooting(true);
                    break;
                case PageType.Home:
                    animationController.PlayShooting(false);
                    animationController.PlayIdle();
                    gunSelector.DisableGun();
                    soundController.PlayIdleSound();
                    break;
            }
        }

        private void Informant_OnWaveStart(int obj)
        {
            animationController.PlayShooting(true);
            gunSelector.ResumeShooting();
        }

        private void Stop()
        {
            animationController.PlayShooting(false);
            animationController.PlayIdle();
            gunSelector.PauseShooting();
        }

        private void PauseAnything(bool isPause)
        {
            if (isPause)
            {
                animationController.Pause();
                gunSelector.PauseShooting();
            }
            else
            {
                animationController.Resume();
                gunSelector.ResumeShooting();
            }
        }
    }
}