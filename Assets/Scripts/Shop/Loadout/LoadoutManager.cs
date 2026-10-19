using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.Handlers;
using JollyPanda.LastFlag.Shop;
using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    /// <summary>
    /// Applies what the player equipped (character, gun, gun skin, bullet) to the scene.
    /// It listens to the existing profile-change event, so the 3D character in the home screen
    /// updates live while the customize popup is open.
    /// Runs before the default Start order so the gun is selected before GunUpgradeManager.Start.
    /// </summary>
    [DefaultExecutionOrder(-10)]
    public class LoadoutManager : MonoBehaviour
    {
        public static LoadoutManager Instance { get; private set; }

        [SerializeField] private GunSelector gunSelector;
        [SerializeField] private CharacterSelector characterSelector;

        private string appliedSignature;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            Informant.OnProfileChange += OnProfileChange;
        }

        private void OnDisable()
        {
            Informant.OnProfileChange -= OnProfileChange;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        private void Start()
        {
            // GunSelector.Start skips its random pick while this component exists, so we must always
            // leave a gun selected, even when no ShopManager is in the scene.
            if (ShopManager.Instance == null)
                SelectFirstGun();

            ApplyLoadout();
        }

        private void SelectFirstGun()
        {
            if (gunSelector == null || gunSelector.Guns.Count == 0 || gunSelector.Guns[0] == null)
                return;

            gunSelector.SelectGun(gunSelector.Guns[0].Id);
        }

        private void OnProfileChange(PlayerSaveData data)
        {
            ApplyLoadout();
        }

        public void ApplyLoadout()
        {
            var shop = ShopManager.Instance;
            if (shop == null || gunSelector == null)
                return;

            var character = shop.GetEquipped(ShopCategory.Character) as CharacterItemData;
            var gun = shop.GetEquipped(ShopCategory.Gun) as GunItemData;
            var bullet = shop.GetEquipped(ShopCategory.Bullet) as BulletItemData;

            // Profile changes also happen for money / wave progress. Only touch the scene when the loadout really changed.
            string signature = BuildSignature(shop, character, gun, bullet);
            if (signature == appliedSignature)
                return;
            appliedSignature = signature;

            if (character != null && characterSelector != null && characterSelector.CurrentVariantId != character.VariantId)
                characterSelector.Select(character.VariantId);

            if (gun != null)
                gunSelector.SelectGun(gun.GunId);
            else
                SelectFirstGun(); // no default gun item in the catalog: never leave the player without a gun

            foreach (var g in gunSelector.Guns)
            {
                if (g == null)
                    continue;

                var skin = shop.GetEquippedSkin(g.Id);
                g.ApplySkin(skin != null ? skin.Material : null);
                g.SetBulletPrefab(bullet != null ? bullet.BulletPrefab : null);
            }
        }

        private string BuildSignature(ShopManager shop, CharacterItemData character, GunItemData gun, BulletItemData bullet)
        {
            var sb = new System.Text.StringBuilder();
            sb.Append(character != null ? character.Id : "-").Append('|');
            sb.Append(gun != null ? gun.Id : "-").Append('|');
            sb.Append(bullet != null ? bullet.Id : "-");

            foreach (var g in gunSelector.Guns)
            {
                if (g == null)
                    continue;

                var skin = shop.GetEquippedSkin(g.Id);
                sb.Append('|').Append(g.Id).Append(':').Append(skin != null ? skin.Id : "-");
            }

            return sb.ToString();
        }
    }
}
