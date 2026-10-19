using System;
using UnityEngine;

namespace JollyPanda.LastFlag.Ads
{
    /// <summary>
    /// Fake ad network for testing the reward flow. It "shows" an ad instantly and rewards the player,
    /// therefore it only works in the Editor and Development Builds. A release build reports "no ad ready",
    /// so nobody can farm free coins by accident.
    /// </summary>
    [AddComponentMenu("LastFlag/Ads/Mock Rewarded Ad Service")]
    public class MockRewardedAdService : RewardedAdServiceBase
    {
        [SerializeField] private bool simulateSkip;

        public override bool IsReady => Application.isEditor || Debug.isDebugBuild;

        public override void Show(Action<bool> onFinished)
        {
            if (!IsReady)
            {
                onFinished?.Invoke(false);
                return;
            }

            onFinished?.Invoke(!simulateSkip);
        }
    }
}
