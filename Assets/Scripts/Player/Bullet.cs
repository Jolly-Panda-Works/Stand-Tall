using JollyPanda.LastFlag.Handlers;
using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    public class Bullet : MonoBehaviour
    {
        [SerializeField, Min(0.01f)] private float speed = 10f;
        [SerializeField, Min(1f)] private int damage = 1;

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
            // Move straight down in world space
            transform.Translate(speed * Time.deltaTime * Vector3.down, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Ground"))
            {
                gameObject.SetActive(false);
            }
            else if (other.TryGetComponent<EnemyModule.EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(damage);
                gameObject.SetActive(false);
            }
        }

        public void Initialize(int damage, float scale, float speed)
        {
            this.damage = damage;
            transform.localScale = Vector3.one * scale;
            this.speed = speed;
        }
    }
}
