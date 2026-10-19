using System;
using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    public enum PurchaseResult
    {
        Success,
        Cancelled,
        Failed,
        Unavailable
    }

    /// <summary>
    /// Bridge between the shop and a real-money store (Cafe Bazaar, Myket, Google Play, Unity IAP ...).
    /// Create one MonoBehaviour per store that derives from this class, put it in the scene and
    /// assign it to <see cref="ShopManager"/>. The shop never talks to a store SDK directly.
    /// </summary>
    public abstract class PurchaseServiceBase : MonoBehaviour
    {
        /// <summary>
        /// Raised when the store reports a payment that was not started by the player in this session
        /// (e.g. the app was closed after paying but before the coins were granted). ShopManager grants the coins.
        /// </summary>
        public event Action<CoinPackData> PurchaseRestored;

        protected void RaisePurchaseRestored(CoinPackData pack)
        {
            PurchaseRestored?.Invoke(pack);
        }

        /// <summary>False while the store SDK is not connected / not ready.</summary>
        public abstract bool IsAvailable { get; }

        /// <summary>Localized price string from the store (e.g. "49,000 تومان"), or null if unknown.</summary>
        public abstract string GetPriceText(CoinPackData pack);

        /// <summary>
        /// Start the purchase flow of a coin pack. <paramref name="onComplete"/> must be called exactly once
        /// and only with <see cref="PurchaseResult.Success"/> after the store (ideally your server) confirmed the payment.
        /// The shop adds the coins itself when it receives Success.
        /// </summary>
        public abstract void Purchase(CoinPackData pack, Action<PurchaseResult> onComplete);
    }
}
