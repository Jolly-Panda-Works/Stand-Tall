using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    /// <summary>
    /// Shows exactly one <see cref="CharacterVariant"/>, hands its Animator to the existing
    /// PlayerAnimationController and moves the guns onto the new model's mount.
    /// </summary>
    public class CharacterSelector : MonoBehaviour
    {
        [SerializeField] private CharacterVariant[] variants;
        [SerializeField] private PlayerAnimationController animationController;
        [SerializeField] private GunSelector gunSelector;

        public string CurrentVariantId { get; private set; }

        public bool Select(string variantId)
        {
            CharacterVariant selected = null;
            foreach (var variant in variants)
            {
                if (variant != null && variant.VariantId == variantId)
                {
                    selected = variant;
                    break;
                }
            }

            if (selected == null)
            {
                Debug.LogWarning($"[CharacterSelector] No CharacterVariant with id '{variantId}'.", this);
                return false;
            }

            // 1) show the new model first
            selected.gameObject.SetActive(true);

            // 2) move the guns BEFORE the old model is hidden (a gun under an inactive parent would stop updating)
            if (selected.GunMount != null && gunSelector != null)
            {
                foreach (var gun in gunSelector.Guns)
                {
                    if (gun != null)
                        gun.transform.SetParent(selected.GunMount, false);
                }
            }

            // 3) hide the others
            foreach (var variant in variants)
            {
                if (variant != null && variant != selected)
                    variant.gameObject.SetActive(false);
            }

            // 4) reuse the existing animation controller with the new Animator
            if (animationController != null)
                animationController.SetAnimator(selected.Animator);

            CurrentVariantId = variantId;
            return true;
        }
    }
}
