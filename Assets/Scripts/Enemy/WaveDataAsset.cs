using UnityEngine;

namespace JollyPanda.LastFlag.EnemyModule
{
    [CreateAssetMenu(fileName = "NewWaveData", menuName = "LastFlag/Wave Data", order = 0)]
    public class WaveDataAsset : ScriptableObject
    {
        [Tooltip("Enemies to spawn in this wave.")]
        [field: SerializeField, Header("Enemy Settings")] public Enemy[] EnemyPrefabs { get; private set; }

        [Tooltip("Number of enemies to spawn in this wave.")]
        [field: SerializeField, Min(0)] public int EnemyCount { get; private set; }

        [Tooltip("Time between spawns in this wave.")]
        [field: SerializeField, Min(0), Header("Spawn Settings")] public float SpawnInterval { get; private set; } = 1f;

        [Tooltip("Minimum speed for enemies in this wave.")]
        [field: SerializeField, Min(0), Header("Speed Settings")] public float MinSpeed { get; private set; } = 1f;

        [Tooltip("Maximum speed for enemies in this wave.")]
        [field: SerializeField, Min(0)] public float MaxSpeed { get; private set; } = 2f;

        [Tooltip("Minimum health for enemies in this wave.")]
        [field: SerializeField, Min(1), Header("Health Settings")]
        public int MinHealth { get; private set; } = 10;

        [Tooltip("Maximum health for enemies in this wave.")]
        [field: SerializeField, Min(1)] public int MaxHealth { get; private set; } = 20;

        [Tooltip("Time to wait before starting the next wave.")]
        [field: SerializeField, Min(0), Header("Wave Settings")] public float DelayAfterWave { get; private set; } = 5f;

        public WaveData GetData()
        {
            return new WaveData(
                EnemyPrefabs,
                EnemyCount,
                SpawnInterval,
                MinSpeed,
                MaxSpeed,
                MinHealth,
                MaxHealth,
                DelayAfterWave
            );
        }
    }
}
