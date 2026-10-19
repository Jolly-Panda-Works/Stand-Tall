using System;
using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    /// <summary>
    /// Fake store used for testing the shop flow. It grants every purchase for free,
    /// therefore it only works in the Editor and in Development Builds. In a release build it reports
    /// "unavailable", so nobody can get free coins by accident.
    /// </summary>
    [AddComponentMenu("LastFlag/Shop/Mock Purchase Service")]
    public class MockPurchaseService : PurchaseServiceBase
    {
        [SerializeField] private bool simulateFailure;

        public override bool IsAvailable => Application.isEditor || Debug.isDebugBuild;

        public override string GetPriceText(CoinPackData pack) => null;

        public override void Purchase(CoinPackData pack, Action<PurchaseResult> onComplete)
        {
            if (!IsAvailable)
            {
                onComplete?.Invoke(PurchaseResult.Unavailable);
                return;
            }

            onComplete?.Invoke(simulateFailure ? PurchaseResult.Failed : PurchaseResult.Success);
        }
    }
}
