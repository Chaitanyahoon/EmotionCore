#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace EmotionCore.EditorTools
{
    public class AutoBuilder
    {
        public static void BuildProject()
        {
            Debug.Log(">>> AUTOBUILDER: Starting Build Process... <<<");

            string folderPath = "Builds";
            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                Debug.Log($">>> AUTOBUILDER: Created directory {folderPath}");
            }

            // Explicit Scene List for Build
            string[] scenes = new string[] 
            { 
                "Assets/Scenes/MainMenu.unity",             // Index 0
                "Assets/Scenes/EmotionCore_FullWorld.unity" // Index 1
            };
            Debug.Log(">>> AUTOBUILDER: Building Scenes: MainMenu (0), GameWorld (1)");

            foreach (var s in scenes) Debug.Log($">>> AUTOBUILDER: Scene to build: {s}");

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.locationPathName = "Builds/EmotionCore.exe";
            buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
            buildPlayerOptions.options = BuildOptions.None;

            Debug.Log($">>> AUTOBUILDER: Target Path: {buildPlayerOptions.locationPathName}");

            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;

            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($">>> AUTOBUILDER: Build SUCCEEDED: {summary.totalSize} bytes");
            }

            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
            {
                Debug.Log(">>> AUTOBUILDER: Build FAILED");
            }
        }
    }
}
#endif
