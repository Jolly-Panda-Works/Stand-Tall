using System;
using UnityEngine;
using UnityEngine.UI;

namespace JollyPanda.LastFlag.EnemyModule
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        public int maxHealth = 3;
        private int currentHealth;
        private bool isFalling = false;

        [Header("UI")]
        [Tooltip("Reference to the health slider UI.")]
        public Slider healthSlider;

        /// <summary>False once the enemy started falling (dead). Bullets pass through dead enemies.</summary>
        public bool IsAlive => !isFalling;

        public event Action<int> OnHealthChange;
        public event Action OnDead;
        [Obsolete("No longer raised. Enemy starts climbing from its own OnEnable.")]
        public event Action OnRevive { add { } remove { } }

        private void Awake()
        {
            healthSlider.maxValue = maxHealth;
        }

        private void OnEnable()
        {
            currentHealth = maxHealth;
            isFalling = false;
            SetHealth(maxHealth);
            ShowHealthBar(true);
        }

        internal void SetMaxHealth(int value)
        {
            maxHealth = value;
            healthSlider.maxValue = maxHealth;
            SetHealth(maxHealth);
        }

        public void TakeDamage(int amount)
        {
            SetHealth(currentHealth - amount);
        }

        private void SetHealth(int amount)
        {
            currentHealth = amount;
            OnHealthChange?.Invoke(currentHealth);
            UpdateHealthBar();

            if (currentHealth <= 0 && !isFalling)
            {
                isFalling = true;
                OnDead?.Invoke();
            }
        }

        private void UpdateHealthBar()
        {
            healthSlider.value = currentHealth;
        }

        public void ShowHealthBar(bool show)
        {
            healthSlider.gameObject.SetActive(show);
        }
    }
}
