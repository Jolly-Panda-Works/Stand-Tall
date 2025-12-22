using JollyPanda.LastFlag.Database;
using UnityEngine;
using UnityEngine.UI;

namespace MJUtilities.UI
{
    public class SettingPopup : UIPopup
    {
        [SerializeField] private Button languageButton;
        [SerializeField] private Button webButton;

        [Header("Sound")]
        [SerializeField] private Slider sfxVolume;
        [SerializeField] private Slider backgroundVolume;

        protected override void OnEnable()
        {
            base.OnEnable();

            backgroundVolume.value = SaveSystem.GetBackgroundVolume();
            sfxVolume.value = SaveSystem.GetSfxVolume();
        }

        private void OnDisable()
        {
            USound.SoundManager.Instance.Save();
        }

        public override void OnAwake()
        {
            languageButton.onClick.AddListener(ClickOnLanguage);

            webButton.onClick.AddListener(ClickOnWeb);

            sfxVolume.onValueChanged.AddListener(ChangeSFXVolume);

            backgroundVolume.onValueChanged.AddListener(ChangeBackgroundVolume);
        }

        private void OnDestroy()
        {
            languageButton.onClick.RemoveListener(ClickOnLanguage);

            webButton.onClick.RemoveListener(ClickOnWeb);

            sfxVolume.onValueChanged.RemoveListener(ChangeSFXVolume);

            backgroundVolume.onValueChanged.RemoveListener(ChangeBackgroundVolume);
        }

        private void ClickOnLanguage() => UIManager.instance.OpenPopup(PopupType.Language);
        private void ClickOnWeb() => Application.OpenURL("https://useffarahmand.com/");
        private void ChangeSFXVolume(float volume) => USound.SoundManager.Instance.SetSFXVolume(volume);
        private void ChangeBackgroundVolume(float volume) => USound.SoundManager.Instance.SetBackgroundVolume(volume);

        public override void OnSetValues()
        {
            BackButtonConfig();
        }
    }
}