using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.Handlers;
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
        
        [Header("CountDown")]
        [SerializeField] private TMP_Text countDownText;
        [SerializeField] private float startTime = 10f;
        private float currentTime;
        private bool isCountingDown;

        public override void OnAwake()
        {
            quitButton.onClick.AddListener(QuitButtonClicked);

            startWaveButton.onClick.AddListener(StartWaveButtonClicked);

            Informant.OnEarnCoin += DisplayEarnCoin;
        }

        private void OnDestroy()
        {
            Informant.OnEarnCoin -= DisplayEarnCoin;
            quitButton.onClick.RemoveListener(QuitButtonClicked);

            startWaveButton.onClick.RemoveListener(StartWaveButtonClicked);

        }

        private void DisplayEarnCoin(int value)
        {
            coinText.SetText(value.ToString("N0", CultureInfo.InvariantCulture));
        }

        public override void OnSetValues()
        {
            

            StartCountdown();
        }
        
        private void QuitButtonClicked()
        {
            GameManager.instance.BackHome();
        }

        private void StartWaveButtonClicked()
        {
            UIManager.instance.ClosePopup(PopupType.Countdown);
            StartNextRound();
        }
        
        
        public void StartCountdown()
        {
            currentTime = startTime;
            StartCoroutine(CountdownCoroutine());
        }

        private IEnumerator CountdownCoroutine()
        {
            isCountingDown = true;

            while (currentTime > 0)
            {
                countDownText.text = Mathf.CeilToInt(currentTime).ToString() + "s";
                yield return new WaitForSecondsRealtime(1f);
                currentTime -= 1f;
            }

            countDownText.text = "0";
            isCountingDown = false;

            StartNextRound();
        }

        private void StartNextRound()
        {
            Debug.Log("Next round started!");
            CloseThisPopup();
            //ToDo: start wave
            GameManager.instance.UnPauseGame();
        }
    }
}