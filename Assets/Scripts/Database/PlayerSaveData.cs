using System;
using System.Collections.Generic;

namespace JollyPanda.LastFlag.Database
{
    [Serializable]
    public class PlayerSaveData
    {
        public int Money = 0;

        public int FireRateLevel = 0;
        public int DamageLevel = 0;
        public int BulletSizeLevel = 0;
        public int BulletSpeedLevel = 0;

        public int lastWaveIndex = -1;

        public float SfxVolume = 1f;
        public float BackgroundVolume = 1f;

        // Selected UI language (ULanguage.SharedTypes.LanguageType as int). -1 = not chosen yet.
        public int LanguageIndex = -1;

        // ---------- Shop / Customize ----------
        // Default (free) items are NOT stored here, only items the player bought.
        public List<string> OwnedItemIds = new();
        public string EquippedCharacterId = "";
        public string EquippedGunId = "";
        public string EquippedBulletId = "";
        // One entry per gun at most (see SaveSystem.SetEquippedGunSkin)
        public List<string> EquippedGunSkinIds = new();

        public PlayerSaveData() { }

        public PlayerSaveData(int money, int fireRateLevel, int damageLevel, int bulletSizeLevel, int bulletSpeedLevel, int lastWaveIndex, float sfxVolume, float backgroundVolume)
        {
            Money = money;
            FireRateLevel = fireRateLevel;
            DamageLevel = damageLevel;
            BulletSizeLevel = bulletSizeLevel;
            BulletSpeedLevel = bulletSpeedLevel;
            this.lastWaveIndex = lastWaveIndex;
            SfxVolume = sfxVolume;
            BackgroundVolume = backgroundVolume;
        }

        public int CurrentWaveIndex => lastWaveIndex + 1;

        /// <summary>Old save files (before the shop existed) do not contain the shop fields.</summary>
        public void EnsureShopData()
        {
            OwnedItemIds ??= new List<string>();
            EquippedGunSkinIds ??= new List<string>();
            EquippedCharacterId ??= "";
            EquippedGunId ??= "";
            EquippedBulletId ??= "";
        }

        public static PlayerSaveData Default => new(50, 0, 0, 0, 0, -1, 1f, 1f);
    }
}
