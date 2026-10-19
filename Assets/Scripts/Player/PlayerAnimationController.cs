using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [Header("Parameters")]
        [SerializeField] private string defeat = "Defeat";
        [SerializeField] private string victory = "Victory";
        [SerializeField] private string idle = "Idle";
        [SerializeField] private string fire = "Fire";

        /// <summary>Customize: the character model changed, drive the new model's Animator from now on.</summary>
        public void SetAnimator(Animator newAnimator)
        {
            if (newAnimator != null)
                animator = newAnimator;
        }

        // Hashed parameter names are faster than strings. Built lazily so the
        // inspector values are used even if they were changed after Awake.
        private int defeatHash, victoryHash, idleHash, fireHash;
        private bool hashesReady;

        private void EnsureHashes()
        {
            if (hashesReady)
                return;

            defeatHash = Animator.StringToHash(defeat);
            victoryHash = Animator.StringToHash(victory);
            idleHash = Animator.StringToHash(idle);
            fireHash = Animator.StringToHash(fire);
            hashesReady = true;
        }

        public void PlayDefeat()
        {
            EnsureHashes();
            animator.Play(defeatHash);
        }

        public void PlayVictory()
        {
            EnsureHashes();
            animator.SetTrigger(victoryHash);
        }

        public void PlayIdle()
        {
            EnsureHashes();
            animator.SetTrigger(idleHash);
        }

        public void PlayShooting(bool shooting)
        {
            EnsureHashes();
            animator.SetBool(fireHash, shooting);
        }

        internal void Pause()
        {
            animator.speed = 0;
        }

        internal void Resume()
        {
            animator.speed = 1;
        }
    }
}