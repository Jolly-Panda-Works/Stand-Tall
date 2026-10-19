using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    [CreateAssetMenu(fileName = "CoinPack_", menuName = "LastFlag/Shop/Coin Pack", order = 10)]
    public class CoinPackData : ShopItemData
    {
        [Header("Coin Pack")]
        [SerializeField, Min(1)] private int coinAmount = 500;
        [Tooltip("Product / SKU id registered in the store (Bazaar, Myket, Google Play ...).")]
        [SerializeField] private string productId;
        [Tooltip("Shown only when the purchase service cannot give a localized price.")]
        [SerializeField] private string fallbackPriceText;

        public override ShopCategory Category => ShopCategory.CoinPack;

        public int CoinAmount => coinAmount;
        public string ProductId => productId;
        public string FallbackPriceText => fallbackPriceText;
    }
}
