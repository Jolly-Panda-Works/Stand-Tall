using System;
using UnityEngine;

namespace JollyPanda.LastFlag.EnemyModule
{
    public class WaveDataGenerator : MonoBehaviour
    {
        public static WaveDataGenerator Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(gameObject);
        }

        [Header("Enemy Settings")]
        public Enemy[] allEnemies;
        [Min(1)] public int maxEnemyTypesPerWave = 3;

        [Header("Spawn Settings")]
        [Min(5)] public int minCount = 5;
        [Min(10)] public int maxCount = 50;
        [Min(0.2f)] public float minSpawnInterval = 0.5f;
        public float maxSpawnInterval = 2.5f;

        [Header("Speed Settings")]
        public float baseSpeedMin = 1f;
        public float baseSpeedMax = 2f;

        [Header("Health Settings")]
        public int baseHealthMin = 1;
        public int baseHealthMax = 2;

        [Header("Wave Timing")]
        public float delayAfterWave = 10f;

        public WaveData GenerateWave(int level)
        {
            int enemyCount = Mathf.Clamp(minCount + level, minCount, maxCount);

            float minSpeed = Mathf.Clamp(baseSpeedMin * (1 + level * 0.1f), baseSpeedMin, 5f);
            float maxSpeed = Mathf.Clamp(baseSpeedMax * (1 + level * 0.15f), baseSpeedMax, 10f);

            float spawnInterval = Mathf.Max(maxSpawnInterval - (level * 0.05f), minSpawnInterval);

            int enemyTypeCount = Mathf.Min(1 + level / 10, maxEnemyTypesPerWave);
            Enemy[] selectedEnemies = new Enemy[enemyTypeCount];
            for (int i = 0; i < enemyTypeCount; i++)
            {
                int index = (level + i) % allEnemies.Length;
                selectedEnemies[i] = allEnemies[index];
            }

            int minHealth = Mathf.Clamp(Mathf.RoundToInt(baseHealthMin * level * 0.1f), baseHealthMin, 10);
            int maxHealth = Mathf.Clamp(Mathf.RoundToInt(baseHealthMax * level * 0.15f), baseHealthMax, 20);

            return new WaveData(selectedEnemies, enemyCount, spawnInterval, minSpeed, maxSpeed, minHealth, maxHealth, delayAfterWave);
        }

        public int CalculateCoinRewardPerEnemy(int level)
        {
            float minSpeed = baseSpeedMin + Mathf.Min(level * 0.1f, 4f);
            float maxSpeed = baseSpeedMax + Mathf.Min(level * 0.15f, 5f);
            float avgSpeed = (minSpeed + maxSpeed) / 2f;

            int minHealth = Mathf.Clamp(Mathf.RoundToInt(baseHealthMin * level * 0.1f), (int)baseHealthMin, 10);
            int maxHealth = Mathf.Clamp(Mathf.RoundToInt(baseHealthMax * level * 0.15f), (int)baseHealthMax, 20);
            float avgHealth = (minHealth + maxHealth) / 2f;

            float healthFactor = 0.1f;
            float speedFactor = 0.2f;
            float levelFactor = 0.25f;

            float reward = (avgHealth * healthFactor) + (avgSpeed * speedFactor) + (level * levelFactor);

            return Mathf.RoundToInt(reward);
        }
    }
}
