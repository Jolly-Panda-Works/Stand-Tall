using JollyPanda.LastFlag.EnviromentModule;
using System.Collections.Generic;
using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    [System.Serializable]
    public class BulletPool
    {
        public Bullet bulletPrefab;
        public Transform parent;
        private readonly List<Bullet> pool = new();

        private Bullet defaultPrefab;
        private bool defaultCaptured;

        /// <summary>Changes the prefab used for new bullets. null = go back to the prefab assigned in the inspector.</summary>
        public void SetBulletPrefab(Bullet prefab)
        {
            if (!defaultCaptured)
            {
                defaultPrefab = bulletPrefab;
                defaultCaptured = true;
            }

            var target = prefab != null ? prefab : defaultPrefab;
            if (target == bulletPrefab)
                return;

            // pooled bullets belong to the old prefab -> throw them away
            foreach (var bullet in pool)
            {
                if (bullet != null)
                    Object.Destroy(bullet.gameObject);
            }
            pool.Clear();

            bulletPrefab = target;
        }

        public Bullet GetBullet(int damage, float scale, float speed,bool isActive,Vector3 position,Quaternion rotation)
        {
            foreach (var bullet in pool)
            {
                if (!bullet.gameObject.activeInHierarchy)
                {
                    bullet.Initialize(damage, scale, speed);
                    bullet.transform.SetPositionAndRotation(position, rotation);
                    bullet.gameObject.SetActive(true);
                    bullet.CanMoving = isActive;
                    return bullet;
                }
            }

            var newBullet = Object.Instantiate(bulletPrefab, SpawnPoint.Instance.transform);
            pool.Add(newBullet);
            newBullet.Initialize(damage, scale, speed);
            newBullet.transform.SetPositionAndRotation(position, rotation);
            newBullet.CanMoving = isActive;
            newBullet.gameObject.SetActive(true);
            return newBullet;
        }

        public void ReturnBullet(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
        }

        public void ReturnAllBullet()
        {
            foreach (var bullet in pool)
            {
                bullet.gameObject.SetActive(false);
            }
        }

        internal void PauseMoving()
        {
            foreach (var bullet in pool)
            {
                bullet.CanMoving = false;
            }
        }

        internal void ResumeMoving()
        {
            foreach (var bullet in pool)
            {
                bullet.CanMoving = true;
            }
        }
    }
}