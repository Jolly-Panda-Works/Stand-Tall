using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.EnemyModule;
using UnityEngine;

namespace JollyPanda.LastFlag.Economy
{
    /// <summary>
    /// The one place that decides how many coins a wave / defeat pays and saves them.
    /// Informant only announces events; it no longer touches the save file.
    /// </summary>
    public static class RewardService
    {
        /// <returns>Coins that were added to the player.</returns>
        public static int GrantWaveReward(int level, int killedEnemies)
        {
            if (killedEnemies <= 0)
                return 0;

            if (WaveDataGenerator.Instance == null)
            {
                Debug.LogError("RewardService: WaveDataGenerator is missing, no coins granted.");
                return 0;
            }

            int perEnemy = WaveDataGenerator.Instance.CalculateCoinRewardPerEnemy(level);
            int earned = killedEnemies * perEnemy;
            SaveSystem.AddMoney(earned);
            return earned;
        }
    }
}
