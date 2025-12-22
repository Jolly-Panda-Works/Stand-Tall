using UnityEngine;
using UnityEngine.UI;

namespace USound
{
    [RequireComponent(typeof(Button))]
    internal class UIButtonSound : SoundTag
    {
        public override SoundType SoundType => SoundType.SFX;

        [SerializeField] private AudioClip clickSound;

        private Button button;

        protected override void Awake()
        {
            base.Awake();

            button = GetComponent<Button>();
            button.onClick.AddListener(PlayClickSound);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(PlayClickSound);
        }

        public void PlayClickSound()
        {
            if (clickSound != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(AudioSource, clickSound);
            }
        }
    }
}