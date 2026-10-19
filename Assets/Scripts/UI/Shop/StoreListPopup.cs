using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.Handlers;
using JollyPanda.LastFlag.Shop;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MJUtilities.UI
{
    /// <summary>
    /// Shared behaviour of the Shop and Customize popups: category tabs, a scrolling list of
    /// <see cref="ShopItemView"/> cards, the money label and a short feedback message.
    /// Same popup pattern as UpgradePopup (opened by UIManager.OpenPopup).
    /// </summary>
    public abstract class StoreListPopup : UIPopup
    {
        [Serializable]
        public class TabEntry
        {
            public ShopCategory category;
            public Button button;
            public TMP_Text label;
            [Tooltip("Optional object shown only on the selected tab.")]
            public GameObject selectedMarker;
        }

        [Header("Money")]
        [SerializeField] private TMP_Text moneyText;

        [Header("Tabs (order = display order, first one opens by default)")]
        [SerializeField] private List<TabEntry> tabs = new();

        [Header("List")]
        [SerializeField] private ShopItemView itemPrefab;
        [SerializeField] private RectTransform content;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private GameObject emptyLabelRoot;
        [SerializeField] private TMP_Text emptyLabel;

        [Header("Feedback (optional)")]
        [SerializeField] private TMP_Text feedbackText;
        [SerializeField] private float feedbackSeconds = 2f;

        private readonly List<ShopItemView> views = new();
        private ShopCategory currentCategory;
        private bool hasCategory;
        private Coroutine feedbackRoutine;

        protected abstract ItemViewMode Mode { get; }

        /// <summary>Items that should be listed in a tab.</summary>
        protected abstract IEnumerable<ShopItemData> GetItems(ShopCategory category);

        /// <summary>The player tapped the card button.</summary>
        protected abstract void OnItemClicked(ShopItemData item);

        // ------------------------------------------------------------------ lifecycle

        public override void OnAwake()
        {
            // listeners are added ONCE here (OnSetValues runs on every OpenPopup)
            foreach (var tab in tabs)
            {
                if (tab.button == null)
                    continue;

                var category = tab.category;
                tab.button.onClick.AddListener(() => SelectTab(category));
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Informant.OnProfileChange += OnProfileChange;
            ShopLocale.LanguageChanged += RefreshAll;
        }

        private void OnDisable()
        {
            Informant.OnProfileChange -= OnProfileChange;
            ShopLocale.LanguageChanged -= RefreshAll;
        }

        public override void OnSetValues()
        {
            BackButtonConfig();

            if (feedbackText != null)
                feedbackText.gameObject.SetActive(false);

            if (tabs.Count > 0)
                SelectTab(hasCategory ? currentCategory : tabs[0].category);
        }

        // ------------------------------------------------------------------ tabs & list

        public void SelectTab(ShopCategory category)
        {
            currentCategory = category;
            hasCategory = true;

            foreach (var tab in tabs)
            {
                if (tab.selectedMarker != null)
                    tab.selectedMarker.SetActive(tab.category == category);
            }

            RebuildList();
            RefreshTabLabels();
            RefreshMoney();
        }

        private void RebuildList()
        {
            var shop = ShopManager.Instance;
            if (shop == null || itemPrefab == null || content == null)
                return;

            int index = 0;
            foreach (var item in GetItems(currentCategory))
            {
                if (index >= views.Count)
                    views.Add(Instantiate(itemPrefab, content));

                var view = views[index++];
                view.gameObject.SetActive(true);
                view.Bind(item, Mode, HandleItemClicked);
            }

            // hide the cards that are not needed (the pool is reused, never destroyed)
            for (int i = index; i < views.Count; i++)
                views[i].gameObject.SetActive(false);

            bool isEmpty = index == 0;
            if (emptyLabelRoot != null)
                emptyLabelRoot.SetActive(isEmpty);
            if (emptyLabel != null)
                ShopLocale.SetText(emptyLabel, ShopLocale.Get(ShopText.EmptyList));

            if (scrollRect != null)
                scrollRect.normalizedPosition = new Vector2(0f, 1f);
        }

        private void RefreshTabLabels()
        {
            foreach (var tab in tabs)
                ShopLocale.SetText(tab.label, ShopLocale.GetCategoryName(tab.category));
        }

        private void RefreshMoney()
        {
            if (moneyText != null)
                moneyText.text = SaveSystem.GetMoney().ToString("N0", CultureInfo.InvariantCulture);
        }

        private void RefreshAll()
        {
            RefreshTabLabels();
            RefreshMoney();
            foreach (var view in views)
            {
                if (view.gameObject.activeSelf)
                    view.Refresh();
            }
        }

        private void OnProfileChange(PlayerSaveData data)
        {
            if (!isActiveAndEnabled)
                return;

            RefreshAll();
        }

        private void HandleItemClicked(ShopItemData item)
        {
            OnItemClicked(item);
        }

        // ------------------------------------------------------------------ helpers for subclasses

        protected void RefreshCards()
        {
            foreach (var view in views)
            {
                if (view.gameObject.activeSelf)
                    view.Refresh();
            }
        }

        protected void ShowFeedback(ShopText message)
        {
            if (feedbackText == null)
                return;

            ShopLocale.SetText(feedbackText, ShopLocale.Get(message));
            feedbackText.gameObject.SetActive(true);

            if (feedbackRoutine != null)
                StopCoroutine(feedbackRoutine);
            feedbackRoutine = StartCoroutine(HideFeedbackAfterDelay());
        }

        private IEnumerator HideFeedbackAfterDelay()
        {
            yield return new WaitForSecondsRealtime(feedbackSeconds);
            feedbackText.gameObject.SetActive(false);
            feedbackRoutine = null;
        }
    }
}
