using System.Collections;
using System.Collections.Generic;
using JollyPanda.LastFlag.Database;
using JollyPanda.LastFlag.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeProgressConfig : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradeCostText;

    [SerializeField] private List<Image> activeProgressImages = new List<Image>();


    public void HandleUpgrade(UpgradeData upgradeData)
    {
        ActivateProgressImages(upgradeData.Level);
        SetUpgradeCostText(upgradeData.CanUpgrade ? upgradeData.NextLevelCost.ToString() : GameLocale.Get(GameText.MaxLevel));
        
        //Debug.Log("level: " + upgradeData.Level + " cost: " + upgradeData.NextLevelCost + " value: " + upgradeData.CurrentValue, this);
    }

    private void ActivateProgressImages(int index)
    {
        // turn ON the first `level` images and OFF the rest (e.g. after Reset Save Data)
        for (int i = 0; i < activeProgressImages.Count; i++)
        {
            if (activeProgressImages[i] != null)
                activeProgressImages[i].gameObject.SetActive(i < index);
        }
    }

    private void SetUpgradeCostText(string costText)
    {
        GameLocale.SetText(upgradeCostText, costText);
    }

    [ContextMenu(nameof(ResetAllImages))]
    private void ResetAllImages()
    {
        foreach (var image in activeProgressImages)
        {
            image.gameObject.SetActive(false);
        }
    }

    [ContextMenu(nameof(SetReferences))]
    private void SetReferences()
    {
        activeProgressImages.Clear();

        var tmpImages = GetComponentsInChildren<Image>(true);
        foreach (var image in tmpImages)
        {
            if (image.name == "PageNavi_On")
                activeProgressImages.Add(image);
        }
    }
}