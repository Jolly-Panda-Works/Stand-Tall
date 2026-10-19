using System;
using TMPro;
using ULanguage;
using ULanguage.SharedTypes;

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
    /// (English + Persian), so no new LanguageEntry assets are needed, and it reuses the
    /// current language and TMP font chosen by LanguageManager.
    /// </summary>
    public static class ShopLocale
    {
        public static event Action LanguageChanged;

        static ShopLocale()
        {
            LanguageManager.OnLanguageChanged += _ => LanguageChanged?.Invoke();
        }

        public static bool IsPersian =>
            LanguageManager.Instance != null && LanguageManager.CurrentLanguage == LanguageType.Persian;

        public static string Get(ShopText key)
        {
            bool fa = IsPersian;
            switch (key)
            {
                case ShopText.Shop: return fa ? "فروشگاه" : "Shop";
                case ShopText.Customize: return fa ? "شخصی‌سازی" : "Customize";
                case ShopText.Buy: return fa ? "خرید" : "Buy";
                case ShopText.Owned: return fa ? "خریداری شده" : "Owned";
                case ShopText.Equip: return fa ? "انتخاب" : "Equip";
                case ShopText.Equipped: return fa ? "انتخاب شده" : "Equipped";
                case ShopText.Free: return fa ? "رایگان" : "Free";
                case ShopText.NotEnoughCoins: return fa ? "سکه کافی نیست" : "Not enough coins";
                case ShopText.PurchaseSuccess: return fa ? "خرید انجام شد" : "Purchase complete";
                case ShopText.PurchaseFailed: return fa ? "خرید ناموفق بود" : "Purchase failed";
                case ShopText.Unavailable: return fa ? "در دسترس نیست" : "Unavailable";
                case ShopText.Coins: return fa ? "سکه" : "Coins";
                case ShopText.Guns: return fa ? "اسلحه" : "Guns";
                case ShopText.Skins: return fa ? "اسکین" : "Skins";
                case ShopText.Characters: return fa ? "کاراکتر" : "Characters";
                case ShopText.Bullets: return fa ? "تیر" : "Bullets";
                case ShopText.EmptyList: return fa ? "موردی وجود ندارد" : "Nothing here yet";
                case ShopText.DefaultSkin: return fa ? "پیش‌فرض" : "Default";
                case ShopText.FreeCoins: return fa ? "سکه رایگان" : "Free coins";
                case ShopText.DoubleReward: return fa ? "دو برابر" : "x2 coins";
                default: return key.ToString();
            }
        }

        public static string GetName(ShopItemData item)
        {
            if (item == null)
                return string.Empty;

            string localized = IsPersian ? item.NamePersian : item.NameEnglish;
            if (!string.IsNullOrEmpty(localized))
                return localized;

            // fall back to the other language, then to the asset id
            string other = IsPersian ? item.NameEnglish : item.NamePersian;
            return !string.IsNullOrEmpty(other) ? other : item.Id;
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
