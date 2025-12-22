using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using MJUtilities.UI;
using TMPro;

public class WinDialog : UIPopup
{
    public static WinDialog instance;
    
    [SerializeField] private Button restartButton;
    
    public override void OnAwake()
    {
        
    }

    public override void OnSetValues()
    {
        
    }
}