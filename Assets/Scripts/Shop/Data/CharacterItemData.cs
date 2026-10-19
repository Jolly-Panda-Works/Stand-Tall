using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    [CreateAssetMenu(fileName = "Character_", menuName = "LastFlag/Shop/Character", order = 13)]
    public class CharacterItemData : ShopItemData
    {
        [Header("Character")]
        [Tooltip("Must match CharacterVariant.VariantId of a character model inside the scene.")]
        [SerializeField] private string variantId;

        public override ShopCategory Category => ShopCategory.Character;

        public string VariantId => variantId;
    }
}
