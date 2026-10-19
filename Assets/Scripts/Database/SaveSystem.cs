using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JollyPanda.LastFlag.Database
{
    public static class SaveSystem
    {
        private const string SaveKey = "PlayerData";

        // The encrypted profile lives in its OWN file, so the old plain-text save (default ES3 file)
        // can still be read once and migrated, without ES3 trying to decrypt a plain file.
        private const string EncryptedFileName = "lf_profile.dat";

        // Obfuscated (XOR) so the password is not a readable string inside the build.
        // CHANGE these bytes for your own game. This only stops casual editing of the save file.
        private static readonly byte[] KeyMask =
        {
            0x16, 0x3b, 0x29, 0x2e, 0x1c, 0x36, 0x3b, 0x3d, 0x79, 0x10, 0x35, 0x36,
            0x36, 0x23, 0x0a, 0x3b, 0x34, 0x3e, 0x3b, 0x7b, 0x31, 0x63, 0x00, 0x2b
        };

        private static ES3Settings encryptedSettings;

        private static ES3Settings EncryptedSettings
        {
            get
            {
                if (encryptedSettings == null)
                {
                    var raw = new byte[KeyMask.Length];
                    for (int i = 0; i < raw.Length; i++)
                        raw[i] = (byte)(KeyMask[i] ^ 0x5A);

                    string password = Encoding.UTF8.GetString(raw) + Application.identifier;
                    encryptedSettings = new ES3Settings(EncryptedFileName, ES3.EncryptionType.AES, password);
                }

                return encryptedSettings;
            }
        }

        public static event Action<PlayerSaveData> OnApplyChange;

        private static PlayerSaveData cachedData;

        static SaveSystem()
        {
            cachedData = Load();
        }

        private static void Save(PlayerSaveData data)
        {
            cachedData = data;
            ES3.Save(SaveKey, data, EncryptedSettings);
            OnApplyChange?.Invoke(data);
        }

        private static PlayerSaveData Load()
        {
            if (cachedData != null)
                return cachedData;

            try
            {
                if (ES3.KeyExists(SaveKey, EncryptedSettings))
                {
                    cachedData = ES3.Load<PlayerSaveData>(SaveKey, EncryptedSettings);
                }
                else if (ES3.KeyExists(SaveKey))
                {
                    // Migration: old plain-text save -> encrypted file, then remove the plain copy
                    cachedData = ES3.Load<PlayerSaveData>(SaveKey);
                    ES3.Save(SaveKey, cachedData, EncryptedSettings);
                    ES3.DeleteKey(SaveKey);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Save file unreadable, starting fresh: {e.Message}");
            }

            cachedData ??= PlayerSaveData.Default;
            cachedData.EnsureShopData();
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
        public static int GetLanguageIndex() => Load().LanguageIndex;

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

        public static void UpdateLanguageIndex(int index)
        {
            var data = Load();
            if (data.LanguageIndex == index)
                return;

            data.LanguageIndex = index;
            Save(data);
        }

        // ---------- Shop / Inventory ----------

        public static bool HasItem(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && Load().OwnedItemIds.Contains(itemId);
        }

        public static IReadOnlyList<string> GetOwnedItemIds() => Load().OwnedItemIds;

        /// <summary>Pays and adds the item in ONE save (no state where coins are gone but the item is missing).</summary>
        public static bool TryPurchaseItem(string itemId, int price)
        {
            var data = Load();

            if (string.IsNullOrEmpty(itemId) || price < 0)
                return false;
            if (data.OwnedItemIds.Contains(itemId))
                return false;
            if (data.Money < price)
                return false;

            data.Money -= price;
            data.OwnedItemIds.Add(itemId);
            Save(data);
            return true;
        }

        public static string GetEquippedCharacterId() => Load().EquippedCharacterId;
        public static string GetEquippedGunId() => Load().EquippedGunId;
        public static string GetEquippedBulletId() => Load().EquippedBulletId;
        public static IReadOnlyList<string> GetEquippedGunSkinIds() => Load().EquippedGunSkinIds;

        public static void SetEquippedCharacterId(string id)
        {
            var data = Load();
            if (data.EquippedCharacterId == id)
                return;
            data.EquippedCharacterId = id;
            Save(data);
        }

        public static void SetEquippedGunId(string id)
        {
            var data = Load();
            if (data.EquippedGunId == id)
                return;
            data.EquippedGunId = id;
            Save(data);
        }

        public static void SetEquippedBulletId(string id)
        {
            var data = Load();
            if (data.EquippedBulletId == id)
                return;
            data.EquippedBulletId = id;
            Save(data);
        }

        /// <param name="sameGunSkinIds">Ids of every skin of the same gun; they are removed so only one skin per gun stays equipped.</param>
        public static void SetEquippedGunSkin(string skinId, IEnumerable<string> sameGunSkinIds)
        {
            var data = Load();
            var list = data.EquippedGunSkinIds;

            if (list.Contains(skinId))
            {
                bool onlyOne = true;
                foreach (var other in sameGunSkinIds)
                {
                    if (other != skinId && list.Contains(other))
                        onlyOne = false;
                }
                if (onlyOne)
                    return;
            }

            foreach (var other in sameGunSkinIds)
                list.Remove(other);

            list.Add(skinId);
            Save(data);
        }

        // ---------- Reset Method ----------

        public static void ResetData()
        {
            Save(PlayerSaveData.Default);
        }
    }
}
