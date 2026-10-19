using JollyPanda.LastFlag.Shop;
using TMPro;
using UnityEngine;

namespace MJUtilities.UI
{
    /// <summary>
    /// Static label (popup title, home button caption ...) that shows one <see cref="ShopText"/>
    /// in the current language and follows language changes.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class ShopLocaleLabel : MonoBehaviour
    {
        [SerializeField] private ShopText text = ShopText.Shop;

        private TMP_Text target;

        public void SetKey(ShopText key)
        {
            text = key;
            Refresh();
        }

        private void OnEnable()
        {
            ShopLocale.LanguageChanged += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            ShopLocale.LanguageChanged -= Refresh;
        }

        private void Refresh()
        {
            if (target == null)
                target = GetComponent<TMP_Text>();

            ShopLocale.SetText(target, ShopLocale.Get(text));
        }
    }
}
