using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    public enum ShopCategory
    {
        CoinPack = 0,
        Gun = 1,
        GunSkin = 2,
        Character = 3,
        Bullet = 4
    }

    /// <summary>
    /// Base class of every sellable / equippable item. One ScriptableObject asset per item.
    /// IMPORTANT: <see cref="Id"/> is what is written to the save file. Never rename it after release,
    /// otherwise players lose that item.
    /// </summary>
    public abstract class ShopItemData : ScriptableObject
    {
        [Header("Identity")]
        [Tooltip("Unique and permanent. Auto-filled from the asset name when empty.")]
        [SerializeField] private string id;
        [SerializeField] private string nameEnglish;
        [SerializeField] private string namePersian;
        [SerializeField] private Sprite icon;

        [Header("Shop")]
        [Tooltip("Price in coins. Ignored for coin packs (they use the store price).")]
        [SerializeField, Min(0)] private int price;
        [Tooltip("Owned from the first launch (free, not stored in the save file).")]
        [SerializeField] private bool isDefault;
        [Tooltip("Lower comes first inside a tab.")]
        [SerializeField] private int sortOrder;

        public abstract ShopCategory Category { get; }

        public string Id => id;
        public string NameEnglish => nameEnglish;
        public string NamePersian => namePersian;
        public Sprite Icon => icon;
        public int Price => price;
        public int SortOrder => sortOrder;
        public bool IsDefault => isDefault && Category != ShopCategory.CoinPack;

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (string.IsNullOrWhiteSpace(id))
                id = name.Trim().ToLowerInvariant().Replace(' ', '_');
        }
#endif
    }
}
