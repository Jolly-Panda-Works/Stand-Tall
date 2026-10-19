using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    /// <summary>
    /// The single list of everything that exists in the shop / customize menu.
    /// Drag every item asset in here.
    /// </summary>
    [CreateAssetMenu(fileName = "ShopCatalog", menuName = "LastFlag/Shop/Catalog", order = 0)]
    public class ShopCatalog : ScriptableObject
    {
        [SerializeField] private List<ShopItemData> items = new();

        private Dictionary<string, ShopItemData> byId;

        public IReadOnlyList<ShopItemData> Items => items;

        private void OnEnable() => byId = null;

        public ShopItemData FindById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            if (byId == null)
            {
                byId = new Dictionary<string, ShopItemData>();
                foreach (var item in items)
                {
                    if (item != null && !string.IsNullOrEmpty(item.Id))
                        byId[item.Id] = item;
                }
            }

            return byId.TryGetValue(id, out var found) ? found : null;
        }

        public IEnumerable<ShopItemData> GetByCategory(ShopCategory category)
        {
            return items
                .Where(i => i != null && i.Category == category)
                .OrderBy(i => i.SortOrder)
                .ThenBy(i => i.Price);
        }

        /// <summary>First item of the category flagged as default (what a new player starts with).</summary>
        public ShopItemData GetDefault(ShopCategory category)
        {
            return GetByCategory(category).FirstOrDefault(i => i.IsDefault);
        }

        /// <summary>Default skin of one gun (a skin flagged as default that belongs to that gun).</summary>
        public GunSkinItemData GetDefaultSkin(int gunId)
        {
            return GetByCategory(ShopCategory.GunSkin)
                .OfType<GunSkinItemData>()
                .FirstOrDefault(s => s.GunId == gunId && s.IsDefault);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            byId = null;

            var seen = new HashSet<string>();
            foreach (var item in items)
            {
                if (item == null)
                    continue;

                if (string.IsNullOrEmpty(item.Id))
                    Debug.LogWarning($"[ShopCatalog] '{item.name}' has an empty Id.", item);
                else if (!seen.Add(item.Id))
                    Debug.LogError($"[ShopCatalog] Duplicate item Id '{item.Id}' ({item.name}).", item);
            }
        }
#endif
    }
}
