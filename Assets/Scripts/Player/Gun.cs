using System;
using UnityEngine;
using UnityEngine.Events;

namespace JollyPanda.LastFlag.PlayerModule
{
    public class Gun : MonoBehaviour
    {
        [field: SerializeField]
        public int Id { get; private set; }

        [field: SerializeField] public Transform FirePoint { get; private set; }

        [SerializeField] private BulletPool BulletPool;

        [SerializeField] private UnityEvent onFire;

        private GunSkinTarget skinTarget;

        private float fireRate = 0.5f;
        private int bulletDamage;
        private float bulletSize;
        private float bulletSpeed;

        private float timer;

        private bool isActive = false;

        /// <summary>Raised on every shot (C# event; GunSelector forwards it to Player for the fire sound).</summary>
        public event Action Fired;

        void Update()
        {
            if (!isActive)
                return;

            timer += Time.deltaTime;
            if (timer >= fireRate)
            {
                Fire();
                timer = 0f;
            }
        }

        void Fire()
        {
            BulletPool.GetBullet(bulletDamage, bulletSize, bulletSpeed, true, FirePoint.position, FirePoint.rotation);
            onFire?.Invoke();
            Fired?.Invoke();
        }

        internal void SetActive(bool isActive)
        {
            this.isActive = isActive;
        }

        internal void DisableAllBullets()
        {
            BulletPool.ReturnAllBullet();
        }

        internal void PauseBulletsMoving(bool isPause)
        {
            if (isPause)
            {
                BulletPool.PauseMoving();
            }
            else
            {
                BulletPool.ResumeMoving();
            }
        }

        public void SetFireRate(float fireRate)
        {
            this.fireRate = fireRate;
        }

        public void SetDamage(float damage)
        {
            bulletDamage = (int)damage;
        }

        /// <summary>Shop / customize: swap the gun material (null = original look).</summary>
        public void ApplySkin(Material skinMaterial)
        {
            if (skinTarget == null)
                skinTarget = GetComponent<GunSkinTarget>();

            if (skinTarget != null)
                skinTarget.Apply(skinMaterial);
        }

        /// <summary>Shop / customize: swap the bullet prefab (null = this gun's own default bullet).</summary>
        public void SetBulletPrefab(Bullet prefab)
        {
            BulletPool.SetBulletPrefab(prefab);
        }

        public void SetBulletSize(float scale)
        {
            bulletSize = scale;
        }

        public void SetBulletSpeed(float speed)
        {
            bulletSpeed = speed;
        }
    }
}