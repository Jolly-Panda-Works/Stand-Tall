using System;
using System.Globalization;
using JollyPanda.LastFlag.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MJUtilities.UI
{
    public enum ItemViewMode
    {
        /// <summary>Shop: buy things.</summary>
        Shop,
        /// <summary>Customize: equip things the player already owns.</summary>
        Customize
    }

    /// <summary>
    /// One card in the shop / customize list. Used as a prefab by <see cref="StoreListPopup"/>.
    /// Every reference except the button is optional, so the card layout is up to you.
    /// </summary>
    public class ShopItemView : MonoBehaviour
    {
        [Header("Content")]
        [SerializeField] private Image iconImage;
        [Tooltip("Shows the item's 3D model (ShopItemData.PreviewModel). The icon is used when there is no model.")]
        [SerializeField] private RawImage previewImage;
        [SerializeField] private TMP_Text nameText;
        [Tooltip("Coin packs only: shows '+1,000'.")]
        [SerializeField] private TMP_Text amountText;

        [Header("Price")]
        [SerializeField] private GameObject priceRoot;
        [SerializeField] private TMP_Text priceText;
        [SerializeField] private Color affordableColor = Color.white;
        [SerializeField] private Color unaffordableColor = new Color(1f, 0.35f, 0.35f);

        [Header("Action")]
        [SerializeField] private Button actionButton;
        [SerializeField] private TMP_Text actionLabel;

        [Header("State Badges (optional)")]
        [SerializeField] private GameObject ownedBadge;
        [SerializeField] private GameObject equippedBadge;

        private ItemViewMode mode;
        private Action<ShopItemData> onClick;

        public ShopItemData Item { get; private set; }

        private void Awake()
        {
            if (actionButton != null)
                actionButton.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            if (actionButton != null)
                actionButton.onClick.RemoveListener(HandleClick);
        }

        public void Bind(ShopItemData item, ItemViewMode viewMode, Action<ShopItemData> clickCallback)
        {
            Item = item;
            mode = viewMode;
            onClick = clickCallback;

            var preview = previewImage != null && item.PreviewModel != null
                ? ModelPreviewRenderer.Instance.GetPreview(item)
                : null;

            if (previewImage != null)
            {
                previewImage.texture = preview;
                previewImage.enabled = preview != null;
            }

            if (iconImage != null)
            {
                iconImage.sprite = item.Icon;
                iconImage.enabled = preview == null && item.Icon != null;
            }

            if (amountText != null)
            {
                bool isPack = item is CoinPackData;
                amountText.gameObject.SetActive(isPack);
                if (isPack)
                    amountText.text = "+" + ((CoinPackData)item).CoinAmount.ToString("N0", CultureInfo.InvariantCulture);
            }

            Refresh();
        }

        /// <summary>Re-reads money / ownership / equipped state. Cheap, called after every profile change.</summary>
        public void Refresh()
        {
            if (Item == null)
                return;

            var shop = ShopManager.Instance;
            if (shop == null)
                return;

            ShopLocale.SetText(nameText, ShopLocale.GetName(Item));

            if (mode == ItemViewMode.Shop)
                RefreshShopState(shop);
            else
                RefreshCustomizeState(shop);
        }

        private void RefreshShopState(ShopManager shop)
        {
            SetBadge(equippedBadge, false);

            if (Item is CoinPackData pack)
            {
                SetBadge(ownedBadge, false);
                SetPrice(true, ResolvePackPrice(shop, pack), affordableColor);

                var service = shop.PurchaseService;
                bool canBuy = service != null && service.IsAvailable && !shop.IsPurchaseInProgress;
                SetAction(ShopText.Buy, canBuy);
                return;
            }

            if (shop.IsOwned(Item))
            {
                SetBadge(ownedBadge, true);
                SetPrice(false, null, affordableColor);
                SetAction(ShopText.Owned, false);
                return;
            }

            bool canAfford = shop.CanAfford(Item);
            SetBadge(ownedBadge, false);
            SetPrice(true,
                Item.Price > 0 ? Item.Price.ToString("N0", CultureInfo.InvariantCulture) : ShopLocale.Get(ShopText.Free),
                canAfford ? affordableColor : unaffordableColor);
            // the button stays clickable when the player is short on coins, so the popup can explain why
            SetAction(ShopText.Buy, true);
        }

        private void RefreshCustomizeState(ShopManager shop)
        {
            SetPrice(false, null, affordableColor);
            SetBadge(ownedBadge, false);

            bool equipped = shop.IsEquipped(Item);
            SetBadge(equippedBadge, equipped);
            SetAction(equipped ? ShopText.Equipped : ShopText.Equip, !equipped);
        }

        private static string ResolvePackPrice(ShopManager shop, CoinPackData pack)
        {
            string price = shop.PurchaseService != null ? shop.PurchaseService.GetPriceText(pack) : null;
            return !string.IsNullOrEmpty(price) ? price : pack.FallbackPriceText;
        }

        private void SetPrice(bool visible, string text, Color color)
        {
            if (priceRoot != null)
                priceRoot.SetActive(visible);

            if (priceText != null && visible)
            {
                ShopLocale.SetText(priceText, text);
                priceText.color = color;
            }
        }

        private void SetAction(ShopText label, bool interactable)
        {
            if (actionButton != null)
                actionButton.interactable = interactable;

            ShopLocale.SetText(actionLabel, ShopLocale.Get(label));
        }

        private static void SetBadge(GameObject badge, bool visible)
        {
            if (badge != null)
                badge.SetActive(visible);
        }

        private void HandleClick()
        {
            if (Item != null)
                onClick?.Invoke(Item);
        }
    }
}
