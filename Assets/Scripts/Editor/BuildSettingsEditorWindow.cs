using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildSettingsWindow : EditorWindow {
    private string buildPath = "../Indra-RL-Lab/unity/builds";
    private bool buildUC1 = true;
    private bool buildUC2 = true;
    private bool buildUC3 = true;
    private string uc1ScenePath = "Assets/Scenes/UC1Scene.unity";
    private string uc2ScenePath = "Assets/Scenes/UC2Scene.unity";
    private string uc3ScenePath = "Assets/Scenes/UC3Scene.unity";

    [MenuItem("Build/Build Settings")]
    public static void ShowWindow() {
        GetWindow<BuildSettingsWindow>("Build Settings");
    }

    void OnGUI() {

        GUILayout.Space(10);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Build Path:", GUILayout.Width(80));
        buildPath = EditorGUILayout.TextField(buildPath);

        if (GUILayout.Button("Browse", GUILayout.Width(80))) {
            string selectedPath = EditorUtility.OpenFolderPanel("Select Build Folder", buildPath, "");
            if (!string.IsNullOrEmpty(selectedPath)) {
                buildPath = selectedPath;
            }
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.Label("Scenes to Build:", EditorStyles.boldLabel);
        buildUC1 = EditorGUILayout.ToggleLeft("UC1 Scene", buildUC1);
        buildUC2 = EditorGUILayout.ToggleLeft("UC2 Scene", buildUC2);
        buildUC3 = EditorGUILayout.ToggleLeft("UC3 Scene", buildUC3);

        GUILayout.Space(10);

        if (GUILayout.Button("Build Windows")){
            if (buildUC1) 
                BuildManager.BuildWindows(uc1ScenePath, buildPath, "uc1");
            if (buildUC2)
                BuildManager.BuildWindows(uc2ScenePath, buildPath, "uc2");
            if (buildUC3)
                BuildManager.BuildWindows(uc3ScenePath, buildPath, "uc3");
        }

        if (GUILayout.Button("Build Linux")) {
            if (buildUC1)
                BuildManager.BuildLinux(uc1ScenePath, buildPath, "uc1");
            if (buildUC2)
                BuildManager.BuildLinux(uc2ScenePath, buildPath, "uc2");
            if (buildUC3)
                BuildManager.BuildLinux(uc3ScenePath, buildPath, "uc3");
        }

        if (GUILayout.Button("Build All Platforms")) {
            if (buildUC1)
                BuildManager.BuildAll(uc1ScenePath, buildPath, "uc1");
            if (buildUC2)
                BuildManager.BuildAll(uc2ScenePath, buildPath, "uc2");
            if (buildUC3)
                BuildManager.BuildAll(uc3ScenePath, buildPath, "uc3");
        }
    }
}

public class BuildManager {
    public static void BuildAll(string scene, string buildPath, string buildName) {

        BuildForPlatform(BuildTarget.StandaloneWindows64, scene, buildPath, buildName);
        BuildForPlatform(BuildTarget.StandaloneLinux64, scene, buildPath, buildName);
        Debug.Log("Build process completed successfully for all platforms!");
    }

    public static void BuildWindows(string scene, string buildPath, string buildName) {
        BuildForPlatform(BuildTarget.StandaloneWindows64, scene, buildPath, buildName);
        Debug.Log("Build process completed successfully for Windows!");
    }

    public static void BuildLinux(string scene, string buildPath, string buildName) {
        BuildForPlatform(BuildTarget.StandaloneLinux64, scene, buildPath, buildName);
        Debug.Log("Build process completed successfully for Linux!");
    }

    private static void BuildForPlatform(BuildTarget target, string scene, string buildFolder, string buildName) {

        string platformFolder;
        string extension;

        switch (target) {
            case BuildTarget.StandaloneWindows64:
                platformFolder = "windows";
                extension = ".exe";
                break;
            case BuildTarget.StandaloneLinux64:
                platformFolder = "linux";
                extension = "";
                break;
            default:
                Debug.LogError("Unsupported build target: " + target);
                return;
        }

        string[] scenes = { scene };

        // Build options for selected scenes
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions {
            scenes = scenes,
            locationPathName = Path.Combine(buildFolder, platformFolder, buildName, buildName + extension),
            target = target,
            options = BuildOptions.Development
        };

        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}




// using UnityEngine;
// using UnityEditor;
// using System.IO;

// public class BuildMenuItems {

//     [MenuItem("Build/Build All Platforms")]
//     public static void BuildAll() {

//         // Build for Windows
//         BuildForPlatform(BuildTarget.StandaloneWindows64);

//         // Build for Linux
//         BuildForPlatform(BuildTarget.StandaloneLinux64);

//         Debug.Log("Build process completed successfully!");
//     }

//     [MenuItem("Build/Build Windows")]
//     public static void BuildWindows() {
//         BuildForPlatform(BuildTarget.StandaloneWindows64);
//     }

//     [MenuItem("Build/Build Linux")]
//     public static void BuildLinux() {
//         BuildForPlatform(BuildTarget.StandaloneLinux64);
//     }


//     private static void BuildForPlatform(BuildTarget target) {

//         string buildFolder = "../builds";
//         string platformFolder;
//         string extension;


//         switch (target) {
//             case BuildTarget.StandaloneWindows64:
//                 platformFolder = "windows";
//                 extension = ".exe";
//                 break;
//             case BuildTarget.StandaloneLinux64:
//                 platformFolder = "linux";
//                 extension = "";
//                 break;
//             default:
//                 Debug.LogError("Unsupported build target: " + target);
//                 return;
//         }

//         // Define the scenes to build
//         string[] scenesUC1 = { "Assets/Scenes/UC1Scene.unity" };
//         string[] scenesUC2 = { "Assets/Scenes/UC2Scene.unity" };

//         // Build UC1 Scene
//         BuildPlayerOptions optionsUC1 = new BuildPlayerOptions {
//             scenes = scenesUC1,
//             locationPathName = Path.Combine(buildFolder, platformFolder, "uc1", "UC1" + extension),
//             target = target,
//             options = BuildOptions.Development
//         };

//         BuildPipeline.BuildPlayer(optionsUC1);

//         // Build UC2 Scene
//         BuildPlayerOptions optionsUC2 = new BuildPlayerOptions {
//             scenes = scenesUC2,
//             locationPathName = Path.Combine(buildFolder, platformFolder, "uc2", "UC2" + extension),
//             target = target,
//             options = BuildOptions.Development
//         };

//         BuildPipeline.BuildPlayer(optionsUC2);
//     }
// }
