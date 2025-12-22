namespace JollyPanda.LastFlag.EnemyModule
{
    public struct WaveData
    {
        public Enemy[] EnemyPrefabs { get; private set;}
        public int EnemyCount { get; private set; }
        public float SpawnInterval { get; private set; }
        public float MinSpeed { get; private set; }
        public float MaxSpeed { get; private set; }
        public int MinHealth { get; private set; }
        public int MaxHealth { get; private set; }
        public float DelayAfterWave { get; private set; }

        public WaveData(Enemy[] enemyPrefabs, int enemyCount, float spawnInterval, float minSpeed, float maxSpeed, int minHealth, int maxHealth, float delayAfterWave)
        {
            EnemyPrefabs = enemyPrefabs;
            EnemyCount = enemyCount;
            SpawnInterval = spawnInterval;
            MinSpeed = minSpeed;
            MaxSpeed = maxSpeed;
            MinHealth = minHealth;
            MaxHealth = maxHealth;
            DelayAfterWave = delayAfterWave;
        }
    }
}
