using JollyPanda.LastFlag.Handlers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    public class GunSelector : MonoBehaviour
    {
        [SerializeField] private Gun[] guns;

        private Gun activeGun;

        public IReadOnlyList<Gun> Guns => guns;

        public event Action<Gun> OnGunChange;
        public event Action OnGunFire;

        private void Awake()
        {
            foreach (var gun in guns)
            {
                if (gun != null)
                    gun.Fired += RaiseGunFire;
            }
        }

        private void OnDestroy()
        {
            foreach (var gun in guns)
            {
                if (gun != null)
                    gun.Fired -= RaiseGunFire;
            }
        }

        private void RaiseGunFire() => OnGunFire?.Invoke();

        private void OnEnable()
        {
            Informant.OnStart += ActiveGun;
        }

        private void OnDisable()
        {
            Informant.OnStart -= ActiveGun;
        }

        private void Start()
        {
            // With the shop system in the scene, LoadoutManager selects the equipped gun.
            if (LoadoutManager.Instance != null)
                return;

            if (guns.Length > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, guns.Length);
                SelectGun(guns[randomIndex].Id);
            }
        }

        public void SelectGun(int gunID)
        {
            foreach (var gun in guns)
            {
                if (gun.Id == gunID)
                {
                    gun.gameObject.SetActive(true);

                    activeGun = gun;
                    //activeGun.SetActive(true);

                    OnGunChange?.Invoke(activeGun);
                }
                else
                {
                    gun.gameObject.SetActive(false);
                }
            }
        }

        private void ActiveGun()
        {
            if (activeGun == null)
                return;

            activeGun.SetActive(true);
        }

        internal void DisableGun()
        {
            if (activeGun == null)
                return;

            activeGun.SetActive(false);
            activeGun.DisableAllBullets();
        }

        internal void PauseShooting()
        {
            if (activeGun == null)
                return;

            activeGun.SetActive(false);
            activeGun.PauseBulletsMoving(true);
        }

        internal void ResumeShooting()
        {
            if (activeGun == null)
                return;

            activeGun.SetActive(true);
            activeGun.PauseBulletsMoving(false);
        }
    }
}