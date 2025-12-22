using System;
using System.Collections.Generic;
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
    internal class LanguageManager : MonoBehaviour
    {
        public static LanguageManager Instance { get; private set; }

        public static event Action<LanguageType> OnLanguageChanged;

        [Header("Language Settings")]
        [SerializeField] private LanguageType currentLanguage = LanguageType.English;
        [SerializeField] private List<LanguageEntry> translations;

        [Header("Fonts")]
        [SerializeField] private Font englishFont;
        [SerializeField] private Font persianFont;

        [SerializeField] private TMP_FontAsset englishTMPFont;
        [SerializeField] private TMP_FontAsset persianTMPFont;

        private Dictionary<string, LanguageEntry> translationDict = new();

        public static LanguageType CurrentLanguage => Instance.currentLanguage;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (var entry in translations)
            {
                if (!translationDict.ContainsKey(entry.key))
                    translationDict.Add(entry.key, entry);
            }
        }

        public void SetLanguage(LanguageType newLanguage)
        {
            if (currentLanguage == newLanguage) return;

            currentLanguage = newLanguage;
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
