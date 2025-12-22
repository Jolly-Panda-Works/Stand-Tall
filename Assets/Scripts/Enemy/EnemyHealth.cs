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

        public event Action<int> OnHealthChange;
        public event Action OnDead;
        public event Action OnRevive;

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
            OnRevive?.Invoke();
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
