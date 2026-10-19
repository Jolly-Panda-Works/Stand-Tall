using JollyPanda.LastFlag.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JollyPanda.LastFlag.Ads
{
    /// <summary>
    /// "Watch a video, get free coins" button. Put it anywhere (Home, Shop popup ...) and assign the Button.
    /// The button is only clickable while an ad is ready and the cooldown is over.
    /// </summary>
    public class RewardedCoinsButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField, Min(1)] private int rewardCoins = 50;
        [Tooltip("Seconds before the button can be used again (resets when the app restarts).")]
        [SerializeField, Min(0f)] private float cooldownSeconds = 60f;
        [Tooltip("Optional. Shows \"+50\".")]
        [SerializeField] private TMP_Text rewardLabel;

        private float nextAllowedTime;
        private bool showing;
        private float nextRefresh;

        private void Awake()
        {
            if (button != null)
                button.onClick.AddListener(OnClicked);

            if (rewardLabel != null)
                rewardLabel.text = "+" + rewardCoins;
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveListener(OnClicked);
        }

        private void OnEnable() => Refresh();

        private void Update()
        {
            // an ad loads in the background, so poll twice a second
            if (Time.unscaledTime < nextRefresh)
                return;

            nextRefresh = Time.unscaledTime + 0.5f;
            Refresh();
        }

        private void Refresh()
        {
            if (button == null)
                return;

            var service = RewardedAdServiceBase.Instance;
            button.interactable = service != null && service.IsReady && !showing && Time.unscaledTime >= nextAllowedTime;
        }

        private void OnClicked()
        {
            var service = RewardedAdServiceBase.Instance;
            if (service == null || !service.IsReady || showing)
                return;

            showing = true;
            Refresh();

            service.Show(earned =>
            {
                showing = false;

                if (earned)
                {
                    SaveSystem.AddMoney(rewardCoins);
                    nextAllowedTime = Time.unscaledTime + cooldownSeconds;
                }

                Refresh();
            });
        }
    }
}
