using System;
using UnityEngine;

namespace JollyPanda.LastFlag.EnemyModule
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Climbing Settings")]
        [Tooltip("Speed at which the enemy climbs upward.")]
        [SerializeField] private float climbSpeed = 2f;

        [Header("Falling Settings")]
        [Tooltip("Speed at which the enemy falls downward.")]
        [SerializeField] private float fallSpeed = 3f;

        [Header("State Flags")]
        private bool canClimb;
        private bool isFalling;

        public event Action OnStartClimbing;
        public event Action OnStartFalling;

        private void Update()
        {
            if (canClimb)
            {
                transform.Translate(climbSpeed * Time.deltaTime * Vector3.up);
            }
            else if (isFalling)
            {
                transform.Translate(fallSpeed * Time.deltaTime * Vector3.down);
            }
        }

        public void Climbing(bool invokeEvent = true)
        {
            canClimb = true;
            isFalling = false;
            if (invokeEvent)
                OnStartClimbing?.Invoke();
        }

        public void StopClimbing()
        {
            canClimb = false;
        }

        public void Falling()
        {
            canClimb = false;
            isFalling = true;
            OnStartFalling?.Invoke();
        }

        public void SetClimbSpeed(float value)
        {
            climbSpeed = value;
        }
    }
}
