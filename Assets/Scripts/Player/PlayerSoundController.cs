using UnityEngine;
using USound;

namespace JollyPanda.LastFlag.PlayerModule
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerSoundController : SoundTag
    {
        public override SoundType SoundType => SoundType.SFX;

        [Header("Sound Clips")]
        [SerializeField] private AudioClip defeatClip;
        [SerializeField] private AudioClip victoryClip;
        [SerializeField] private AudioClip idleClip;
        [SerializeField] private AudioClip fireClip;

        private bool isMuted = false;

        public void PlayDefeatSound()
        {
            PlaySFX(defeatClip);
        }

        public void PlayVictorySound()
        {
            PlaySFX(victoryClip);
        }

        public void PlayIdleSound()
        {
            PlaySFX(idleClip);
        }

        public void PlayFireSound()
        {
            PlaySFX(fireClip);
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip != null && SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(AudioSource, clip);
        }

        public void SetMute(bool mute)
        {
            isMuted = mute;
            if (isMuted)
            {
                AudioSource.Stop();
            }
        }
    }
}