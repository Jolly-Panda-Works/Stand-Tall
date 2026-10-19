using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    [CreateAssetMenu(fileName = "GunSkin_", menuName = "LastFlag/Shop/Gun Skin", order = 12)]
    public class GunSkinItemData : ShopItemData
    {
        [Header("Gun Skin")]
        [Tooltip("Gun.Id of the gun this skin belongs to.")]
        [SerializeField] private int gunId;
        [Tooltip("Leave empty for the 'original' skin (restores the gun's own material).")]
        [SerializeField] private Material material;

        public override ShopCategory Category => ShopCategory.GunSkin;

        public int GunId => gunId;
        public Material Material => material;
    }
}
