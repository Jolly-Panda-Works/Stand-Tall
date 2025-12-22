using UnityEngine;
using UnityEngine.UI;
using ULanguage.SharedTypes;

namespace ULanguage.Components
{
    [AddComponentMenu("ULanguage/Localized Image")]
    public class LocalizedImage : MonoBehaviour
    {
        [SerializeField] private Sprite englishSprite;
        [SerializeField] private Sprite persianSprite;

        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
            UpdateImage(LanguageManager.CurrentLanguage);
            LanguageManager.OnLanguageChanged += UpdateImage;
        }

        private void OnDestroy()
        {
            LanguageManager.OnLanguageChanged -= UpdateImage;
        }

        private void UpdateImage(LanguageType language)
        {
            if (image == null) return;

            image.sprite = language switch
            {
                LanguageType.English => englishSprite,
                LanguageType.Persian => persianSprite,
                _ => image.sprite
            };
        }
    }
}
