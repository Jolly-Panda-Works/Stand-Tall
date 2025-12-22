using UnityEngine;

namespace USound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundTag : MonoBehaviour
    {
        public virtual SoundType SoundType { get; protected set; }

        private AudioSource audioSource;
        protected AudioSource AudioSource
        {
            get
            {
                if (audioSource == null)
                {
                    audioSource = GetComponent<AudioSource>();
                    AudioSource.playOnAwake = false;
                }
                return audioSource;
            }
        }

        protected virtual void Awake()
        {

        }

        private void Start()
        {
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.RegisterSound(this);
            }
        }

        public void SetVolume(float volume)
        {
            if (AudioSource != null)
            {
                AudioSource.volume = volume;
            }
        }

        public AudioSource GetAudioSource() => AudioSource;
    }
}