#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JollyPanda.LastFlag.Ads;
using JollyPanda.LastFlag.PlayerModule;
using MJUtilities.UI;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace JollyPanda.LastFlag.Shop.EditorTools
{
    /// <summary>
    /// Tools > LastFlag > Setup > Wire Game Scene
    /// Does the manual Unity steps of GUIDE_FA.md section 3 in one go, on Assets/Scenes/Game.unity:
    ///  - fixes the starter shop data (duplicate ids, gunId mismatch, a default skin per gun)
    ///  - 5 upgrade levels (T04), services (shop / mock purchase / mock rewarded ad)
    ///  - CharacterVariant / CharacterSelector / LoadoutManager / GunSkinTarget wiring
    ///  - Shop and Customize popups, the item card prefab, the Home buttons and the x2 reward button.
    /// Every step is idempotent: running it again only fills what is still missing.
    /// Batch mode: Unity -batchmode -projectPath . -executeMethod JollyPanda.LastFlag.Shop.EditorTools.LastFlagSceneSetup.RunBatch -quit
    /// </summary>
    public static class LastFlagSceneSetup
    {
        private const string ScenePath = "Assets/Scenes/Game.unity";
        private const string ShopDataFolder = "Assets/ScriptableObjects/Shop";
        private const string UiPrefabFolder = "Assets/Art/Prefabs/UserInterface/Shop";
        private const string CardPrefabPath = UiPrefabFolder + "/ShopItemCard.prefab";
        private const string ShopPopupPrefabPath = UiPrefabFolder + "/Popup_Shop.prefab";
        private const string CustomizePopupPrefabPath = UiPrefabFolder + "/Popup_Customize.prefab";
        private const string Hero2ModelPath = "Assets/PolygonMilitary/Prefabs/Characters/SM_Chr_Soldier_Male_01.prefab";
        private const string FireBulletPrefabPath = "Assets/Art/Prefabs/Bullets/Line_Red Variant.prefab";
        // Bullet.prefab is a tiny particle that renders empty: the blue line bullet looks the same in the shop
        private const string DefaultBulletPrefabPath = "Assets/Art/Prefabs/Bullets/Line_Blue Variant.prefab";
        private static readonly Vector3 SideView = new(0f, 90f, 0f);
        private static readonly Vector3 FrontView = new(0f, 180f, 0f);

        private static readonly StringBuilder Log = new();

        [MenuItem("Tools/LastFlag/Setup/Wire Game Scene")]
        public static void Run()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            RunInternal();
            EditorUtility.DisplayDialog("LastFlag setup", Log.ToString(), "OK");
        }

        /// <summary>Entry point for -executeMethod (CI or local batch mode).</summary>
        public static void RunBatch()
        {
            try
            {
                RunInternal();
                Debug.Log("[LastFlagSetup]\n" + Log);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                EditorApplication.Exit(1);
            }
        }

        private static void RunInternal()
        {
            Log.Clear();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

            var catalog = FixShopData();
            SetupServices(scene, catalog);
            SetupUpgrades(scene);
            SetupLoadout(scene, catalog);
            SetupPreviews(scene, catalog);
            SetupPopups(scene);
            SetupHomeButtons(scene);
            SetupCountdownButton(scene);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            Note("Scene saved: " + ScenePath);
        }

        // ================================================================== shop data

        private static ShopCatalog FixShopData()
        {
            var catalog = AssetDatabase.LoadAssetAtPath<ShopCatalog>(ShopDataFolder + "/ShopCatalog.asset");
            if (catalog == null)
            {
                ShopSampleBuilder.CreateStarterItems();
                catalog = AssetDatabase.LoadAssetAtPath<ShopCatalog>(ShopDataFolder + "/ShopCatalog.asset");
            }

            if (catalog == null)
                throw new Exception("ShopCatalog could not be created.");

            var catalogSo = new SerializedObject(catalog);
            var itemsProp = catalogSo.FindProperty("items");
            var items = new List<ShopItemData>();
            for (int i = 0; i < itemsProp.arraySize; i++)
            {
                if (itemsProp.GetArrayElementAtIndex(i).objectReferenceValue is ShopItemData item && !items.Contains(item))
                    items.Add(item);
            }

            // items that were created in the folder (e.g. duplicated Gun_2 / Gun_3) but never added to the catalog
            foreach (var guid in AssetDatabase.FindAssets("t:ShopItemData", new[] { ShopDataFolder }))
            {
                var item = AssetDatabase.LoadAssetAtPath<ShopItemData>(AssetDatabase.GUIDToAssetPath(guid));
                if (item != null && !items.Contains(item))
                {
                    items.Add(item);
                    Note($"{item.name} added to the catalog");
                }
            }

            // 1) unique ids (Gun_2 / Gun_3 were duplicated from Gun_1 and kept "gun_1")
            var seen = new HashSet<string>();
            foreach (var item in items)
            {
                if (seen.Add(item.Id))
                    continue;

                string newId = item.name.ToLowerInvariant();
                SetField(item, "id", newId);
                Note($"Duplicate id fixed: {item.name} -> '{newId}'");
                seen.Add(newId);
            }

            // 2) GunSkin_<k>_* belongs to the gun asset Gun_<k>: copy that gun's real Gun.Id
            var guns = items.OfType<GunItemData>().ToList();
            foreach (var skin in items.OfType<GunSkinItemData>())
            {
                var parts = skin.name.Split('_');
                if (parts.Length < 3 || parts[0] != "GunSkin")
                    continue;

                var gun = guns.FirstOrDefault(g => g.name == "Gun_" + parts[1]);
                if (gun == null || gun.GunId == skin.GunId)
                    continue;

                Note($"Skin {skin.name}: gunId {skin.GunId} -> {gun.GunId} ({gun.name})");
                SetField(skin, "gunId", gun.GunId);
            }

            // 3) every gun needs a default skin, otherwise the Skins tab of that gun is empty
            foreach (var gun in guns)
            {
                if (items.OfType<GunSkinItemData>().Any(s => s.GunId == gun.GunId && s.IsDefault))
                    continue;

                string index = gun.name.StartsWith("Gun_") ? gun.name.Substring(4) : gun.GunId.ToString();
                string path = $"{ShopDataFolder}/GunSkin_{index}_Default.asset";
                var skin = AssetDatabase.LoadAssetAtPath<GunSkinItemData>(path);
                if (skin == null)
                {
                    skin = ScriptableObject.CreateInstance<GunSkinItemData>();
                    AssetDatabase.CreateAsset(skin, path);
                }

                var so = new SerializedObject(skin);
                so.FindProperty("id").stringValue = $"skin_{index}_default";
                so.FindProperty("nameEnglish").stringValue = "Default";
                so.FindProperty("isDefault").boolValue = true;
                so.FindProperty("gunId").intValue = gun.GunId;
                so.ApplyModifiedPropertiesWithoutUndo();
                if (!items.Contains(skin))
                    items.Add(skin);
                Note($"Default skin created for {gun.name} (gunId {gun.GunId})");
            }

            // 4) give the sample paid items something visible
            var fireBullet = items.OfType<BulletItemData>().FirstOrDefault(b => !b.IsDefault && b.BulletPrefab == null);
            var firePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(FireBulletPrefabPath);
            if (fireBullet != null && firePrefab != null && firePrefab.GetComponent<JollyPanda.LastFlag.PlayerModule.Bullet>() != null)
            {
                SetField(fireBullet, "bulletPrefab", firePrefab.GetComponent<JollyPanda.LastFlag.PlayerModule.Bullet>());
                Note($"{fireBullet.name}: bullet prefab = {firePrefab.name}");
            }

            var gold = FindGoldMaterial();
            foreach (var skin in items.OfType<GunSkinItemData>().Where(s => !s.IsDefault && s.Material == null))
            {
                if (gold == null)
                    break;
                SetField(skin, "material", gold);
                Note($"{skin.name}: material = {gold.name}");
            }

            // write the (possibly extended) item list back
            itemsProp.arraySize = items.Count;
            for (int i = 0; i < items.Count; i++)
                itemsProp.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
            catalogSo.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(catalog);
            return catalog;
        }

        private static Material FindGoldMaterial()
        {
            foreach (var guid in AssetDatabase.FindAssets("gold t:Material"))
            {
                var mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
                if (mat != null)
                    return mat;
            }

            // nothing in the project: make a simple metallic yellow material with the gun's shader family
            const string path = ShopDataFolder + "/Materials/GunSkin_Gold.mat";
            var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (existing != null)
                return existing;

            EnsureFolder(ShopDataFolder + "/Materials");
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (shader == null)
                return null;

            var gold = new Material(shader) { name = "GunSkin_Gold", color = new Color(1f, 0.78f, 0.2f) };
            if (gold.HasProperty("_Metallic")) gold.SetFloat("_Metallic", 0.9f);
            if (gold.HasProperty("_Smoothness")) gold.SetFloat("_Smoothness", 0.75f);
            if (gold.HasProperty("_Glossiness")) gold.SetFloat("_Glossiness", 0.75f);
            AssetDatabase.CreateAsset(gold, path);
            return gold;
        }

        // ================================================================== services

        private static void SetupServices(Scene scene, ShopCatalog catalog)
        {
            var shop = FindInScene<ShopManager>(scene).FirstOrDefault();
            var mockPurchase = FindInScene<MockPurchaseService>(scene).FirstOrDefault();

            GameObject host;
            if (shop != null)
                host = shop.gameObject;
            else if (mockPurchase != null)
                host = mockPurchase.gameObject;
            else
            {
                host = new GameObject("ShopServices");
                SceneManager.MoveGameObjectToScene(host, scene);
            }

            if (shop == null)
                shop = host.AddComponent<ShopManager>();
            if (mockPurchase == null)
                mockPurchase = host.AddComponent<MockPurchaseService>();

            var so = new SerializedObject(shop);
            if (so.FindProperty("catalog").objectReferenceValue == null)
                so.FindProperty("catalog").objectReferenceValue = catalog;
            if (so.FindProperty("purchaseService").objectReferenceValue == null)
                so.FindProperty("purchaseService").objectReferenceValue = mockPurchase;
            so.ApplyModifiedPropertiesWithoutUndo();

            if (!FindInScene<RewardedAdServiceBase>(scene).Any())
            {
                mockPurchase.gameObject.AddComponent<MockRewardedAdService>();
                Note("MockRewardedAdService added to " + mockPurchase.gameObject.name);
            }

            Note($"ShopManager on '{shop.gameObject.name}' (catalog + MockPurchaseService)");
        }

        // ================================================================== upgrades (T04)

        private static void SetupUpgrades(Scene scene)
        {
            foreach (var manager in FindInScene<GunUpgradeManager>(scene))
            {
                var so = new SerializedObject(manager);
                foreach (var field in new[] { "fireRateUpgrade", "damageUpgrade", "bulletSizeUpgrade", "bulletSpeedUpgrade" })
                {
                    var values = so.FindProperty(field + ".<Values>k__BackingField");
                    if (values == null || values.arraySize >= 5)
                        continue;

                    int old = values.arraySize;
                    while (values.arraySize < 5)
                    {
                        int n = values.arraySize;
                        values.arraySize = n + 1;
                        var last = n >= 1 ? values.GetArrayElementAtIndex(n - 1) : null;
                        var prev = n >= 2 ? values.GetArrayElementAtIndex(n - 2) : null;
                        var added = values.GetArrayElementAtIndex(n);

                        int lastCost = last?.FindPropertyRelative("cost").intValue ?? 0;
                        float lastValue = last?.FindPropertyRelative("value").floatValue ?? 0f;
                        float prevValue = prev?.FindPropertyRelative("value").floatValue ?? lastValue;

                        // cost grows 50 %, value keeps the same step as the previous level
                        added.FindPropertyRelative("cost").intValue = Mathf.CeilToInt(Mathf.Max(10, lastCost * 1.5f) / 10f) * 10;
                        added.FindPropertyRelative("value").floatValue = lastValue + (lastValue - prevValue);
                    }

                    Note($"{field}: Values {old} -> 5 (check the new last level in the inspector)");
                }

                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // ================================================================== loadout (S03)

        private static void SetupLoadout(Scene scene, ShopCatalog catalog)
        {
            var gunSelector = FindInScene<GunSelector>(scene).FirstOrDefault(g => g.gameObject.activeInHierarchy)
                              ?? FindInScene<GunSelector>(scene).FirstOrDefault();
            if (gunSelector == null)
            {
                Note("WARNING: no GunSelector in the scene, loadout not wired.");
                return;
            }

            var animationController = FindInScene<PlayerAnimationController>(scene).FirstOrDefault(a => a.gameObject.activeInHierarchy)
                                      ?? FindInScene<PlayerAnimationController>(scene).FirstOrDefault();

            // the default model is the object whose Animator drives the player
            Animator defaultAnimator = null;
            if (animationController != null)
            {
                var acSo = new SerializedObject(animationController);
                defaultAnimator = acSo.FindProperty("animator").objectReferenceValue as Animator
                                  ?? animationController.GetComponent<Animator>();
            }

            // --- GunSkinTarget on every gun
            foreach (var gun in gunSelector.GetComponentsInChildren<Gun>(true))
            {
                if (gun.GetComponent<GunSkinTarget>() != null)
                    continue;
                gun.gameObject.AddComponent<GunSkinTarget>(); // renderers are collected from children at runtime
                Note("GunSkinTarget added to " + gun.name);
            }

            var characterSelector = FindInScene<CharacterSelector>(scene).FirstOrDefault();
            if (characterSelector == null)
                characterSelector = gunSelector.transform.root.gameObject.AddComponent<CharacterSelector>();

            var loadout = FindInScene<LoadoutManager>(scene).FirstOrDefault();
            if (loadout == null)
                loadout = characterSelector.gameObject.AddComponent<LoadoutManager>();

            var loSo = new SerializedObject(loadout);
            loSo.FindProperty("gunSelector").objectReferenceValue = gunSelector;
            loSo.FindProperty("characterSelector").objectReferenceValue = characterSelector;
            loSo.ApplyModifiedPropertiesWithoutUndo();

            var csSo = new SerializedObject(characterSelector);
            csSo.FindProperty("gunSelector").objectReferenceValue = gunSelector;
            csSo.FindProperty("animationController").objectReferenceValue = animationController;

            var variants = new List<CharacterVariant>();
            if (defaultAnimator != null)
            {
                var gunMount = FirstGunParent(gunSelector);
                var defaultVariant = EnsureVariant(defaultAnimator.gameObject, "default", defaultAnimator, gunMount);
                variants.Add(defaultVariant);

                // every other character item of the catalog gets a model, if it can be built safely
                foreach (var character in catalog.Items.OfType<CharacterItemData>())
                {
                    if (character.VariantId == "default")
                        continue;

                    var variant = FindInScene<CharacterVariant>(scene).FirstOrDefault(v => v.VariantId == character.VariantId)
                                  ?? CreateVariant(character.VariantId, defaultAnimator, gunMount);
                    if (variant != null)
                        variants.Add(variant);
                    else
                        Note($"WARNING: no model for character '{character.VariantId}'. Add a CharacterVariant manually.");
                }
            }
            else
            {
                Note("WARNING: the player Animator was not found, CharacterVariants not created.");
            }

            var variantsProp = csSo.FindProperty("variants");
            variantsProp.arraySize = variants.Count;
            for (int i = 0; i < variants.Count; i++)
                variantsProp.GetArrayElementAtIndex(i).objectReferenceValue = variants[i];
            csSo.ApplyModifiedPropertiesWithoutUndo();

            Note($"Loadout wired: {variants.Count} character variant(s), GunSelector '{gunSelector.name}'");
        }

        private static Transform FirstGunParent(GunSelector selector)
        {
            var gun = selector.GetComponentsInChildren<Gun>(true).FirstOrDefault();
            return gun != null ? gun.transform.parent : null;
        }

        private static CharacterVariant EnsureVariant(GameObject model, string id, Animator animator, Transform gunMount)
        {
            var variant = model.GetComponent<CharacterVariant>() ?? model.AddComponent<CharacterVariant>();
            var so = new SerializedObject(variant);
            so.FindProperty("variantId").stringValue = id;
            so.FindProperty("animator").objectReferenceValue = animator;
            so.FindProperty("gunMount").objectReferenceValue = gunMount;
            so.ApplyModifiedPropertiesWithoutUndo();
            return variant;
        }

        private static CharacterVariant CreateVariant(string id, Animator defaultAnimator, Transform defaultGunMount)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Hero2ModelPath);
            if (prefab == null)
                return null;

            var defaultModel = defaultAnimator.transform;
            var model = (GameObject)PrefabUtility.InstantiatePrefab(prefab, defaultModel.parent);
            model.name = "Character_" + id;
            model.transform.localPosition = defaultModel.localPosition;
            model.transform.localRotation = defaultModel.localRotation;
            model.transform.localScale = defaultModel.localScale;
            model.layer = defaultModel.gameObject.layer;

            var animator = model.GetComponent<Animator>() ?? model.AddComponent<Animator>();
            animator.runtimeAnimatorController = defaultAnimator.runtimeAnimatorController;
            if (animator.avatar == null)
                animator.avatar = defaultAnimator.avatar;
            animator.applyRootMotion = defaultAnimator.applyRootMotion;
            animator.cullingMode = defaultAnimator.cullingMode;

            // the guns hang under a hand bone of the default model: build the same mount on the same bone
            Transform mount = null;
            if (defaultGunMount != null && defaultGunMount.IsChildOf(defaultModel) && defaultGunMount != defaultModel)
            {
                var bone = defaultGunMount.parent;
                var newBone = model.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == bone.name);
                if (newBone == null)
                {
                    Object.DestroyImmediate(model);
                    return null;
                }

                var go = new GameObject("GunMount");
                mount = go.transform;
                mount.SetParent(newBone, false);
                mount.localPosition = defaultGunMount.localPosition;
                mount.localRotation = defaultGunMount.localRotation;
                mount.localScale = defaultGunMount.localScale;
            }

            model.SetActive(false); // CharacterSelector shows it when equipped
            Note($"Character model '{prefab.name}' added as variant '{id}'");
            return EnsureVariant(model, id, animator, mount);
        }

        // ================================================================== 3D previews (RawImage)

        private static void SetupPreviews(Scene scene, ShopCatalog catalog)
        {
            var shop = FindInScene<ShopManager>(scene).FirstOrDefault();
            var previewRenderer = FindInScene<ModelPreviewRenderer>(scene).FirstOrDefault();
            if (previewRenderer == null && shop != null)
                previewRenderer = shop.gameObject.AddComponent<ModelPreviewRenderer>();

            var animationController = FindInScene<PlayerAnimationController>(scene).FirstOrDefault(a => a.gameObject.activeInHierarchy)
                                      ?? FindInScene<PlayerAnimationController>(scene).FirstOrDefault();
            if (previewRenderer != null && animationController != null)
            {
                var animator = new SerializedObject(animationController).FindProperty("animator").objectReferenceValue as Animator
                               ?? animationController.GetComponent<Animator>();
                var rSo = new SerializedObject(previewRenderer);
                if (animator != null && rSo.FindProperty("characterController").objectReferenceValue == null)
                    rSo.FindProperty("characterController").objectReferenceValue = animator.runtimeAnimatorController;
                rSo.ApplyModifiedPropertiesWithoutUndo();
            }

            var variants = FindInScene<CharacterVariant>(scene).ToList();
            var guns = FindInScene<Gun>(scene).ToList();
            var defaultBullet = AssetDatabase.LoadAssetAtPath<GameObject>(DefaultBulletPrefabPath);

            foreach (var item in catalog.Items)
            {
                if (item == null || item.PreviewModel != null)
                    continue;

                GameObject model = null;
                Vector3 rotation = Vector3.zero;
                switch (item)
                {
                    case CharacterItemData character:
                        var variant = variants.FirstOrDefault(v => v.VariantId == character.VariantId);
                        model = variant != null ? AssetRoot(variant.gameObject) : null;
                        rotation = FrontView;
                        break;
                    case GunItemData gunItem:
                        model = GunModel(guns, gunItem.GunId);
                        rotation = SideView;
                        break;
                    case GunSkinItemData skin:
                        model = GunModel(guns, skin.GunId);
                        rotation = SideView;
                        break;
                    case BulletItemData bullet:
                        model = bullet.BulletPrefab != null ? bullet.BulletPrefab.gameObject : defaultBullet;
                        rotation = SideView;
                        break;
                }

                if (model == null)
                    continue;

                var so = new SerializedObject(item);
                so.FindProperty("previewModel").objectReferenceValue = model;
                so.FindProperty("previewRotation").vector3Value = rotation;
                so.ApplyModifiedPropertiesWithoutUndo();
                EditorUtility.SetDirty(item);
                Note($"{item.name}: preview model = {model.name}");
            }
        }

        private static GameObject GunModel(List<Gun> guns, int gunId)
        {
            var gun = guns.FirstOrDefault(g => g.Id == gunId);
            return gun != null ? AssetRoot(gun.gameObject) : null;
        }

        /// <summary>The prefab asset a scene object comes from (walking up nested prefabs to the first asset it is the root of).</summary>
        private static GameObject AssetRoot(GameObject sceneObject)
        {
            var source = PrefabUtility.GetCorrespondingObjectFromSource(sceneObject);
            while (source != null && source.transform.parent != null)
                source = PrefabUtility.GetCorrespondingObjectFromSource(source);
            return source;
        }

        /// <summary>Cards made before the 3D previews existed get their RawImage here.</summary>
        private static void EnsureCardPreview(string prefabPath)
        {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                var view = root.GetComponent<ShopItemView>();
                if (view == null)
                    return;

                var so = new SerializedObject(view);
                var prop = so.FindProperty("previewImage");
                if (prop == null || prop.objectReferenceValue != null)
                    return;

                var icon = so.FindProperty("iconImage").objectReferenceValue as Image;
                var preview = NewUi("Preview", root.transform).gameObject.AddComponent<RawImage>();
                preview.raycastTarget = false;
                Stretch(preview.rectTransform, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.95f));
                if (icon != null)
                    preview.rectTransform.SetSiblingIndex(icon.rectTransform.GetSiblingIndex() + 1);

                prop.objectReferenceValue = preview;
                so.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
                Note("Card prefab: RawImage preview added");
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }

        // ================================================================== popups (S04)

        private class Style
        {
            public TMP_FontAsset Font;
            public Sprite PanelSprite;
            public Sprite ButtonSprite;
            public Sprite CoinSprite;
            public Button BackButtonTemplate;
            public Button ButtonTemplate;
        }

        private static void SetupPopups(Scene scene)
        {
            var uiManager = FindInScene<UIManager>(scene).FirstOrDefault();
            if (uiManager == null)
            {
                Note("WARNING: UIManager not found, popups not created.");
                return;
            }

            var upgradePopup = FindInScene<UpgradePopup>(scene).FirstOrDefault();
            var style = BuildStyle(scene, upgradePopup);
            var card = EnsureCardPrefab(style);

            var parent = upgradePopup != null ? upgradePopup.transform.parent : uiManager.transform;
            EnsurePopup<ShopPopup>(scene, uiManager, parent, PopupType.Shop, ShopText.Shop, ShopPopupPrefabPath, style, card,
                new[] { ShopCategory.CoinPack, ShopCategory.Gun, ShopCategory.Character, ShopCategory.GunSkin, ShopCategory.Bullet },
                withFreeCoins: true);
            EnsurePopup<CustomizePopup>(scene, uiManager, parent, PopupType.Customize, ShopText.Customize, CustomizePopupPrefabPath, style, card,
                new[] { ShopCategory.Character, ShopCategory.Gun, ShopCategory.GunSkin, ShopCategory.Bullet },
                withFreeCoins: false);
        }

        private static Style BuildStyle(Scene scene, UpgradePopup upgradePopup)
        {
            var style = new Style();
            var anyText = upgradePopup != null
                ? upgradePopup.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault()
                : FindInScene<TMP_Text>(scene).FirstOrDefault();
            style.Font = anyText != null ? anyText.font : TMP_Settings.defaultFontAsset;

            if (upgradePopup != null)
            {
                var popupRect = (RectTransform)upgradePopup.transform;
                // panel = the biggest sliced image that is not full screen
                style.PanelSprite = upgradePopup.GetComponentsInChildren<Image>(true)
                    .Where(i => i.sprite != null && i.transform != popupRect && i.type == Image.Type.Sliced)
                    .OrderByDescending(i => i.rectTransform.rect.width * i.rectTransform.rect.height)
                    .Select(i => i.sprite).FirstOrDefault();

                var upSo = new SerializedObject(upgradePopup);
                style.BackButtonTemplate = upSo.FindProperty("backButton")?.objectReferenceValue as Button;
                style.ButtonTemplate = upSo.FindProperty("fireRateUpgradeButton")?.objectReferenceValue as Button;
                if (style.ButtonTemplate != null && style.ButtonTemplate.image != null)
                    style.ButtonSprite = style.ButtonTemplate.image.sprite;
            }

            var home = FindInScene<HomePage>(scene).FirstOrDefault();
            if (home != null)
            {
                var money = new SerializedObject(home).FindProperty("moneyText").objectReferenceValue as TMP_Text;
                if (money != null && money.transform.parent != null)
                    style.CoinSprite = money.transform.parent.GetComponentsInChildren<Image>(true)
                        .Where(i => i.sprite != null && i.gameObject != money.gameObject)
                        .OrderBy(i => i.rectTransform.rect.width * i.rectTransform.rect.height)
                        .Select(i => i.sprite).FirstOrDefault();
            }

            return style;
        }

        private static void EnsurePopup<T>(Scene scene, UIManager uiManager, Transform parent, PopupType type, ShopText title,
            string prefabPath, Style style, ShopItemView card, ShopCategory[] categories, bool withFreeCoins) where T : StoreListPopup
        {
            var popup = FindInScene<T>(scene).FirstOrDefault();
            if (popup == null)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if (prefab == null)
                {
                    var built = BuildPopup<T>(type, title, style, card, categories, withFreeCoins);
                    EnsureFolder(UiPrefabFolder);
                    prefab = PrefabUtility.SaveAsPrefabAsset(built, prefabPath);
                    Object.DestroyImmediate(built);
                    Note("Prefab created: " + prefabPath);
                }

                var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
                instance.transform.SetAsLastSibling();
                instance.SetActive(false);
                popup = instance.GetComponent<T>();
                Note($"{typeof(T).Name} added under '{parent.name}'");
            }

            var uiSo = new SerializedObject(uiManager);
            var list = uiSo.FindProperty("popupList");
            for (int i = 0; i < list.arraySize; i++)
            {
                if (list.GetArrayElementAtIndex(i).objectReferenceValue == popup)
                    return;
            }

            list.arraySize++;
            list.GetArrayElementAtIndex(list.arraySize - 1).objectReferenceValue = popup;
            uiSo.ApplyModifiedPropertiesWithoutUndo();
            Note($"{typeof(T).Name} registered in UIManager.popupList");
        }

        private static GameObject BuildPopup<T>(PopupType type, ShopText title, Style style, ShopItemView card,
            ShopCategory[] categories, bool withFreeCoins) where T : StoreListPopup
        {
            var root = NewUi("Popup_" + type, null);
            Stretch(root, Vector2.zero, Vector2.one);
            var dim = root.gameObject.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.65f); // also blocks touches to the game behind

            var panel = NewUi("Panel", root);
            Stretch(panel, new Vector2(0.05f, 0.1f), new Vector2(0.95f, 0.9f));
            var panelImage = panel.gameObject.AddComponent<Image>();
            ApplySprite(panelImage, style.PanelSprite, new Color(0.16f, 0.2f, 0.3f, 0.97f));

            // header: title + money + back
            var titleText = NewText("Title", panel, style, 64, TextAlignmentOptions.Center);
            Stretch(titleText.rectTransform, new Vector2(0.15f, 0.9f), new Vector2(0.85f, 0.98f));
            titleText.gameObject.AddComponent<ShopLocaleLabel>().SetKey(title);

            var moneyRow = NewUi("Money", panel);
            Stretch(moneyRow, new Vector2(0.05f, 0.84f), new Vector2(0.5f, 0.9f));
            var coin = NewUi("CoinIcon", moneyRow).gameObject.AddComponent<Image>();
            coin.preserveAspect = true;
            ApplySprite(coin, style.CoinSprite, new Color(1f, 0.8f, 0.2f));
            Stretch(coin.rectTransform, new Vector2(0f, 0f), new Vector2(0.18f, 1f));
            var moneyText = NewText("MoneyText", moneyRow, style, 44, TextAlignmentOptions.Left);
            Stretch(moneyText.rectTransform, new Vector2(0.22f, 0f), new Vector2(1f, 1f));

            Button back;
            if (style.BackButtonTemplate != null)
            {
                back = Object.Instantiate(style.BackButtonTemplate, panel);
                back.name = "BackButton";
                back.onClick.RemoveAllListeners();
            }
            else
            {
                back = NewButton("BackButton", panel, style, "X", 48);
            }

            var backRect = (RectTransform)back.transform;
            backRect.anchorMin = backRect.anchorMax = new Vector2(1f, 1f);
            backRect.pivot = new Vector2(1f, 1f);
            backRect.anchoredPosition = new Vector2(-20f, -20f);
            if (backRect.sizeDelta.x < 80f)
                backRect.sizeDelta = new Vector2(120f, 120f);

            // free coins (rewarded video) next to the money label
            if (withFreeCoins)
            {
                var freeButton = NewButton("FreeCoinsButton", panel, style, "", 34);
                Stretch((RectTransform)freeButton.transform, new Vector2(0.55f, 0.84f), new Vector2(0.95f, 0.9f));
                var label = freeButton.GetComponentInChildren<TMP_Text>(true);
                label.gameObject.AddComponent<ShopLocaleLabel>().SetKey(ShopText.FreeCoins);
                var rewarded = freeButton.gameObject.AddComponent<RewardedCoinsButton>();
                var rSo = new SerializedObject(rewarded);
                rSo.FindProperty("button").objectReferenceValue = freeButton;
                rSo.ApplyModifiedPropertiesWithoutUndo();
            }

            // tabs
            var tabRow = NewUi("Tabs", panel);
            Stretch(tabRow, new Vector2(0.03f, 0.76f), new Vector2(0.97f, 0.83f));
            var tabLayout = tabRow.gameObject.AddComponent<HorizontalLayoutGroup>();
            tabLayout.spacing = 10;
            tabLayout.childForceExpandWidth = true;
            tabLayout.childForceExpandHeight = true;
            tabLayout.childControlWidth = true;
            tabLayout.childControlHeight = true;

            var tabEntries = new List<(ShopCategory category, Button button, TMP_Text label, GameObject marker)>();
            foreach (var category in categories)
            {
                var tab = NewButton("Tab_" + category, tabRow, style, category.ToString(), 32);
                var marker = NewUi("Selected", tab.transform);
                Stretch(marker, new Vector2(0.1f, 0f), new Vector2(0.9f, 0.08f));
                marker.gameObject.AddComponent<Image>().color = new Color(1f, 0.85f, 0.25f);
                marker.gameObject.SetActive(false);
                tabEntries.Add((category, tab, tab.GetComponentInChildren<TMP_Text>(true), marker.gameObject));
            }

            // list
            var scroll = NewUi("ScrollView", panel);
            Stretch(scroll, new Vector2(0.03f, 0.08f), new Vector2(0.97f, 0.75f));
            var scrollRect = scroll.gameObject.AddComponent<ScrollRect>();
            scrollRect.horizontal = false;
            scrollRect.movementType = ScrollRect.MovementType.Elastic;

            var viewport = NewUi("Viewport", scroll);
            Stretch(viewport, Vector2.zero, Vector2.one);
            viewport.gameObject.AddComponent<RectMask2D>();
            var viewportImage = viewport.gameObject.AddComponent<Image>(); // raycast target for dragging
            viewportImage.color = new Color(0f, 0f, 0f, 0.001f);

            var content = NewUi("Content", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.sizeDelta = Vector2.zero;
            var grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(440, 560);
            grid.spacing = new Vector2(24, 24);
            grid.padding = new RectOffset(16, 16, 16, 16);
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 2;
            content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            scrollRect.viewport = viewport;
            scrollRect.content = content;

            var emptyLabel = NewText("EmptyLabel", scroll, style, 40, TextAlignmentOptions.Center);
            Stretch(emptyLabel.rectTransform, new Vector2(0f, 0.4f), new Vector2(1f, 0.6f));
            emptyLabel.gameObject.SetActive(false);

            var feedback = NewText("Feedback", panel, style, 40, TextAlignmentOptions.Center);
            Stretch(feedback.rectTransform, new Vector2(0.05f, 0.01f), new Vector2(0.95f, 0.07f));
            feedback.color = new Color(1f, 0.9f, 0.4f);
            feedback.gameObject.SetActive(false);

            // component
            var popup = root.gameObject.AddComponent<T>();
            var so = new SerializedObject(popup);
            so.FindProperty("popupType").enumValueIndex = Array.IndexOf(Enum.GetValues(typeof(PopupType)), type);
            so.FindProperty("backButton").objectReferenceValue = back;
            so.FindProperty("moneyText").objectReferenceValue = moneyText;
            so.FindProperty("itemPrefab").objectReferenceValue = card;
            so.FindProperty("content").objectReferenceValue = content;
            so.FindProperty("scrollRect").objectReferenceValue = scrollRect;
            so.FindProperty("emptyLabelRoot").objectReferenceValue = emptyLabel.gameObject;
            so.FindProperty("emptyLabel").objectReferenceValue = emptyLabel;
            so.FindProperty("feedbackText").objectReferenceValue = feedback;

            var tabs = so.FindProperty("tabs");
            tabs.arraySize = tabEntries.Count;
            for (int i = 0; i < tabEntries.Count; i++)
            {
                var entry = tabs.GetArrayElementAtIndex(i);
                entry.FindPropertyRelative("category").enumValueIndex =
                    Array.IndexOf(Enum.GetValues(typeof(ShopCategory)), tabEntries[i].category);
                entry.FindPropertyRelative("button").objectReferenceValue = tabEntries[i].button;
                entry.FindPropertyRelative("label").objectReferenceValue = tabEntries[i].label;
                entry.FindPropertyRelative("selectedMarker").objectReferenceValue = tabEntries[i].marker;
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            return root.gameObject;
        }

        private static ShopItemView EnsureCardPrefab(Style style)
        {
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(CardPrefabPath);
            if (existing != null)
            {
                EnsureCardPreview(CardPrefabPath);
                return AssetDatabase.LoadAssetAtPath<GameObject>(CardPrefabPath).GetComponent<ShopItemView>();
            }

            var root = NewUi("ShopItemCard", null);
            root.sizeDelta = new Vector2(440, 560);
            var bg = root.gameObject.AddComponent<Image>();
            ApplySprite(bg, style.PanelSprite, new Color(0.24f, 0.29f, 0.42f));

            var icon = NewUi("Icon", root).gameObject.AddComponent<Image>();
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            Stretch(icon.rectTransform, new Vector2(0.15f, 0.45f), new Vector2(0.85f, 0.9f));

            var preview = NewUi("Preview", root).gameObject.AddComponent<RawImage>();
            preview.raycastTarget = false;
            Stretch(preview.rectTransform, new Vector2(0.08f, 0.45f), new Vector2(0.92f, 0.95f));

            var nameText = NewText("Name", root, style, 36, TextAlignmentOptions.Center);
            Stretch(nameText.rectTransform, new Vector2(0.05f, 0.34f), new Vector2(0.95f, 0.45f));

            var amount = NewText("Amount", root, style, 48, TextAlignmentOptions.Center);
            Stretch(amount.rectTransform, new Vector2(0.05f, 0.55f), new Vector2(0.95f, 0.75f));
            amount.color = new Color(1f, 0.85f, 0.3f);

            var priceRoot = NewUi("Price", root);
            Stretch(priceRoot, new Vector2(0.1f, 0.22f), new Vector2(0.9f, 0.33f));
            var coin = NewUi("Coin", priceRoot).gameObject.AddComponent<Image>();
            coin.preserveAspect = true;
            ApplySprite(coin, style.CoinSprite, new Color(1f, 0.8f, 0.2f));
            Stretch(coin.rectTransform, new Vector2(0f, 0f), new Vector2(0.22f, 1f));
            var priceText = NewText("PriceText", priceRoot, style, 38, TextAlignmentOptions.Left);
            Stretch(priceText.rectTransform, new Vector2(0.26f, 0f), new Vector2(1f, 1f));

            var action = NewButton("ActionButton", root, style, "Buy", 36);
            Stretch((RectTransform)action.transform, new Vector2(0.1f, 0.04f), new Vector2(0.9f, 0.19f));

            var owned = NewText("OwnedBadge", root, style, 30, TextAlignmentOptions.TopRight);
            Stretch(owned.rectTransform, new Vector2(0.5f, 0.88f), new Vector2(0.96f, 0.98f));
            owned.text = "✓";
            owned.color = new Color(0.45f, 1f, 0.45f);

            var equipped = NewText("EquippedBadge", root, style, 30, TextAlignmentOptions.TopRight);
            Stretch(equipped.rectTransform, new Vector2(0.5f, 0.88f), new Vector2(0.96f, 0.98f));
            equipped.text = "★";
            equipped.color = new Color(1f, 0.85f, 0.25f);

            var view = root.gameObject.AddComponent<ShopItemView>();
            var so = new SerializedObject(view);
            so.FindProperty("iconImage").objectReferenceValue = icon;
            so.FindProperty("previewImage").objectReferenceValue = preview;
            so.FindProperty("nameText").objectReferenceValue = nameText;
            so.FindProperty("amountText").objectReferenceValue = amount;
            so.FindProperty("priceRoot").objectReferenceValue = priceRoot.gameObject;
            so.FindProperty("priceText").objectReferenceValue = priceText;
            so.FindProperty("actionButton").objectReferenceValue = action;
            so.FindProperty("actionLabel").objectReferenceValue = action.GetComponentInChildren<TMP_Text>(true);
            so.FindProperty("ownedBadge").objectReferenceValue = owned.gameObject;
            so.FindProperty("equippedBadge").objectReferenceValue = equipped.gameObject;
            so.ApplyModifiedPropertiesWithoutUndo();

            EnsureFolder(UiPrefabFolder);
            var prefab = PrefabUtility.SaveAsPrefabAsset(root.gameObject, CardPrefabPath);
            Object.DestroyImmediate(root.gameObject);
            Note("Prefab created: " + CardPrefabPath);
            return prefab.GetComponent<ShopItemView>();
        }

        // ================================================================== home + countdown buttons

        private static void SetupHomeButtons(Scene scene)
        {
            var home = FindInScene<HomePage>(scene).FirstOrDefault();
            if (home == null)
            {
                Note("WARNING: HomePage not found.");
                return;
            }

            var so = new SerializedObject(home);
            var upgrade = so.FindProperty("upgradeButton").objectReferenceValue as Button;
            if (upgrade == null)
            {
                Note("WARNING: HomePage.upgradeButton is empty, Shop/Customize buttons not created.");
                return;
            }

            var height = ((RectTransform)upgrade.transform).rect.height;
            var gap = Mathf.Max(height, 120f) + 30f;

            if (so.FindProperty("shopButton").objectReferenceValue == null)
            {
                so.FindProperty("shopButton").objectReferenceValue = CloneButton(upgrade, "ShopButton", ShopText.Shop, gap);
                Note("Home: Shop button created (copy of the Upgrade button, placed above it)");
            }

            if (so.FindProperty("customizeButton").objectReferenceValue == null)
            {
                so.FindProperty("customizeButton").objectReferenceValue = CloneButton(upgrade, "CustomizeButton", ShopText.Customize, gap * 2f);
                Note("Home: Customize button created (copy of the Upgrade button, placed above Shop)");
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetupCountdownButton(Scene scene)
        {
            var countdown = FindInScene<CountdownPopup>(scene).FirstOrDefault();
            if (countdown == null)
                return;

            var so = new SerializedObject(countdown);
            if (so.FindProperty("doubleRewardButton").objectReferenceValue != null)
                return;

            var start = so.FindProperty("startWaveButton").objectReferenceValue as Button;
            if (start == null)
                return;

            var height = ((RectTransform)start.transform).rect.height;
            so.FindProperty("doubleRewardButton").objectReferenceValue =
                CloneButton(start, "DoubleRewardButton", ShopText.DoubleReward, -(Mathf.Max(height, 100f) + 30f));
            so.ApplyModifiedPropertiesWithoutUndo();
            Note("Countdown: x2 reward button created (copy of Start Wave, placed below it)");
        }

        /// <summary>Copies a button (keeps its look and click sound), drops its old listeners and LocalizedText.</summary>
        private static Button CloneButton(Button template, string name, ShopText label, float offsetY)
        {
            var copy = Object.Instantiate(template.gameObject, template.transform.parent);
            copy.name = name;
            copy.transform.SetSiblingIndex(template.transform.GetSiblingIndex() + 1);
            var rect = (RectTransform)copy.transform;
            rect.anchoredPosition += new Vector2(0f, offsetY);

            var button = copy.GetComponent<Button>();
            // persistent (inspector) listeners of the template would also fire on the copy
            for (int i = button.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
                UnityEditor.Events.UnityEventTools.RemovePersistentListener(button.onClick, i);

            foreach (var localized in copy.GetComponentsInChildren<MonoBehaviour>(true)
                         .Where(m => m != null && m.GetType().Name == "LocalizedText").ToArray())
                Object.DestroyImmediate(localized);

            var text = copy.GetComponentsInChildren<TMP_Text>(true).FirstOrDefault();
            if (text != null)
                (text.GetComponent<ShopLocaleLabel>() ?? text.gameObject.AddComponent<ShopLocaleLabel>()).SetKey(label);

            return button;
        }

        // ================================================================== ui helpers

        private static RectTransform NewUi(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI") >= 0 ? LayerMask.NameToLayer("UI") : 5;
            if (parent != null)
                go.transform.SetParent(parent, false);
            return (RectTransform)go.transform;
        }

        private static void Stretch(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = min;
            rect.anchorMax = max;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static TMP_Text NewText(string name, Transform parent, Style style, float size, TextAlignmentOptions alignment)
        {
            var text = NewUi(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
            if (style.Font != null)
                text.font = style.Font;
            text.fontSize = size;
            text.enableAutoSizing = true;
            text.fontSizeMin = size * 0.5f;
            text.fontSizeMax = size;
            text.alignment = alignment;
            text.color = Color.white;
            text.raycastTarget = false;
            return text;
        }

        private static Button NewButton(string name, Transform parent, Style style, string label, float size)
        {
            Button button;
            if (style.ButtonTemplate != null)
            {
                // clean copy of the upgrade button: same sprite, transition and click sound, no children
                var go = NewUi(name, parent).gameObject;
                var image = go.AddComponent<Image>();
                ApplySprite(image, style.ButtonSprite, new Color(0.3f, 0.6f, 1f));
                button = go.AddComponent<Button>();
                button.transition = style.ButtonTemplate.transition;
                button.colors = style.ButtonTemplate.colors;
                button.spriteState = style.ButtonTemplate.spriteState;

                var sound = style.ButtonTemplate.GetComponents<MonoBehaviour>().FirstOrDefault(m => m != null && m.GetType().Name == "UIButtonSound");
                if (sound != null && UnityEditorInternal.ComponentUtility.CopyComponent(sound))
                    UnityEditorInternal.ComponentUtility.PasteComponentAsNew(go);
            }
            else
            {
                var go = NewUi(name, parent).gameObject;
                go.AddComponent<Image>().color = new Color(0.3f, 0.6f, 1f);
                button = go.AddComponent<Button>();
            }

            var text = NewText("Label", button.transform, style, size, TextAlignmentOptions.Center);
            Stretch(text.rectTransform, new Vector2(0.05f, 0.1f), new Vector2(0.95f, 0.9f));
            text.text = label;
            return button;
        }

        private static void ApplySprite(Image image, Sprite sprite, Color fallbackColor)
        {
            if (sprite != null)
            {
                image.sprite = sprite;
                image.type = sprite.border.sqrMagnitude > 0f ? Image.Type.Sliced : Image.Type.Simple;
                image.color = Color.white;
            }
            else
            {
                image.color = fallbackColor;
            }
        }

        // ================================================================== misc helpers

        private static IEnumerable<T> FindInScene<T>(Scene scene) where T : Object
        {
            return Resources.FindObjectsOfTypeAll<T>().Where(o =>
            {
                var go = o is Component c ? c.gameObject : o as GameObject;
                return go != null && go.scene == scene && !EditorUtility.IsPersistent(go);
            });
        }

        private static void SetField(Object target, string field, object value)
        {
            var so = new SerializedObject(target);
            var prop = so.FindProperty(field);
            switch (value)
            {
                case string s: prop.stringValue = s; break;
                case int i: prop.intValue = i; break;
                case bool b: prop.boolValue = b; break;
                case Object o: prop.objectReferenceValue = o; break;
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(target);
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
                return;

            var parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(parent))
                EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }

        private static void Note(string line) => Log.AppendLine("• " + line);
    }
}
#endif
