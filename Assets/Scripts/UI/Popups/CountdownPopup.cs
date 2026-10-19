using JollyPanda.LastFlag.Ads;
using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.Handlers;
using JollyPanda.LastFlag.Localization;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MJUtilities.UI
{
    public class CountdownPopup : UIPopup
    {
        [SerializeField] private TMP_Text coinText;

        [Header("Buttons")]
        [SerializeField] private Button quitButton;
        [SerializeField] private Button startWaveButton;
        
        [Header("Rewarded Ad (optional)")]
        [Tooltip("\"Watch a video: double the coins of this wave\". Hidden when no ad is ready.")]
        [SerializeField] private Button doubleRewardButton;

        [Header("CountDown")]
        [SerializeField] private TMP_Text countDownText;
        [SerializeField] private float startTime = 10f;
        private float currentTime;
        private bool isCountingDown;
        private Coroutine countdownRoutine;
        private int lastEarned;
        private bool rewardDoubled;
        private bool adShowing;

        public override void OnAwake()
        {
            quitButton.onClick.AddListener(QuitButtonClicked);

            startWaveButton.onClick.AddListener(StartWaveButtonClicked);

            if (doubleRewardButton != null)
                doubleRewardButton.onClick.AddListener(DoubleRewardClicked);

            Informant.OnEarnCoin += DisplayEarnCoin;
        }

        private void OnDestroy()
        {
            Informant.OnEarnCoin -= DisplayEarnCoin;
            quitButton.onClick.RemoveListener(QuitButtonClicked);

            startWaveButton.onClick.RemoveListener(StartWaveButtonClicked);

            if (doubleRewardButton != null)
                doubleRewardButton.onClick.RemoveListener(DoubleRewardClicked);
        }

        private void DisplayEarnCoin(int value)
        {
            lastEarned = value;
            rewardDoubled = false;
            coinText.SetText(value.ToString("N0", CultureInfo.InvariantCulture));
        }

        public override void OnSetValues()
        {
            adShowing = false;
            RefreshDoubleRewardButton();
            StartCountdown();
        }

        private void RefreshDoubleRewardButton()
        {
            if (doubleRewardButton == null)
                return;

            var service = RewardedAdServiceBase.Instance;
            bool available = service != null && service.IsReady && lastEarned > 0 && !rewardDoubled && !adShowing;
            doubleRewardButton.gameObject.SetActive(available);
        }

        private void DoubleRewardClicked()
        {
            var service = RewardedAdServiceBase.Instance;
            if (service == null || !service.IsReady || rewardDoubled || adShowing || lastEarned <= 0)
                return;

            adShowing = true; // the countdown waits while the ad is on screen
            RefreshDoubleRewardButton();

            service.Show(earned =>
            {
                adShowing = false;

                if (earned && !rewardDoubled)
                {
                    rewardDoubled = true;
                    SaveSystem.AddMoney(lastEarned);
                    coinText.SetText((lastEarned * 2).ToString("N0", CultureInfo.InvariantCulture));
                }

                RefreshDoubleRewardButton();
            });
        }
        
        private void QuitButtonClicked()
        {
            GameManager.instance.BackHome();
        }

        private void StartWaveButtonClicked()
        {
            StartNextRound();
        }
        
        
        public void StartCountdown()
        {
            currentTime = startTime;

            if (countdownRoutine != null)
                StopCoroutine(countdownRoutine);
            countdownRoutine = StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            isCountingDown = true;

            while (currentTime > 0)
            {
                while (adShowing)
                    yield return null; // do not run the timer out while a rewarded ad is showing

                GameLocale.SetText(countDownText, GameLocale.Format(GameText.SecondsSuffix, Mathf.CeilToInt(currentTime)));
                yield return new WaitForSecondsRealtime(1f);
                currentTime -= 1f;
            }

            while (adShowing)
                yield return null; // an ad was started during the last second

            countDownText.text = "0";
            isCountingDown = false;
            countdownRoutine = null;

            StartNextRound();
        }

        private void StartNextRound()
        {
            if (countdownRoutine != null)
            {
                StopCoroutine(countdownRoutine);
                countdownRoutine = null;
            }
            isCountingDown = false;

            CloseThisPopup();
            GameManager.instance.UnPauseGame();
            Informant.NotifyNextWave(); // the spawner starts the next wave right here
        }
    }
}