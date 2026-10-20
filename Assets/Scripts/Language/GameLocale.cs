using TMPro;
using ULanguage;
using ULanguage.SharedTypes;
using UnityEngine;

namespace JollyPanda.LastFlag.Localization
{
    public enum GameText
    {
        WaveStarted,   // "{0}" = wave number
        MaxLevel,
        SecondsSuffix  // "{0}" = seconds
    }

    /// <summary>
    /// Code-based strings for the gameplay UI (same approach as ShopLocale), so no new
    /// LanguageEntry assets are needed. The game UI is English only.
    /// </summary>
    public static class GameLocale
    {
        public static string Get(GameText key)
        {
            switch (key)
            {
                case GameText.WaveStarted:
                    return "Enemies rise!\nWave {0} has begun!";
                case GameText.MaxLevel:
                    return "MAX";
                case GameText.SecondsSuffix:
                    return "{0}s";
                default:
                    return key.ToString();
            }
        }

        public static string Format(GameText key, object arg) => string.Format(Get(key), arg);

        /// <summary>Sets the text and the TMP font of the current language.</summary>
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
