using UnityEngine;

namespace JollyPanda.LastFlag.EnemyModule
{
    public class EnemyAnimationController : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        [Header("Parameters")]
        [SerializeField] private string climbing = "Clim";
        [SerializeField] private string falling = "Fall";
        [SerializeField] private string overEdge = "OverEdge";
        [SerializeField] private string hangingIdle = "HangingIdle";

        public void PlayClimb()
        {
            //animator.Play(climbing);
        }

        public void PlayFall()
        {
            animator.SetTrigger(falling);
        }

        internal void PlayClimbingOverEdge()
        {
            animator.SetTrigger(overEdge);
        }

        internal void PlayHangingIdle()
        {
            animator.SetTrigger(hangingIdle);
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
