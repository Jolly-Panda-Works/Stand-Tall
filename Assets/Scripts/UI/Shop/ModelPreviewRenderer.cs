using System.Collections.Generic;
using JollyPanda.LastFlag.Shop;
using UnityEngine;

namespace MJUtilities.UI
{
    /// <summary>
    /// Renders the 3D model of a shop item (character, gun, gun skin, bullet) into a RenderTexture once,
    /// so the shop / customize cards can show it with a RawImage. The model is copied far below the level
    /// on its own layer, all its scripts / sounds are stripped, it is framed by a private camera and removed
    /// again in the same frame, so gameplay never sees it. Textures are cached per item id.
    /// </summary>
    public class ModelPreviewRenderer : MonoBehaviour
    {
        private static ModelPreviewRenderer instance;

        public static ModelPreviewRenderer Instance
        {
            get
            {
                if (instance == null)
                    instance = FindAnyObjectByType<ModelPreviewRenderer>();
                if (instance == null)
                    instance = new GameObject(nameof(ModelPreviewRenderer)).AddComponent<ModelPreviewRenderer>();
                return instance;
            }
        }

        [Tooltip("Used for character models that have no Animator Controller (so they stand in the idle pose instead of a T-pose).")]
        [SerializeField] private RuntimeAnimatorController characterController;
        [SerializeField, Min(64)] private int resolution = 256;
        [SerializeField] private Color background = new(0f, 0f, 0f, 0f);
        [Tooltip("Direction from the model to the camera.")]
        [SerializeField] private Vector3 viewDirection = new(0.35f, 0.2f, 1f);
        [SerializeField, Range(1f, 2f)] private float padding = 1.1f;
        [SerializeField, Range(10f, 60f)] private float fieldOfView = 30f;

        private static readonly Vector3 StagePosition = new(0f, -5000f, 0f);

        private readonly Dictionary<string, RenderTexture> cache = new();
        private Camera previewCamera;
        private Transform stage;
        private int layer = -1;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }

