#if UNITY_EDITOR
using System;
using System.IO;
using JollyPanda.LastFlag.Shop;
using UnityEditor;
using UnityEngine;

namespace JollyPanda.LastFlag.Shop.EditorTools
{
    /// <summary>
    /// Tools > LastFlag > Shop > Create Starter Items
    /// Creates placeholder shop assets (default character/gun/bullet/skins, a few paid items and 3 coin packs)
    /// plus a ShopCatalog that references all of them, so the scene can be wired quickly.
    /// Everything it creates is a PLACEHOLDER: set the real gunId / variantId / icons / materials / prices afterwards.
    /// Existing assets are never overwritten.
    /// </summary>
    public static class ShopSampleBuilder
    {
        private const string Folder = "Assets/ScriptableObjects/Shop";

        [MenuItem("Tools/LastFlag/Shop/Create Starter Items")]
        public static void CreateStarterItems()
        {
            EnsureFolder("Assets/ScriptableObjects");
            EnsureFolder(Folder);

            var items = new System.Collections.Generic.List<ShopItemData>();

            // ---- characters (variantId must match CharacterVariant.VariantId in the scene)
            items.Add(Make<CharacterItemData>("Character_Default", so =>
            {
                Set(so, "id", "char_default"); Set(so, "nameEnglish", "Default");
                Set(so, "isDefault", true); Set(so, "sortOrder", 0); Set(so, "variantId", "default");
            }));
            items.Add(Make<CharacterItemData>("Character_Hero2", so =>
            {
                Set(so, "id", "char_hero2"); Set(so, "nameEnglish", "Hero 2");
                Set(so, "price", 500); Set(so, "sortOrder", 1); Set(so, "variantId", "hero2");
            }));

            // ---- guns (gunId must match Gun.Id inside GunSelector)
            items.Add(Make<GunItemData>("Gun_0", so =>
            {
                Set(so, "id", "gun_0"); Set(so, "nameEnglish", "Starter Gun");
                Set(so, "isDefault", true); Set(so, "sortOrder", 0); Set(so, "gunId", 0);
            }));
            items.Add(Make<GunItemData>("Gun_1", so =>
            {
                Set(so, "id", "gun_1"); Set(so, "nameEnglish", "Gun 2");
                Set(so, "price", 400); Set(so, "sortOrder", 1); Set(so, "gunId", 1);
            }));

            // ---- skins: one default (original look) per gun + one paid example per gun
            for (int gun = 0; gun < 2; gun++)
            {
                int g = gun;
                items.Add(Make<GunSkinItemData>($"GunSkin_{g}_Default", so =>
                {
                    Set(so, "id", $"skin_{g}_default"); Set(so, "nameEnglish", "Default");
                    Set(so, "isDefault", true); Set(so, "sortOrder", 0); Set(so, "gunId", g);
                }));
                items.Add(Make<GunSkinItemData>($"GunSkin_{g}_Gold", so =>
                {
                    Set(so, "id", $"skin_{g}_gold"); Set(so, "nameEnglish", "Gold");
                    Set(so, "price", 150); Set(so, "sortOrder", 1); Set(so, "gunId", g);
                    // assign the real skin Material in the inspector
                }));
            }

            // ---- bullets (empty prefab = each gun's own bullet)
            items.Add(Make<BulletItemData>("Bullet_Default", so =>
            {
                Set(so, "id", "bullet_default"); Set(so, "nameEnglish", "Default");
                Set(so, "isDefault", true); Set(so, "sortOrder", 0);
            }));
            items.Add(Make<BulletItemData>("Bullet_Fire", so =>
            {
                Set(so, "id", "bullet_fire"); Set(so, "nameEnglish", "Fire Bullet");
                Set(so, "price", 250); Set(so, "sortOrder", 1);
                // assign a bullet prefab (with a Bullet component) in the inspector
            }));

            // ---- coin packs (productId = SKU you create in the store console)
            items.Add(MakePack("CoinPack_Small", "coins_500", 500, "29,000 Toman", 0));
            items.Add(MakePack("CoinPack_Medium", "coins_1500", 1500, "79,000 Toman", 1));
            items.Add(MakePack("CoinPack_Large", "coins_4000", 4000, "169,000 Toman", 2));

            // ---- catalog
            string catalogPath = $"{Folder}/ShopCatalog.asset";
            var catalog = AssetDatabase.LoadAssetAtPath<ShopCatalog>(catalogPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<ShopCatalog>();
                AssetDatabase.CreateAsset(catalog, catalogPath);
            }

            var catalogSo = new SerializedObject(catalog);
            var list = catalogSo.FindProperty("items");
            foreach (var item in items)
            {
                if (item == null || Contains(list, item))
                    continue;

                list.arraySize++;
                list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = item;
            }
            catalogSo.ApplyModifiedPropertiesWithoutUndo();

            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = catalog;
            Debug.Log($"[Shop] Starter items created in {Folder}. Set the real gunId/variantId/materials/prefabs/icons now.");
        }

        private static CoinPackData MakePack(string assetName, string productId, int coins, string fallbackPrice, int order)
        {
            return Make<CoinPackData>(assetName, so =>
            {
                Set(so, "id", assetName.ToLowerInvariant());
                Set(so, "nameEnglish", $"{coins} Coins");
                Set(so, "sortOrder", order);
                Set(so, "coinAmount", coins);
                Set(so, "productId", productId);
                Set(so, "fallbackPriceText", fallbackPrice);
            });
        }

        private static bool Contains(SerializedProperty list, UnityEngine.Object item)
        {
            for (int i = 0; i < list.arraySize; i++)
            {
                if (list.GetArrayElementAtIndex(i).objectReferenceValue == item)
                    return true;
            }
            return false;
        }

        private static T Make<T>(string assetName, Action<SerializedObject> setup) where T : ScriptableObject
        {
            string path = $"{Folder}/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
                return existing; // never overwrite designer edits

            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);

            var so = new SerializedObject(asset);
            setup(so);
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void Set(SerializedObject so, string field, object value)
        {
            var p = so.FindProperty(field);
            if (p == null)
            {
                Debug.LogWarning($"[Shop] Field '{field}' not found on {so.targetObject.name}");
                return;
            }

            switch (value)
            {
                case string s: p.stringValue = s; break;
                case int i: p.intValue = i; break;
                case bool b: p.boolValue = b; break;
            }
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            string parent = Path.GetDirectoryName(path).Replace('\\', '/');
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }
}
#endif
