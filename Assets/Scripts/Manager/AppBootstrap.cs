using UnityEngine;

/// <summary>
/// Runs once before the first scene loads (no GameObject needed). Mobile defaults:
/// 60 FPS (Android often defaults to 30) and a screen that stays on during play.
/// </summary>
public static class AppBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        QualitySettings.vSyncCount = 0; // otherwise targetFrameRate is ignored where vSync is on
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