            instance = this;
        }

        private void OnDestroy()
        {
            if (instance == this)
                instance = null;

            foreach (var texture in cache.Values)
            {
                if (texture != null)
                    texture.Release();
            }

            cache.Clear();
        }

        /// <summary>Preview texture of the item, or null when it has no preview model.</summary>
        public Texture GetPreview(ShopItemData item)
        {
            if (item == null || item.PreviewModel == null)
                return null;

            if (cache.TryGetValue(item.Id, out var cached) && cached != null && cached.IsCreated())
                return cached;

            var texture = Render(item);
            cache[item.Id] = texture;
            return texture;
        }

        // ------------------------------------------------------------------ rendering

        private RenderTexture Render(ShopItemData item)
        {
            EnsureRig();

            // inactive holder: no Awake/OnEnable of the copied scripts ever runs
            var holder = new GameObject("PreviewHolder");
            holder.SetActive(false);
            holder.transform.SetParent(stage, false);

            var model = Instantiate(item.PreviewModel, holder.transform);
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.Euler(item.PreviewRotation);
            model.SetActive(true); // some prefabs (e.g. pooled bullets) are saved inactive; the holder keeps it asleep
            Strip(model);
            SetLayer(model.transform, layer);

            if (item is GunSkinItemData skin && skin.Material != null)
                ApplyMaterial(model, skin.Material);

            var animator = model.GetComponentInChildren<Animator>(true);
            if (animator != null && animator.runtimeAnimatorController == null && item is CharacterItemData)
                animator.runtimeAnimatorController = characterController;

            holder.SetActive(true);

            if (animator != null && animator.runtimeAnimatorController != null)
            {
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                animator.Update(0f); // pose of the default (idle) state
            }

            foreach (var particles in model.GetComponentsInChildren<ParticleSystem>(true))
                particles.Simulate(0.5f, true, true);

            RenderTexture texture = null;
            if (TryGetBounds(model, out var bounds))
            {
                Frame(bounds);
                texture = new RenderTexture(resolution, resolution, 16, RenderTextureFormat.ARGB32)
                {
                    name = "Preview_" + item.Id,
                    antiAliasing = 2
                };
                texture.Create();

                previewCamera.targetTexture = texture;
                previewCamera.Render();
                previewCamera.targetTexture = null;
            }

            holder.SetActive(false);
            if (Application.isPlaying)
                Destroy(holder);
            else
                DestroyImmediate(holder); // edit mode (editor tools)
            return texture;
        }

        private void EnsureRig()
        {
            if (previewCamera != null)
                return;

            layer = FindFreeLayer();

            stage = new GameObject("PreviewStage").transform;
            stage.SetParent(transform, false);
            stage.position = StagePosition;

            var cameraObject = new GameObject("PreviewCamera");
            cameraObject.transform.SetParent(stage, false);
            previewCamera = cameraObject.AddComponent<Camera>();
            previewCamera.enabled = false; // only renders when asked
            previewCamera.clearFlags = CameraClearFlags.SolidColor;
            previewCamera.backgroundColor = background;
            previewCamera.cullingMask = 1 << layer;
            previewCamera.fieldOfView = fieldOfView;
            previewCamera.allowHDR = false;
            previewCamera.allowMSAA = true;

            var lightObject = new GameObject("PreviewLight");
            lightObject.transform.SetParent(cameraObject.transform, false);
            lightObject.transform.localRotation = Quaternion.Euler(35f, -30f, 0f);
            var previewLight = lightObject.AddComponent<Light>();
            previewLight.type = LightType.Directional;
            previewLight.intensity = 1.1f;
            previewLight.cullingMask = 1 << layer;
            previewLight.shadows = LightShadows.None;
        }

        private void Frame(Bounds bounds)
        {
            float radius = Mathf.Max(bounds.extents.magnitude, 0.01f) * padding;
            float distance = radius / Mathf.Sin(fieldOfView * 0.5f * Mathf.Deg2Rad);
            var direction = viewDirection.sqrMagnitude > 0f ? viewDirection.normalized : Vector3.forward;

            var cameraTransform = previewCamera.transform;
            cameraTransform.position = bounds.center + direction * distance;
            cameraTransform.LookAt(bounds.center);
            previewCamera.nearClipPlane = Mathf.Max(0.01f, distance - radius * 2f);
            previewCamera.farClipPlane = distance + radius * 2f;
        }

        private static bool TryGetBounds(GameObject model, out Bounds bounds)
        {
            bounds = default;
            bool found = false;
            foreach (var renderer in model.GetComponentsInChildren<Renderer>())
            {
                if (renderer is ParticleSystemRenderer || !renderer.enabled)
                    continue;

                // skinned meshes report bind-pose bounds until they are drawn: measure the posed mesh instead
                var rendererBounds = renderer is SkinnedMeshRenderer skinned ? PosedBounds(skinned) : renderer.bounds;
                if (found)
                    bounds.Encapsulate(rendererBounds);
                else
                    bounds = rendererBounds;
                found = true;
            }

            // particle-only models (some bullets)
            if (!found)
            {
                foreach (var renderer in model.GetComponentsInChildren<ParticleSystemRenderer>())
                {
                    if (found)
                        bounds.Encapsulate(renderer.bounds);
                    else
                        bounds = renderer.bounds;
                    found = true;
                }
            }

            return found && bounds.size.sqrMagnitude > 0f;
        }

        private static Bounds PosedBounds(SkinnedMeshRenderer skinned)
        {
            if (skinned.sharedMesh == null)
                return skinned.bounds;

            var mesh = new Mesh();
            skinned.BakeMesh(mesh, true); // vertices already scaled -> transform without scale
            var local = mesh.bounds;
            DestroyImmediate(mesh);

            var t = skinned.transform;
            var matrix = Matrix4x4.TRS(t.position, t.rotation, Vector3.one);
            var result = new Bounds(matrix.MultiplyPoint3x4(local.center), Vector3.zero);
            for (int i = 0; i < 8; i++)
            {
                var corner = local.center + Vector3.Scale(local.extents,
                    new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                result.Encapsulate(matrix.MultiplyPoint3x4(corner));
            }

            return result;
        }

        /// <summary>Removes everything that could react to the game (scripts, sounds, physics).</summary>
        private static void Strip(GameObject model)
        {
            foreach (var source in model.GetComponentsInChildren<AudioSource>(true))
                DestroyImmediate(source);

            // reverse order: a script that [RequireComponent]s another one was added after it
            var behaviours = model.GetComponentsInChildren<MonoBehaviour>(true);
            for (int i = behaviours.Length - 1; i >= 0; i--)
            {
                if (behaviours[i] != null)
                    DestroyImmediate(behaviours[i]);
            }

            foreach (var body in model.GetComponentsInChildren<Rigidbody>(true))
                DestroyImmediate(body);

            foreach (var collider in model.GetComponentsInChildren<Collider>(true))
                DestroyImmediate(collider);
        }

        private static void ApplyMaterial(GameObject model, Material material)
        {
            foreach (var renderer in model.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer is ParticleSystemRenderer || renderer is LineRenderer || renderer is TrailRenderer)
                    continue;

                var materials = renderer.sharedMaterials;
                if (materials.Length == 0)
                    continue;

                materials[0] = material; // same slot as GunSkinTarget's default
                renderer.sharedMaterials = materials;
            }
        }

        private static void SetLayer(Transform root, int targetLayer)
        {
            root.gameObject.layer = targetLayer;
            foreach (Transform child in root)
                SetLayer(child, targetLayer);
        }

        private static int FindFreeLayer()
        {
            for (int i = 31; i >= 8; i--)
            {
                if (string.IsNullOrEmpty(LayerMask.LayerToName(i)))
                    return i;
            }

            return 31;
        }
    }
}
