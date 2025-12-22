using System;
using UnityEngine;
using UnityEngine.UI;
using MJUtilities.UI;

public class PausePopup : UIPopup
{
    [Header("Buttons")]
    [SerializeField] private Button quitButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    public override void OnSetValues()
    {
        

    }
    
    public override void OnAwake()
    {
        quitButton.onClick.AddListener(QuitButtonClicked);

        resumeButton.onClick.AddListener(ResumeButtonClicked);

        restartButton.onClick.AddListener(RestartButtonClicked);
    }

    private void OnDestroy()
    {
        quitButton.onClick.RemoveListener(QuitButtonClicked);

        resumeButton.onClick.RemoveListener(ResumeButtonClicked);

        restartButton.onClick.RemoveListener(RestartButtonClicked);

    }

    private void QuitButtonClicked()
    {
        GameManager.instance.UnPauseGame();
        GameManager.instance.BackHome();
        UIManager.instance.OpenPage(PageType.Home);
    }
    private void ResumeButtonClicked()
    {
        GameManager.instance.UnPauseGame();
        UIManager.instance.ClosePopup(PopupType.Pause);
    }
    private void RestartButtonClicked()
    {
        //GameManager.instance.UnPauseGame();
        //GameManager.instance.BackHome();
        //UIManager.instance.OpenPage(PageType.Home);
    }
}