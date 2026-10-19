using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    /// <summary>
    /// Put it on a Gun. It swaps one material slot of the gun's renderers when a skin is equipped
    /// and restores the original look when no skin material is given.
    /// </summary>
    public class GunSkinTarget : MonoBehaviour
    {
        [SerializeField] private Renderer[] renderers;
        [Tooltip("Which material slot of each renderer is replaced by the skin.")]
        [SerializeField, Min(0)] private int materialSlot = 0;

        private Material[][] originals;

        private void EnsureCached()
        {
            if (originals != null)
                return;

            if (renderers == null || renderers.Length == 0)
                renderers = GetComponentsInChildren<Renderer>(true);

            originals = new Material[renderers.Length][];
            for (int i = 0; i < renderers.Length; i++)
                originals[i] = renderers[i] != null ? renderers[i].sharedMaterials : null;
        }

        /// <param name="skinMaterial">null = restore the original material.</param>
        public void Apply(Material skinMaterial)
        {
            EnsureCached();

            for (int i = 0; i < renderers.Length; i++)
            {
                var targetRenderer = renderers[i];
                var original = originals[i];
                if (targetRenderer == null || original == null || materialSlot >= original.Length)
                    continue;

                var materials = (Material[])original.Clone();
                if (skinMaterial != null)
                    materials[materialSlot] = skinMaterial;

                targetRenderer.sharedMaterials = materials;
            }
        }
    }
}
