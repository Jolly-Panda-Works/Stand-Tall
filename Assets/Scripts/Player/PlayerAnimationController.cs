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

        public void PlayDefeat()
        {
            animator.Play(defeat);
        }

        public void PlayVictory()
        {
            animator.SetTrigger(victory);
        }

        public void PlayIdle()
        {
            animator.SetTrigger(idle);
        }

        public void PlayShooting(bool shooting)
        {
            animator.SetBool(fire, shooting);
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