using UnityEngine;
using USound;

namespace JollyPanda.LastFlag.EnemyModule
{
    [RequireComponent(typeof(AudioSource))]
    public class EnemySoundController : SoundTag
    {
        public override SoundType SoundType => SoundType.SFX;

        [Header("Sound Clips")]
        [SerializeField] private AudioClip climbClip;
        [SerializeField] private AudioClip fallClip;

        public void PlayClimbSound()
        {
            PlaySFX(climbClip);
        }

        public void PlayFallSound()
        {
            PlaySFX(fallClip);
        }

        private void PlaySFX(AudioClip clip)
        {
            if (clip != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlaySFX(AudioSource, clip);
            }
        }
    }
}
