using ULanguage.SharedTypes;
using UnityEngine;
using UnityEngine.UI;

namespace ULanguage.Components
{
    [RequireComponent(typeof(Toggle))]
    [AddComponentMenu("ULanguage/Language Toggle")]
    internal class LanguageToggle : MonoBehaviour
    {
        [SerializeField] private LanguageType languageToSet;

        private Toggle toggle;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }

        private void OnToggleChanged(bool isOn)
        {
            if (isOn)
            {
                LanguageManager.Instance.SetLanguage(languageToSet);
            }
        }
    }
}