using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ULanguage.SharedTypes;

namespace ULanguage.Components
{
    [AddComponentMenu("ULanguage/Localized Text")]
    public class LocalizedText : MonoBehaviour
    {
        [SerializeField] private string translationKey;

        private Text uiText;
        private TMP_Text tmpText;

        private void Awake()
        {
            uiText = GetComponent<Text>();
            tmpText = GetComponent<TMP_Text>();

            UpdateText(LanguageManager.CurrentLanguage);
            LanguageManager.OnLanguageChanged += UpdateText;
        }

        private void OnDestroy()
        {
            LanguageManager.OnLanguageChanged -= UpdateText;
        }

        private void UpdateText(LanguageType language)
        {
            string localizedText = LanguageManager.Instance.GetLocalizedText(translationKey);

            if (tmpText != null)
            {
                tmpText.text = localizedText;
                tmpText.font = LanguageManager.Instance.GetCurrentTMPFont();
            }
            else if (uiText != null)
            {
                uiText.text = localizedText;
                uiText.font = LanguageManager.Instance.GetCurrentFont();
            }
        }
    }
}
