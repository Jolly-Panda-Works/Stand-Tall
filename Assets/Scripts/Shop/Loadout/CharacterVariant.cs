using UnityEngine;

namespace JollyPanda.LastFlag.PlayerModule
{
    /// <summary>
    /// One selectable character model. Put one on each character model that lives under the Player in the scene
    /// (all models must use the same Animator Controller / parameters as the current character).
    /// </summary>
    public class CharacterVariant : MonoBehaviour
    {
        [Tooltip("Must match CharacterItemData.VariantId.")]
        [SerializeField] private string variantId;
        [SerializeField] private Animator animator;
        [Tooltip("Optional. Where the guns are attached on this model (e.g. an empty object on the hand bone). " +
                 "Leave empty if the guns are not parented to the character.")]
        [SerializeField] private Transform gunMount;

        public string VariantId => variantId;
        public Animator Animator => animator;
        public Transform GunMount => gunMount;

        private void Reset()
        {
            animator = GetComponentInChildren<Animator>(true);
        }
    }
}
