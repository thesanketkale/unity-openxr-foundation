using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using Debug = UnityEngine.Debug;

namespace Foundation.Editor
{
    /// <summary>
    /// Headless batchmode build runners for Standalone Android (Meta Quest) and Windows PC VR.
    /// Derives semantic versioning from git tags and configures keystores from environment variables.
    /// </summary>
    public static class BuildCommands
    {
        private static readonly string[] ScenePaths =
        {
            "Assets/_Project/Scenes/Bootstrap.unity",
            "Assets/_Project/Scenes/InteractionPlayground.unity"
        };

        public static void Android()
        {
            Debug.Log("[BuildCommands] Starting Standalone Android (Quest) Build...");
            ConfigureVersion();
            ConfigureKeystore();

            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Android");
            Directory.CreateDirectory(outputDir);
            string apkPath = Path.Combine(outputDir, $"{PlayerSettings.productName}.apk");

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = ScenePaths,
                locationPathName = apkPath,
                target = BuildTarget.Android,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            EvaluateReport(report);
        }

        public static void Windows()
        {
            Debug.Log("[BuildCommands] Starting Standalone Windows PC VR Build...");
            ConfigureVersion();

            string outputDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds", "Windows");
            Directory.CreateDirectory(outputDir);
            string exePath = Path.Combine(outputDir, $"{PlayerSettings.productName}.exe");

            var buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = ScenePaths,
                locationPathName = exePath,
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            EvaluateReport(report);
        }

        private static void ConfigureVersion()
        {
            string gitTag = GetGitVersionTag();
            string runNumber = Environment.GetEnvironmentVariable("GITHUB_RUN_NUMBER") ??
                               Environment.GetEnvironmentVariable("BUILD_NUMBER") ?? "1";

            PlayerSettings.bundleVersion = gitTag;

            if (int.TryParse(runNumber, out int buildCode))
            {
                PlayerSettings.Android.bundleVersionCode = buildCode;
            }

            Debug.Log($"[BuildCommands] Set Bundle Version: {PlayerSettings.bundleVersion}, Code: {PlayerSettings.Android.bundleVersionCode}");
        }

        private static void ConfigureKeystore()
        {
            string keystorePath = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PATH");
            string keystorePass = Environment.GetEnvironmentVariable("ANDROID_KEYSTORE_PASS");
            string keyaliasName = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_NAME");
            string keyaliasPass = Environment.GetEnvironmentVariable("ANDROID_KEYALIAS_PASS");

            if (!string.IsNullOrEmpty(keystorePath) && File.Exists(keystorePath))
            {
                PlayerSettings.Android.useCustomKeystore = true;
                PlayerSettings.Android.keystoreName = keystorePath;
                PlayerSettings.Android.keystorePass = keystorePass;
                PlayerSettings.Android.keyaliasName = keyaliasName;
                PlayerSettings.Android.keyaliasPass = keyaliasPass;
                Debug.Log("[BuildCommands] Custom Android keystore configured from environment.");
            }
            else
            {
                PlayerSettings.Android.useCustomKeystore = false;
                Debug.LogWarning("[BuildCommands] No custom keystore specified; building unsigned/development package.");
            }
        }

        private static string GetGitVersionTag()
        {
            try
            {
                var psi = new ProcessStartInfo("git", "describe --tags --always")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = Process.Start(psi);
                if (process != null)
                {
                    string output = process.StandardOutput.ReadToEnd().Trim();
                    process.WaitForExit();
                    if (!string.IsNullOrEmpty(output))
                    {
                        return output;
                    }
                }
            }
            catch
            {
                // Fallback to default
            }

            return "0.1.0";
        }

        private static void EvaluateReport(BuildReport report)
        {
            BuildSummary summary = report.summary;
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[BuildCommands SUCCESS] Build succeeded: {summary.totalSize} bytes in {summary.totalTime.TotalSeconds:F1}s.");
                EditorApplication.Exit(0);
            }
            else
            {
                Debug.LogError($"[BuildCommands FAIL] Build failed with result: {summary.result}, errors: {summary.totalErrors}");
                EditorApplication.Exit(1);
            }
        }
    }
}
