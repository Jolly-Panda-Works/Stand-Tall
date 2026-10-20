using System;
using TMPro;
using ULanguage;

namespace JollyPanda.LastFlag.Shop
{
    public enum ShopText
    {
        Shop,
        Customize,
        Buy,
        Owned,
        Equip,
        Equipped,
        Free,
        NotEnoughCoins,
        PurchaseSuccess,
        PurchaseFailed,
        Unavailable,
        Coins,
        Guns,
        Skins,
        Characters,
        Bullets,
        EmptyList,
        DefaultSkin,
        // appended at the end on purpose: ShopLocaleLabel serializes these values
        FreeCoins,
        DoubleReward
    }

    /// <summary>
    /// Tiny bridge to the existing ULanguage system. It keeps all shop strings in code
    /// (English only, like the rest of the UI) and reuses the TMP font chosen by LanguageManager.
    /// </summary>
    public static class ShopLocale
    {
        public static event Action LanguageChanged;

        static ShopLocale()
        {
            LanguageManager.OnLanguageChanged += _ => LanguageChanged?.Invoke();
        }

        public static string Get(ShopText key)
        {
            switch (key)
            {
                case ShopText.Shop: return "Shop";
                case ShopText.Customize: return "Customize";
                case ShopText.Buy: return "Buy";
                case ShopText.Owned: return "Owned";
                case ShopText.Equip: return "Equip";
                case ShopText.Equipped: return "Equipped";
                case ShopText.Free: return "Free";
                case ShopText.NotEnoughCoins: return "Not enough coins";
                case ShopText.PurchaseSuccess: return "Purchase complete";
                case ShopText.PurchaseFailed: return "Purchase failed";
                case ShopText.Unavailable: return "Unavailable";
                case ShopText.Coins: return "Coins";
                case ShopText.Guns: return "Guns";
                case ShopText.Skins: return "Skins";
                case ShopText.Characters: return "Characters";
                case ShopText.Bullets: return "Bullets";
                case ShopText.EmptyList: return "Nothing here yet";
                case ShopText.DefaultSkin: return "Default";
                case ShopText.FreeCoins: return "Free coins";
                case ShopText.DoubleReward: return "x2 coins";
                default: return key.ToString();
            }
        }

        public static string GetName(ShopItemData item)
        {
            if (item == null)
                return string.Empty;

            return !string.IsNullOrEmpty(item.NameEnglish) ? item.NameEnglish : item.Id;
        }

        public static string GetCategoryName(ShopCategory category)
        {
            switch (category)
            {
                case ShopCategory.CoinPack: return Get(ShopText.Coins);
                case ShopCategory.Gun: return Get(ShopText.Guns);
                case ShopCategory.GunSkin: return Get(ShopText.Skins);
                case ShopCategory.Character: return Get(ShopText.Characters);
                case ShopCategory.Bullet: return Get(ShopText.Bullets);
                default: return category.ToString();
            }
        }

        /// <summary>Sets the text and the font of the current language (same thing LocalizedText does).</summary>
        public static void SetText(TMP_Text target, string value)
        {
            if (target == null)
                return;

            target.text = value;

            if (LanguageManager.Instance != null)
            {
                var font = LanguageManager.Instance.GetCurrentTMPFont();
                if (font != null)
                    target.font = font;
            }
        }
    }
}
