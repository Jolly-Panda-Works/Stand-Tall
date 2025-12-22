using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class SplashScreenItem
{
    public CanvasGroup canvasGroup;
    public float holdDuration = 2f;
}

public class SplashScreenController : MonoBehaviour
{
    [SerializeField] private List<SplashScreenItem> splashScreens;
    [SerializeField] private float fadeInDuration = 1f;
    [SerializeField] private float fadeOutDuration = 1f;
    [SerializeField] private string sceneToLoad = "MainScene";

    private void Start()
    {
        foreach (var item in splashScreens)
        {
            if (item.canvasGroup != null)
            {
                item.canvasGroup.gameObject.SetActive(false);
                item.canvasGroup.alpha = 0f;
            }
        }

        StartCoroutine(PlaySplashSequence());
    }

    private IEnumerator PlaySplashSequence()
    {
        foreach (var item in splashScreens)
        {
            if (item.canvasGroup == null)
                continue;

            item.canvasGroup.alpha = 0f;
            item.canvasGroup.gameObject.SetActive(true);

            // Fade In
            float timer = 0f;
            while (timer < fadeInDuration)
            {
                item.canvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            item.canvasGroup.alpha = 1f;

            // Hold
            yield return new WaitForSecondsRealtime(item.holdDuration);

            // Fade Out
            timer = 0f;
            while (timer < fadeOutDuration)
            {
                item.canvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeOutDuration);
                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            item.canvasGroup.alpha = 0f;
            item.canvasGroup.gameObject.SetActive(false);
        }

        // Load next scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
