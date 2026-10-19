using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    [CreateAssetMenu(fileName = "Gun_", menuName = "LastFlag/Shop/Gun", order = 11)]
    public class GunItemData : ShopItemData
    {
        [Header("Gun")]
        [Tooltip("Must match Gun.Id of a gun inside GunSelector.")]
        [SerializeField] private int gunId;

        public override ShopCategory Category => ShopCategory.Gun;

        public int GunId => gunId;
    }
}
