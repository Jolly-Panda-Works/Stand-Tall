using System;
using System.Collections.Generic;
using System.Linq;
using JollyPanda.LastFlag.Database;
using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    public enum BuyResult
    {
        Success,
        AlreadyOwned,
        NotEnoughCoins,
        NotPurchasable
    }

    /// <summary>
    /// Single entry point for buying and equipping. All state lives in <see cref="SaveSystem"/>
    /// (no parallel state): owned ids + equipped ids are part of PlayerSaveData, and every change
    /// is announced through the existing Informant.OnProfileChange event.
    /// Place it in the main scene next to GunUpgradeManager.
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        public static ShopManager Instance { get; private set; }

        [SerializeField] private ShopCatalog catalog;
        [Tooltip("Optional. Without it coin packs are shown as unavailable.")]
        [SerializeField] private PurchaseServiceBase purchaseService;

        private bool purchaseInProgress;

        public ShopCatalog Catalog => catalog;
        public PurchaseServiceBase PurchaseService => purchaseService;
        public bool IsPurchaseInProgress => purchaseInProgress;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            if (purchaseService != null)
                purchaseService.PurchaseRestored += OnPurchaseRestored;
        }

        private void OnDisable()
        {
            if (purchaseService != null)
                purchaseService.PurchaseRestored -= OnPurchaseRestored;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>A paid purchase that was recovered on startup: give the coins now.</summary>
        private void OnPurchaseRestored(CoinPackData pack)
        {
            if (pack != null)
                SaveSystem.AddMoney(pack.CoinAmount);
        }

        // ------------------------------------------------------------------ queries

        public bool IsOwned(ShopItemData item)
        {
            if (item == null || item.Category == ShopCategory.CoinPack)
                return false;

            return item.IsDefault || SaveSystem.HasItem(item.Id);
        }

        public bool CanAfford(ShopItemData item)
        {
            return item != null && SaveSystem.GetMoney() >= item.Price;
        }

        public bool IsEquipped(ShopItemData item)
        {
            if (item == null)
                return false;

            switch (item)
            {
                case GunSkinItemData skin:
                    return GetEquippedSkin(skin.GunId) == skin;
                case GunItemData _:
                case CharacterItemData _:
                case BulletItemData _:
                    return GetEquipped(item.Category) == item;
                default:
                    return false;
            }
        }

        /// <summary>Equipped gun / character / bullet. Falls back to the default item of the category.</summary>
        public ShopItemData GetEquipped(ShopCategory category)
        {
            string id;
            switch (category)
            {
                case ShopCategory.Gun: id = SaveSystem.GetEquippedGunId(); break;
                case ShopCategory.Character: id = SaveSystem.GetEquippedCharacterId(); break;
                case ShopCategory.Bullet: id = SaveSystem.GetEquippedBulletId(); break;
                default: return null;
            }

            var item = catalog != null ? catalog.FindById(id) : null;
            if (item != null && item.Category == category && IsOwned(item))
                return item;

            return catalog != null ? catalog.GetDefault(category) : null;
        }

        /// <summary>Skin equipped on one gun. Falls back to that gun's default skin (can be null = original look).</summary>
        public GunSkinItemData GetEquippedSkin(int gunId)
        {
            if (catalog == null)
                return null;

            foreach (var id in SaveSystem.GetEquippedGunSkinIds())
            {
                if (catalog.FindById(id) is GunSkinItemData skin && skin.GunId == gunId && IsOwned(skin))
                    return skin;
            }

            return catalog.GetDefaultSkin(gunId);
        }

        // ------------------------------------------------------------------ buying

        /// <summary>Buys a gun / skin / character / bullet with coins. One atomic save.</summary>
        public BuyResult TryBuy(ShopItemData item)
        {
            if (item == null || item.Category == ShopCategory.CoinPack)
                return BuyResult.NotPurchasable;

            if (IsOwned(item))
                return BuyResult.AlreadyOwned;

            if (!SaveSystem.TryPurchaseItem(item.Id, item.Price))
                return BuyResult.NotEnoughCoins;

            return BuyResult.Success;
        }

        /// <summary>Real-money purchase of a coin pack through the assigned purchase service.</summary>
        public void BuyCoinPack(CoinPackData pack, Action<PurchaseResult> onComplete)
        {
            if (pack == null || purchaseService == null || !purchaseService.IsAvailable)
            {
                onComplete?.Invoke(PurchaseResult.Unavailable);
                return;
            }

            if (purchaseInProgress)
                return;

            purchaseInProgress = true;
            purchaseService.Purchase(pack, result =>
            {
                purchaseInProgress = false;

                if (result == PurchaseResult.Success)
                    SaveSystem.AddMoney(pack.CoinAmount);

                onComplete?.Invoke(result);
            });
        }

        // ------------------------------------------------------------------ equipping

        public bool Equip(ShopItemData item)
        {
            if (item == null || !IsOwned(item))
                return false;

            switch (item)
            {
                case GunItemData gun:
                    SaveSystem.SetEquippedGunId(gun.Id);
                    return true;

                case CharacterItemData character:
                    SaveSystem.SetEquippedCharacterId(character.Id);
                    return true;

                case BulletItemData bullet:
                    SaveSystem.SetEquippedBulletId(bullet.Id);
                    return true;

                case GunSkinItemData skin:
                    IEnumerable<string> sameGunSkins = catalog.GetByCategory(ShopCategory.GunSkin)
                        .OfType<GunSkinItemData>()
                        .Where(s => s.GunId == skin.GunId)
                        .Select(s => s.Id);
                    SaveSystem.SetEquippedGunSkin(skin.Id, sameGunSkins);
                    return true;

                default:
                    return false;
            }
        }
    }
}
