using System.Collections;
using System.Collections.Generic;
using JollyPanda.LastFlag.Database;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeProgressConfig : MonoBehaviour
{
    [SerializeField] private TMP_Text upgradeCostText;

    [SerializeField] private List<Image> activeProgressImages = new List<Image>();


    public void HandleUpgrade(UpgradeData upgradeData)
    {
        SetUpgradeCostText(upgradeData.NextLevelCost.ToString());
        ActivateProgressImages(upgradeData.Level);
        
        if(upgradeData.Level == 4)
            SetUpgradeCostText("MAX");
        
        //Debug.Log("level: " + upgradeData.Level + " cost: " + upgradeData.NextLevelCost + " value: " + upgradeData.CurrentValue, this);
    }

    private void ActivateProgressImages(int index)
    {
        for (int i = 0; i < index; i++)
        {
            activeProgressImages[i].gameObject.SetActive(true);
        }
    }

    private void SetUpgradeCostText(string costText)
    {
        upgradeCostText.text = costText;
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