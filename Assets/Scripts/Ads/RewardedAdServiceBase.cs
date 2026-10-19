using System;
using UnityEngine;

namespace JollyPanda.LastFlag.Ads
{
    /// <summary>
    /// Bridge between the game and a rewarded-video ad network (AdMob, Unity Ads, Tapsell, Adivery ...).
    /// Put ONE derived component in the main scene. The game never talks to an ad SDK directly,
    /// it only asks <see cref="Instance"/> whether an ad is ready and shows it.
    /// </summary>
    public abstract class RewardedAdServiceBase : MonoBehaviour
    {
        public static RewardedAdServiceBase Instance { get; private set; }

        /// <summary>True when an ad is loaded and can be shown right now.</summary>
        public abstract bool IsReady { get; }

        /// <summary>
        /// Shows the ad. <paramref name="onFinished"/> is called exactly once after the ad closed:
        /// true = the player watched it and earned the reward, false = not ready, failed or skipped.
        /// </summary>
        public abstract void Show(Action<bool> onFinished);

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        protected virtual void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>False for a duplicate that is about to be destroyed (derived classes must not initialise an SDK then).</summary>
        protected bool IsActiveInstance => Instance == this;
    }
}
