using JollyPanda.LastFlag.Handlers;
using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float speed = 10f;
        [SerializeField, Min(1f)] private int damage = 1;

        [Tooltip("Layers the bullet can hit (Enemy + Ground). Leave as Everything if unsure.")]
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField, Min(0.01f)] private float hitRadius = 0.05f;

        private static readonly RaycastHit[] hitBuffer = new RaycastHit[8];

        public bool CanMoving { private get; set; }

        private void OnEnable()
        {
            Informant.OnLose += ForceDestory;
        }

        private void OnDisable()
        {
            Informant.OnLose -= ForceDestory;
        }

        private void ForceDestory()
        {
            gameObject.SetActive(false);
        }

        void Update()
        {
            if (!CanMoving)
            {
                return;
            }

            float step = speed * Time.deltaTime;
            float radius = hitRadius * transform.localScale.x;

            // Sweep along this frame's movement: at high speed the bullet would otherwise jump
            // over a thin collider between two frames (OnTriggerEnter never fires).
            int count = Physics.SphereCastNonAlloc(transform.position, radius, Vector3.down, hitBuffer, step, hitMask, QueryTriggerInteraction.Collide);
            for (int i = 0; i < count; i++)
            {
                if (Hit(hitBuffer[i].collider))
                    return;
            }

            // Move straight down in world space
            transform.Translate(step * Vector3.down, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            Hit(other);
        }

        /// <returns>true when the bullet was consumed.</returns>
        private bool Hit(Collider other)
        {
            if (!gameObject.activeSelf)
                return true;

            if (other.CompareTag("Ground"))
            {
                gameObject.SetActive(false);
                return true;
            }

            // the collider can sit on a child of the enemy model
            var enemyHealth = other.GetComponentInParent<EnemyModule.EnemyHealth>();
            if (enemyHealth != null && enemyHealth.IsAlive)
            {
                enemyHealth.TakeDamage(damage);
                gameObject.SetActive(false);
                return true;
            }

            return false;
        }

        public void Initialize(int damage, float scale, float speed)
        {
            this.damage = damage;
            transform.localScale = Vector3.one * scale;
            this.speed = speed;
        }
    }
}
