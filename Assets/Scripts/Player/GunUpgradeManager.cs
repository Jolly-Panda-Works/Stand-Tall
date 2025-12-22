using JollyPanda.LastFlag.Database;
using System;
using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    public class GunUpgradeManager : MonoBehaviour
    {
        public static GunUpgradeManager instance;

        [SerializeField] private GunSelector gunSelector;

        [Header("Upgrade Configs")]
        [SerializeField] private UpgradeData fireRateUpgrade;
        [SerializeField] private UpgradeData damageUpgrade;
        [SerializeField] private UpgradeData bulletSizeUpgrade;
        [SerializeField] private UpgradeData bulletSpeedUpgrade;

        private Gun activeGun;

        public UpgradeData GetBulletSizeUpgradeData() => bulletSizeUpgrade;
        public UpgradeData GetBulletSpeedUpgradeData() => bulletSpeedUpgrade;
        public UpgradeData GetDamageUpgradeData() => damageUpgrade;
        public UpgradeData GetFireRateUpgradeData() => fireRateUpgrade;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
        }

        private void OnEnable()
        {
            gunSelector.OnGunChange += GunChanged;
        }

        private void OnDisable()
        {
            gunSelector.OnGunChange -= GunChanged;
        }

        private void GunChanged(Gun newGun)
        {
            activeGun = newGun;
            ApplyUpgrades();
        }

        private void Start()
        {
            ApplyUpgrades();
        }

        private void LoadPlayerData()
        {
            fireRateUpgrade.Level = SaveSystem.GetFireRateLevel();
            damageUpgrade.Level = SaveSystem.GetDamageLevel();
            bulletSizeUpgrade.Level = SaveSystem.GetBulletSizeLevel();
            bulletSpeedUpgrade.Level = SaveSystem.GetBulletSpeedLevel();
        }

        public void ApplyUpgrades()
        {
            LoadPlayerData();

            if (activeGun == null)
            {
                Debug.LogWarning("No active gun assigned to GunUpgradeManager.");
                return;
            }

            activeGun.SetFireRate(fireRateUpgrade.CurrentValue);
            activeGun.SetDamage(damageUpgrade.CurrentValue);
            activeGun.SetBulletSize(bulletSizeUpgrade.CurrentValue);
            activeGun.SetBulletSpeed(bulletSpeedUpgrade.CurrentValue);
        }

        public static bool TryUpgradeFireRate()
        {
            return instance.TryUpgrade(instance.fireRateUpgrade, SaveSystem.UpdateFireRateLevel);
        }

        public static bool TryUpgradeDamage()
        {
            return instance.TryUpgrade(instance.damageUpgrade, SaveSystem.UpdateDamageLevel);
        }

        public static bool TryUpgradeBulletSpeed()
        {
            return instance.TryUpgrade(instance.bulletSpeedUpgrade, SaveSystem.UpdateBulletSpeedLevel);
        }

        public static bool TryUpgradeBulletSize()
        {
            return instance.TryUpgrade(instance.bulletSizeUpgrade, SaveSystem.UpdateBulletSizeLevel);
        }

        private bool TryUpgrade(UpgradeData upgrade, Action<int> saveLevelAction)
        {
            int playerMoney = SaveSystem.GetMoney();

            if (!upgrade.CanUpgrade || playerMoney < upgrade.NextLevelCost)
                return false;

            playerMoney -= upgrade.NextLevelCost;
            upgrade.Level++;
            saveLevelAction(upgrade.Level);
            SaveSystem.UpdateMoney(playerMoney);

            ApplyUpgrades();
            return true;
        }
    }
}
