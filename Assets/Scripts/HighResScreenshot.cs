using UnityEngine;
using System.IO;

public class HighResScreenshot : MonoBehaviour
{
    public Camera cam;
    public int width = 3840;
    public int height = 2160;

    // Debug tool: active only in the Editor and Development Builds. The component can stay on a scene object,
    // in a release build it simply does nothing (no Missing Script).
    public void Capture()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        RenderTexture rt = new RenderTexture(width, height, 24);
        cam.targetTexture = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);

        cam.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        cam.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);

        byte[] bytes = screenShot.EncodeToPNG();
        string filename = $"HighResShot_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png";
        // Application.dataPath is read-only on Android -> persistentDataPath
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, filename), bytes);

        Debug.Log("Screenshot saved to: " + filename);
#endif
    }

    void Update()
    {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        if (Input.GetKeyDown(KeyCode.P))
        {
            //Capture();

            string filename = $"Screenshot_{System.DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png";
            ScreenCapture.CaptureScreenshot(filename, 4);
            Debug.Log("Screenshot with UI saved to: " + filename);
        }
#endif
    }
}
