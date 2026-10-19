using System.Collections.Generic;
using JollyPanda.LastFlag.Shop;

namespace MJUtilities.UI
{
    /// <summary>
    /// The shop: coin packs (real money through PurchaseServiceBase) and guns, gun skins,
    /// characters and bullets (bought with coins).
    /// Open it with UIManager.instance.OpenPopup(PopupType.Shop).
    /// </summary>
    public class ShopPopup : StoreListPopup
    {
        protected override ItemViewMode Mode => ItemViewMode.Shop;

        protected override IEnumerable<ShopItemData> GetItems(ShopCategory category)
        {
            var shop = ShopManager.Instance;
            return shop != null && shop.Catalog != null
                ? shop.Catalog.GetByCategory(category)
                : new List<ShopItemData>();
        }

        protected override void OnItemClicked(ShopItemData item)
        {
            var shop = ShopManager.Instance;
            if (shop == null)
                return;

            if (item is CoinPackData pack)
            {
                shop.BuyCoinPack(pack, OnCoinPackResult);
                RefreshCards(); // disables the buy buttons while the store dialog is open
                return;
            }

            switch (shop.TryBuy(item))
            {
                case BuyResult.Success:
                    ShowFeedback(ShopText.PurchaseSuccess);
                    break;
                case BuyResult.NotEnoughCoins:
                    ShowFeedback(ShopText.NotEnoughCoins);
                    break;
                case BuyResult.NotPurchasable:
                    ShowFeedback(ShopText.Unavailable);
                    break;
            }
        }

        private void OnCoinPackResult(PurchaseResult result)
        {
            switch (result)
            {
                case PurchaseResult.Success:
                    ShowFeedback(ShopText.PurchaseSuccess);
                    break;
                case PurchaseResult.Failed:
                    ShowFeedback(ShopText.PurchaseFailed);
                    break;
                case PurchaseResult.Unavailable:
                    ShowFeedback(ShopText.Unavailable);
                    break;
            }

            RefreshCards();
        }
    }
}
