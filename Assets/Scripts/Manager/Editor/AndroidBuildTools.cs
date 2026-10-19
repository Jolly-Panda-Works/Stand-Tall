#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace JollyPanda.LastFlag.EditorTools
{
    /// <summary>
    /// Tools > LastFlag > Build > Android Test APK
    /// Quick local APK signed with the Android debug key (no keystore password needed), for testing on a phone.
    /// Release builds with the real keystore are made by GitHub Actions (.github/workflows/android-release.yml).
    /// Batch mode: Unity -batchmode -quit -projectPath . -buildTarget Android -executeMethod JollyPanda.LastFlag.EditorTools.AndroidBuildTools.BuildTestApk
    /// </summary>
    public static class AndroidBuildTools
    {
        [MenuItem("Tools/LastFlag/Build/Android Test APK")]
        public static void BuildTestApk()
        {
            bool useCustomKeystore = PlayerSettings.Android.useCustomKeystore;
            bool buildAab = EditorUserBuildSettings.buildAppBundle;
            try
            {
                PlayerSettings.Android.useCustomKeystore = false; // debug key
                EditorUserBuildSettings.buildAppBundle = false;

                string path = Path.Combine("Builds", "Android", $"{PlayerSettings.productName}-{PlayerSettings.bundleVersion}-test.apk");
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                var options = new BuildPlayerOptions
                {
                    scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                    locationPathName = path,
                    target = BuildTarget.Android,
                    options = BuildOptions.Development
                };

                var report = BuildPipeline.BuildPlayer(options);
                Debug.Log($"[AndroidBuildTools] {report.summary.result}: {path} ({report.summary.totalSize / (1024 * 1024)} MB, {report.summary.totalErrors} errors)");

                if (Application.isBatchMode && report.summary.result != BuildResult.Succeeded)
                    EditorApplication.Exit(1);
            }
            finally
            {
                PlayerSettings.Android.useCustomKeystore = useCustomKeystore;
                EditorUserBuildSettings.buildAppBundle = buildAab;
            }
        }
    }
}
#endif
