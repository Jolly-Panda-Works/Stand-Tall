// Google Play (and any store supported by Unity IAP) purchase service.
// Compiled only when the scripting define symbol LASTFLAG_UNITY_IAP is set, AFTER installing
// the "In App Purchasing" package, version 4.x (the IStoreListener API). See the guide.
#if LASTFLAG_UNITY_IAP
using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace JollyPanda.LastFlag.Shop
{
    [AddComponentMenu("LastFlag/Shop/Unity IAP Purchase Service")]
    public class UnityIapPurchaseService : PurchaseServiceBase, IStoreListener
    {
        [Tooltip("Every CoinPackData with a ProductId in this catalog is registered as a consumable product.")]
        [SerializeField] private ShopCatalog catalog;

        private IStoreController controller;
        private Action<PurchaseResult> pending;

        public override bool IsAvailable => controller != null;

        private void Start()
        {
            if (catalog == null)
            {
                Debug.LogError("[IAP] ShopCatalog is not assigned.", this);
                return;
            }

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (var item in catalog.Items)
            {
                if (item is CoinPackData pack && !string.IsNullOrEmpty(pack.ProductId))
                    builder.AddProduct(pack.ProductId, ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);
        }

        public override string GetPriceText(CoinPackData pack)
        {
            if (controller == null || pack == null)
                return null;

            var product = controller.products.WithID(pack.ProductId);
            return product != null ? product.metadata.localizedPriceString : null;
        }

        public override void Purchase(CoinPackData pack, Action<PurchaseResult> onComplete)
        {
            if (controller == null || pack == null)
            {
                onComplete?.Invoke(PurchaseResult.Unavailable);
                return;
            }

            if (pending != null)
            {
                onComplete?.Invoke(PurchaseResult.Failed);
                return;
            }

            pending = onComplete;
            controller.InitiatePurchase(pack.ProductId);
        }

        // ---------------------------------------------------------------- IStoreListener

        public void OnInitialized(IStoreController storeController, IExtensionProvider extensions)
        {
            controller = storeController;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.LogWarning("[IAP] Initialize failed: " + error);
        }

        // newer 4.x versions also call this overload
        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.LogWarning("[IAP] Initialize failed: " + error + " " + message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var pack = FindPack(args.purchasedProduct.definition.id);

            if (pack != null)
            {
                // TODO (before release): validate args.purchasedProduct.receipt (Unity's CrossPlatformValidator or your server).
                if (pending != null)
                {
                    var callback = pending;
                    pending = null;
                    callback(PurchaseResult.Success); // ShopManager adds the coins (saved synchronously)
                }
                else
                {
                    // paid in an earlier session, the coins were never granted: grant them now
                    RaisePurchaseRestored(pack);
                }
            }

            // The coins are already saved at this point, so the purchase can be completed (consumed).
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
        {
            var callback = pending;
            pending = null;
            callback?.Invoke(reason == PurchaseFailureReason.UserCancelled ? PurchaseResult.Cancelled : PurchaseResult.Failed);
        }

        private CoinPackData FindPack(string productId)
        {
            foreach (var item in catalog.Items)
            {
                if (item is CoinPackData pack && pack.ProductId == productId)
                    return pack;
            }

            return null;
        }
    }
}
#endif
