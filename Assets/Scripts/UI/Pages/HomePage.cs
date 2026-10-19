using System;
using System.Globalization;
using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.Handlers;
using UnityEngine;
using UnityEngine.UI;
using MJUtilities.UI;
using MJUtilities;
using TMPro;

public class HomePage : UIPage
{
    [Header("Buttons")] 
    [SerializeField] private Button settingButton;
    [SerializeField] private Button upgradeButton;
    [Tooltip("Optional. Opens the shop popup.")]
    [SerializeField] private Button shopButton;
    [Tooltip("Optional. Opens the customize popup.")]
    [SerializeField] private Button customizeButton;

    [Header("Dragging")] 
    //private bool isGameStarted;
    private Vector2 startTouchPosition;
    private bool isDragging;
    public float dragThreshold;

    [Header("Money")] 
    [SerializeField] private TMP_Text moneyText;

    private void OnEnable()
    {
        Informant.OnProfileChange += ProfileChange;
        
        long currentMoney = SaveSystem.GetMoney();
        moneyText.text = FormatMoney(currentMoney);
    }

    private void OnDisable()
    {
        Informant.OnProfileChange -= ProfileChange;
    }

    public override void OnSetValues()
    {
        // Listeners are added once in OnAwake. RemoveAllListeners here would also remove
        // the click sound that UIButtonSound registered on the same buttons.
    }

    public override void OnAwake()
    {
        settingButton.onClick.AddListener(SettingButtonClicked);
        upgradeButton.onClick.AddListener(UpgradeButtonClicked);

        if (shopButton != null)
            shopButton.onClick.AddListener(ShopButtonClicked);

        if (customizeButton != null)
            customizeButton.onClick.AddListener(CustomizeButtonClicked);
    }

    private void Awake() => Input.simulateMouseWithTouches = true;

    private void Update()
    {
        //if (isGameStarted) return;
        if (UIManager.instance.HasActivePopup())
        {
            isDragging = false;
            return;
        }

        Dragging();
    }

    private void Dragging()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 endPosition = Input.mousePosition;
            float distance = Vector2.Distance(startTouchPosition, endPosition);

            if (distance > dragThreshold)
            {
                //Debug.Log("Dragging");
                StartGame();
            }

            isDragging = false;
        }
    }

    private void StartGame()
    {
        //isGameStarted = true;
        UIManager.instance.OpenPage(PageType.HUD, true);
        Informant.NotifyStart();
    }

    private void ProfileChange(PlayerSaveData playerSaveData)
    {
        long currentMoney = playerSaveData.Money;
        moneyText.text = FormatMoney(currentMoney);
    }

    private string FormatMoney(long money)
    {
        return money.ToString("N0", CultureInfo.InvariantCulture);
    }


    private void UpgradeButtonClicked()
    {
        UIManager.instance.OpenPopup(PopupType.Upgrade);
    }

    private void ShopButtonClicked()
    {
        UIManager.instance.OpenPopup(PopupType.Shop);
    }

    private void CustomizeButtonClicked()
    {
        UIManager.instance.OpenPopup(PopupType.Customize);
    }

    private void SettingButtonClicked()
    {
        UIManager.instance.OpenPopup(PopupType.Setting);
    }
}