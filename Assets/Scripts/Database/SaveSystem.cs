using System;

namespace JollyPanda.LastFlag.Database
{
    public static class SaveSystem
    {
        private const string SaveKey = "PlayerData";

        public static event Action<PlayerSaveData> OnApplyChange;

        private static PlayerSaveData cachedData;

        static SaveSystem()
        {
            cachedData = Load();
        }

        private static void Save(PlayerSaveData data)
        {
            cachedData = data;
            ES3.Save(SaveKey, data);
            OnApplyChange?.Invoke(data);
        }

        private static PlayerSaveData Load()
        {
            if (cachedData != null)
                return cachedData;

            cachedData = ES3.KeyExists(SaveKey)
                ? ES3.Load<PlayerSaveData>(SaveKey)
                : PlayerSaveData.Default;

            return cachedData;
        }

        // ---------- Getter Methods ----------

        public static int GetMoney() => Load().Money;
        public static int GetFireRateLevel() => Load().FireRateLevel;
        public static int GetDamageLevel() => Load().DamageLevel;
        public static int GetBulletSizeLevel() => Load().BulletSizeLevel;
        public static int GetBulletSpeedLevel() => Load().BulletSpeedLevel;
        public static int GetLastWaveIndex() => Load().lastWaveIndex;
        public static int GetCurrentWaveIndex() => Load().CurrentWaveIndex;
        public static float GetSfxVolume() => Load().SfxVolume;
        public static float GetBackgroundVolume() => Load().BackgroundVolume;

        // ---------- Updater Methods ----------

        public static void UpdateMoney(int newMoney)
        {
            var data = Load();
            data.Money = newMoney;
            Save(data);
        }

        public static void AddMoney(int amount)
        {
            var data = Load();
            data.Money += amount;
            Save(data);
        }

        public static void UpdateFireRateLevel(int level)
        {
            var data = Load();
            data.FireRateLevel = level;
            Save(data);
        }

        public static void UpdateDamageLevel(int level)
        {
            var data = Load();
            data.DamageLevel = level;
            Save(data);
        }

        public static void UpdateBulletSizeLevel(int level)
        {
            var data = Load();
            data.BulletSizeLevel = level;
            Save(data);
        }

        public static void UpdateBulletSpeedLevel(int level)
        {
            var data = Load();
            data.BulletSpeedLevel = level;
            Save(data);
        }

        public static void UpdateLastWaveIndex(int index)
        {
            var data = Load();
            data.lastWaveIndex = index;
            Save(data);
        }

        public static void UpdateSfxVolume(float volume)
        {
            var data = Load();
            data.SfxVolume = volume;
            Save(data);
        }

        public static void UpdateBackgroundVolume(float volume)
        {
            var data = Load();
            data.BackgroundVolume = volume;
            Save(data);
        }

        // ---------- Reset Method ----------

        public static void ResetData()
        {
            Save(PlayerSaveData.Default);
        }
    }
}
