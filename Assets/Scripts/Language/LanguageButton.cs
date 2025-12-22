using ULanguage.SharedTypes;
using UnityEngine;
using UnityEngine.UI;

namespace ULanguage.Components
{
    [RequireComponent(typeof(Button))]
    [AddComponentMenu("ULanguage/Language Button")]
    internal class LanguageButton : MonoBehaviour
    {
        [SerializeField] private LanguageType languageToSet;

        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(SetLanguage);
        }

        public void SetLanguage()
        {
            LanguageManager.Instance.SetLanguage(languageToSet);
        }
    }
}