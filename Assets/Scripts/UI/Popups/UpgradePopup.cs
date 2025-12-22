using System.Globalization;
using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.Handlers;
using JollyPanda.LastFlag.PlayerModule;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MJUtilities.UI
{
    public class UpgradePopup : UIPopup
    {
        [Header("Money")] 
        [SerializeField] private TMP_Text moneyText;

        [Header("Buttons")] 
        [SerializeField] private Button fireRateUpgradeButton;
        [SerializeField] private Button damageUpgradeButton;
        [SerializeField] private Button bulletSpeedUpgradeButton;
        [SerializeField] private Button bulletSizeUpgradeButton;
        
        [Header("BuyCostText")] 
        [SerializeField] private UpgradeProgressConfig fireRateUpgradeProgress;
        [SerializeField] private UpgradeProgressConfig damageUpgradeProgress;
        [SerializeField] private UpgradeProgressConfig bulletSpeedUpgradProgress;
        [SerializeField] private UpgradeProgressConfig bulletSizeUpgradeProgress;

        protected override void OnEnable()
        {
            base.OnEnable();
            Informant.OnProfileChange += ProfileChange;

            long currentMoney = SaveSystem.GetMoney();
            moneyText.text = FormatMoney(currentMoney);
        }

        private void OnDisable()
        {
            Informant.OnProfileChange -= ProfileChange;
        }
        public override void OnSetValues()
        {
            BackButtonConfig();
            
            fireRateUpgradeButton.onClick.AddListener(FireRateUpgradeButtonClicked);
            
            damageUpgradeButton.onClick.AddListener(DamageUpgradeButtonClicked);
            
            bulletSpeedUpgradeButton.onClick.AddListener(BulletSpeedUpgradeButtonClicked);
            
            bulletSizeUpgradeButton.onClick.AddListener(BulletSizeUpgradeButtonClicked);

            LoadCurrentUpgradeStatus();
        }

        private void OnDestroy()
        {
            fireRateUpgradeButton.onClick.RemoveListener(FireRateUpgradeButtonClicked);

            damageUpgradeButton.onClick.RemoveListener(DamageUpgradeButtonClicked);

            bulletSpeedUpgradeButton.onClick.RemoveListener(BulletSpeedUpgradeButtonClicked);

            bulletSizeUpgradeButton.onClick.RemoveListener(BulletSizeUpgradeButtonClicked);
        }

        private void BulletSizeUpgradeButtonClicked()
        {
            var canUpgrade = GunUpgradeManager.TryUpgradeBulletSize();
            if (canUpgrade)
            {
                var upgradeData = GunUpgradeManager.instance.GetBulletSizeUpgradeData();
                bulletSizeUpgradeProgress.HandleUpgrade(upgradeData);
            }

            LoadCurrentUpgradeStatus();
        }

        private void BulletSpeedUpgradeButtonClicked()
        {
            var canUpgrade = GunUpgradeManager.TryUpgradeBulletSpeed();
            if (canUpgrade)
            {
                var upgradeData = GunUpgradeManager.instance.GetBulletSpeedUpgradeData();
                bulletSpeedUpgradProgress.HandleUpgrade(upgradeData);
            }

            LoadCurrentUpgradeStatus();
        }

        private void DamageUpgradeButtonClicked()
        {
            var canUpgrade = GunUpgradeManager.TryUpgradeDamage();
            if (canUpgrade)
            {
                var upgradeData = GunUpgradeManager.instance.GetDamageUpgradeData();
                damageUpgradeProgress.HandleUpgrade(upgradeData);
            }

            LoadCurrentUpgradeStatus();
        }

        private void FireRateUpgradeButtonClicked()
        {
            var canUpgrade = GunUpgradeManager.TryUpgradeFireRate();
            if (canUpgrade)
            {
                var upgradeData = GunUpgradeManager.instance.GetFireRateUpgradeData();
                fireRateUpgradeProgress.HandleUpgrade(upgradeData);
            }

            LoadCurrentUpgradeStatus();
        }

        private void LoadCurrentUpgradeStatus()
        {
            var gunUpgradeManager = GunUpgradeManager.instance;
            long currentMoney = SaveSystem.GetMoney();

            var upgradeData = gunUpgradeManager.GetFireRateUpgradeData();
            fireRateUpgradeProgress.HandleUpgrade(upgradeData);
            fireRateUpgradeButton.interactable = upgradeData.NextLevelCost <= currentMoney && upgradeData.Level < 4;

            upgradeData = gunUpgradeManager.GetDamageUpgradeData();
            damageUpgradeProgress.HandleUpgrade(upgradeData);
            damageUpgradeButton.interactable = upgradeData.NextLevelCost <= currentMoney && upgradeData.Level < 4;

            upgradeData = gunUpgradeManager.GetBulletSpeedUpgradeData();
            bulletSpeedUpgradProgress.HandleUpgrade(upgradeData);
            bulletSpeedUpgradeButton.interactable = upgradeData.NextLevelCost <= currentMoney && upgradeData.Level < 4;

            upgradeData = gunUpgradeManager.GetBulletSizeUpgradeData();
            bulletSizeUpgradeProgress.HandleUpgrade(upgradeData);
            bulletSizeUpgradeButton.interactable = upgradeData.NextLevelCost <= currentMoney && upgradeData.Level < 4;
        }

        public override void OnAwake()
        {
            
        }
        
        private void ProfileChange(PlayerSaveData playerSaveData)
        {
            long currentMoney = playerSaveData.Money;
            moneyText.text = FormatMoney(currentMoney);
        }

        private string FormatMoney(long money)
        {
            return money.ToString("N0", CultureInfo.InvariantCulture);
        }

    }
}