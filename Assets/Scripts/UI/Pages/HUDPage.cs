using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using JollyPanda.LastFlag.EnemyModule;
using JollyPanda.LastFlag.Handlers;
using UnityEngine;
using UnityEngine.UI;
using MJUtilities.UI;
using TMPro;

public class HUDPage : UIPage
{
    [Header("Buttons")]
    [SerializeField] private Button pauseButton;
    
    [Header("Texts")]
    [SerializeField] private TMP_Text enemyStatusText;
    [SerializeField] private TMP_Text startWaveText;
    [SerializeField] private TMP_Text waveIndexText;
    
    [Header("CanvasGroups")]
    [SerializeField] private CanvasGroup newWaveNotificationCanvasGroup;

    private void OnEnable()
    {
        Informant.OnEnemyKilled += EnemyKilled;
        Informant.OnWaveEnd += WaveEnd;
        Informant.OnWaveStart += WaveStart;
        Informant.OnEnemyReachedTop += EnemyReachedTop;
    }
    private void OnDisable()
    {
        Informant.OnEnemyKilled -= EnemyKilled;
        Informant.OnWaveEnd -= WaveEnd;
        Informant.OnWaveStart -= WaveStart;
        Informant.OnEnemyReachedTop -= EnemyReachedTop;
    }

    public override void OnSetValues()
    {
        pauseButton.onClick.RemoveAllListeners();
        pauseButton.onClick.AddListener(PauseButtonClicked);        
    }

    private void Start()
    {
        
    }

    public override void OnAwake()
    {
        newWaveNotificationCanvasGroup.alpha = 0;
    }

    private void PauseButtonClicked()
    {
        UIManager.instance.OpenPopup(PopupType.Pause);
        GameManager.instance.PauseGame();
    }
    
    private void EnemyKilled(int alivedEnemyCount, int totalEnemy)
    {
        enemyStatusText.text = alivedEnemyCount + "/" + totalEnemy;
    }
    private void WaveEnd(int waveIndex, int killedEnemy)
    {
        UIManager.instance.OpenPopup(PopupType.Countdown);
        //GameManager.instance.PauseGame();
    }
    private void WaveStart(int waveIndex)
    {
        GameManager.instance.UnPauseGame();
        
        var currentWave = waveIndex + 1;
        waveIndexText.text = currentWave.ToString();
        StartCoroutine(WaveNotificationCoroutine(currentWave));
    }
    
    private void EnemyReachedTop(Enemy enemy)
    {
        UIManager.instance.OpenPopup(PopupType.Lose);
    }

    private IEnumerator WaveNotificationCoroutine(int waveIndex)
    {
        newWaveNotificationCanvasGroup.alpha = 1f;
        startWaveText.text = $"Enemies rise!\n Wave {waveIndex} has begun!";

        float duration = 5f;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            newWaveNotificationCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        newWaveNotificationCanvasGroup.alpha = 0f;
    }
}