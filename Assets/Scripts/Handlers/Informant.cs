using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.EnemyModule;
using System;

namespace JollyPanda.LastFlag.Handlers
{
    public static class Informant
    {
        public static event Action<Enemy> OnEnemyReachedTop;
        public static event Action OnLose;
        public static event Action<int> OnEarnCoin;
        public static event Action OnStart;
        public static event Action OnFinish;
        public static event Action<bool> OnPause;
        public static event Action<PageType> OnChangeUIPage;
        public static event Action<int> OnWaveStart;
        public static event Action<int, int> OnWaveEnd;
        public static event Action<int, int> OnEnemyKilled;
        public static event Action<PlayerSaveData> OnProfileChange
        {
            add => SaveSystem.OnApplyChange += value;
            remove => SaveSystem.OnApplyChange -= value;
        }

        public static void NotifyEnemyReachedTop(Enemy enemy, int killedEnemy)
        {
            OnEnemyReachedTop?.Invoke(enemy);
            OnLose?.Invoke();

            int level = SaveSystem.GetCurrentWaveIndex();
            int rewardPerEnemy = WaveDataGenerator.Instance.CalculateCoinRewardPerEnemy(level);
            int earnedCoins = killedEnemy * rewardPerEnemy;
            SaveSystem.AddMoney(earnedCoins);
            OnEarnCoin?.Invoke(earnedCoins);
        }

        public static void NotifyStart()
        {
            OnStart?.Invoke();
        }

        public static void PauseGame(bool isPause)
        {
            OnPause?.Invoke(isPause);
        }

        public static void NotifyStartWave(int waveIndex)
        {
            OnWaveStart?.Invoke(waveIndex);
        }

        public static void NotifyFinishWave(int waveIndex, int killedEnemy)
        {
            int level = waveIndex + 1;
            int rewardPerEnemy = WaveDataGenerator.Instance.CalculateCoinRewardPerEnemy(level);
            int earnedCoins = killedEnemy * rewardPerEnemy;

            SaveSystem.AddMoney(earnedCoins);
            SaveSystem.UpdateLastWaveIndex(waveIndex);
            OnEarnCoin?.Invoke(earnedCoins);
            OnWaveEnd?.Invoke(waveIndex, killedEnemy);
            OnFinish?.Invoke();
        }

        public static void NotifyEnemyKilled(int alivedEnemyCount, int totalEnemy)
        {
            //Debug.Log("NotifyEnemyKilled");
            OnEnemyKilled?.Invoke(alivedEnemyCount, totalEnemy);
        }

        internal static void OnChangePage(PageType pageType)
        {
            OnChangeUIPage?.Invoke(pageType);
        }
    }
}