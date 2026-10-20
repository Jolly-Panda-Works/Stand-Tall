using System;
using System.Collections.Generic;
using JollyPanda.LastFlag.Database;
using UnityEngine;
using ULanguage.SharedTypes;
using ULanguage.Data;
using TMPro;

namespace ULanguage.Data
{
    [CreateAssetMenu(fileName = "LanguageEntry", menuName = "ULanguage/Language Entry")]
    internal class LanguageEntry : ScriptableObject
    {
        public string key;

        [TextArea] public string englishText;
        [TextArea] public string persianText;

        public string GetText(LanguageType language)
        {
            return language switch
            {
                LanguageType.English => englishText,
                LanguageType.Persian => persianText,
                _ => englishText
            };
        }
    }
}

namespace ULanguage
{
    // Must run before every LocalizedText/LocalizedImage Awake (they read CurrentLanguage).
    [DefaultExecutionOrder(-200)]
    internal class LanguageManager : MonoBehaviour
    {
        public static LanguageManager Instance { get; private set; }

        public static event Action<LanguageType> OnLanguageChanged;

        /// <summary>The game ships in English only: the saved/device language is ignored and SetLanguage only accepts English.</summary>
        public const bool EnglishOnly = true;

        [Header("Language Settings")]
        [SerializeField] private LanguageType currentLanguage = LanguageType.English;
        [SerializeField] private List<LanguageEntry> translations;

        [Header("Fonts")]
        [SerializeField] private Font englishFont;
        [SerializeField] private Font persianFont;

        [SerializeField] private TMP_FontAsset englishTMPFont;
        [SerializeField] private TMP_FontAsset persianTMPFont;

        private Dictionary<string, LanguageEntry> translationDict = new();

        public static LanguageType CurrentLanguage => Instance != null ? Instance.currentLanguage : LanguageType.English;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            // saved language, or on the very first launch the phone language
            int saved = SaveSystem.GetLanguageIndex();
            if (saved < 0 || !Enum.IsDefined(typeof(LanguageType), saved))
            {
                // Unity's SystemLanguage enum has no Persian, so read the device culture ("fa", "fa-IR", ...)
                string iso = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                saved = string.Equals(iso, "fa", StringComparison.OrdinalIgnoreCase)
                    ? (int)LanguageType.Persian
                    : (int)LanguageType.English;
            }
            currentLanguage = EnglishOnly ? LanguageType.English : (LanguageType)saved;

            foreach (var entry in translations)
            {
                if (!translationDict.ContainsKey(entry.key))
                    translationDict.Add(entry.key, entry);
            }
        }

        public void SetLanguage(LanguageType newLanguage)
        {
            if (EnglishOnly)
                newLanguage = LanguageType.English;

            if (currentLanguage == newLanguage) return;

            currentLanguage = newLanguage;
            SaveSystem.UpdateLanguageIndex((int)newLanguage);
            OnLanguageChanged?.Invoke(currentLanguage);
        }

        public string GetLocalizedText(string key)
        {
            if (translationDict.TryGetValue(key, out var entry))
                return entry.GetText(currentLanguage);

            Debug.LogWarning($"Missing translation for key: {key}");
            return key;
        }

        public Font GetCurrentFont()
        {
            return currentLanguage == LanguageType.English ? englishFont : persianFont;
        }

        public TMP_FontAsset GetCurrentTMPFont()
        {
            return currentLanguage == LanguageType.English ? englishTMPFont : persianTMPFont;
        }
    }
}
