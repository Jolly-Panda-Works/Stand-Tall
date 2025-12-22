using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.Handlers;
using MJUtilities.UI;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LosePopup : UIPopup
{
    public static LosePopup instance;

    [SerializeField] private TMP_Text coinText;

    [Header("Buttons")]
    [SerializeField] private Button restartButton;
    [SerializeField] private Button homeButton;

    public override void OnAwake()
    {
        Informant.OnEarnCoin += DisplayEarnCoin;
        restartButton.onClick.AddListener(RestartButtonClicked);

        homeButton.onClick.AddListener(HomeButtonClicked);
    }

    private void OnDestroy()
    {
        Informant.OnEarnCoin -= DisplayEarnCoin;
        restartButton.onClick.RemoveListener(RestartButtonClicked);

        homeButton.onClick.RemoveListener(HomeButtonClicked);
    }

    private void DisplayEarnCoin(int value)
    {
        coinText.SetText(value.ToString("N0", CultureInfo.InvariantCulture));
    }

    public override void OnSetValues()
    {
    }


    private void RestartButtonClicked()
    {
        GameManager.instance.RestartCurrentWave();
    }

    private void HomeButtonClicked()
    {
        GameManager.instance.BackHome();
    }
}