using System.Collections.Generic;
using System.Linq;
using JollyPanda.LastFlag.Shop;

namespace MJUtilities.UI
{
    /// <summary>
    /// Character customization: the player chooses, among the items he owns, the character,
    /// gun, gun skin and bullet. The 3D character on the home screen updates live (LoadoutManager).
    /// Open it with UIManager.instance.OpenPopup(PopupType.Customize).
    /// </summary>
    public class CustomizePopup : StoreListPopup
    {
        protected override ItemViewMode Mode => ItemViewMode.Customize;

        protected override IEnumerable<ShopItemData> GetItems(ShopCategory category)
        {
            var shop = ShopManager.Instance;
            if (shop == null || shop.Catalog == null)
                return Enumerable.Empty<ShopItemData>();

            var owned = shop.Catalog.GetByCategory(category).Where(shop.IsOwned);

            // A skin only fits its own gun -> list the skins of the gun that is currently equipped.
            if (category == ShopCategory.GunSkin && shop.GetEquipped(ShopCategory.Gun) is GunItemData gun)
                return owned.OfType<GunSkinItemData>().Where(s => s.GunId == gun.GunId);

            return owned;
        }

        protected override void OnItemClicked(ShopItemData item)
        {
            ShopManager.Instance?.Equip(item);
            // Equip() saves -> Informant.OnProfileChange -> cards refresh + LoadoutManager updates the scene.
        }
    }
}
