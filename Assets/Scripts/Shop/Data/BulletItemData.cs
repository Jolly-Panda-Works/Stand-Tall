using JollyPanda.LastFlag.PlayerModule;
using UnityEngine;

namespace JollyPanda.LastFlag.Shop
{
    [CreateAssetMenu(fileName = "Bullet_", menuName = "LastFlag/Shop/Bullet", order = 14)]
    public class BulletItemData : ShopItemData
    {
        [Header("Bullet")]
        [Tooltip("Bullet prefab (must contain the Bullet component). Empty = each gun's own default bullet.")]
        [SerializeField] private Bullet bulletPrefab;

        public override ShopCategory Category => ShopCategory.Bullet;

        public Bullet BulletPrefab => bulletPrefab;
    }
}
