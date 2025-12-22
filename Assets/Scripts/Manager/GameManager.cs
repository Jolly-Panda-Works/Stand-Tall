using JollyPanda.LastFlag.Handlers;
using MJUtilities.UI;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private void Awake()
    {
        if (!instance)
            instance = this;
    }
    
    
    public void LoadLevel(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
    
    public void BackHome()
    {
        UIManager.instance.CloseEverything();
        UIManager.instance.OpenPage(PageType.Home);
        //SceneManager.LoadScene(0);
    }

    public void RestartCurrentWave()
    {
        UIManager.instance.CloseEverything();
        UIManager.instance.OpenPage(PageType.HUD);
        Informant.NotifyStart();
    }

    public void PauseGame()
    {
        //Time.timeScale = 0f;
        Informant.PauseGame(true);
    }
    public void UnPauseGame()
    {
        //Time.timeScale = 1f;
        Informant.PauseGame(false);
    }
}