using System;
using UnityEngine;

namespace JollyPanda.LastFlag.EnemyModule
{
    public class EnemyPositionChecker : MonoBehaviour
    {
        [Header("Enemy Movement Settings")]
        [SerializeField] private float edgeY = 10f;
        [SerializeField] private float groundY;

        private bool hasReachedTop = false;  // Flag to track if the enemy has reached the top
        private CheckState checkState;
        public event Action OnEnemyReachedTop;
        public event Action OnEnemyReachedGround;

        private void Awake()
        {
            GameObject ground = GameObject.FindWithTag("Ground");
            if (ground != null)
            {
                groundY = ground.transform.position.y;
            }
            else
            {
                Debug.LogWarning("Ground object not found! Make sure it's tagged as 'Ground'.");
                groundY = -10f; // default fallback
            }

            GameObject edge = GameObject.FindWithTag("Edge");
            if (edge != null)
            {
                edgeY = edge.transform.position.y;
            }
            else
            {
                Debug.LogWarning("Edge object not found! Make sure it's tagged as 'Edge'.");
                edgeY = 10f; // default fallback
            }
        }

        private void OnEnable()
        {
            SetCheckState(CheckState.MoveUp);
            hasReachedTop = false;
        }

        public void SetCheckState(CheckState state)
        {
            checkState = state;
        }

        private void Update()
        {
            switch (checkState)
            {
                case CheckState.MoveUp:
                    CheckUpward();
                    break;
                case CheckState.MoveDown:
                    CheckDownward();
                    break;
            }
        }

        private void CheckUpward()
        {
            if (!hasReachedTop && transform.position.y >= edgeY)
            {
                hasReachedTop = true;

                OnEnemyReachedTop?.Invoke();
            }
        }

        private void CheckDownward()
        {
            if (transform.position.y < groundY)
            {
                OnEnemyReachedGround?.Invoke();
            }
        }

        public enum CheckState
        {
            MoveUp,
            MoveDown
        }
    }
}
