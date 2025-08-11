/*
// File: Assets/Editor/SceneDataGenerator.cs
// This script must be placed in an 'Editor' folder within your Unity project (e.g., Assets/Editor/).

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// Removed System.Text.Json, now using Unity's built-in JsonUtility

namespace MyGame.Editor
{
    // Matches the structure in the source generator
    // Note: JsonUtility requires fields to be public or have [SerializeField]
    [System.Serializable] // Required for JsonUtility to serialize this class
    public class SceneInfo
    {
        public string path; // Changed to field for JsonUtility
        public bool enabled; // Changed to field for JsonUtility
    }

    // This class handles generating the scenes.json file
    [InitializeOnLoad] // Ensures this script runs when Unity loads
    public class SceneDataGenerator : IPreprocessBuildWithReport
    {
        // Path where the scenes.json file will be saved relative to the project root.
        // This path MUST match what you configure in your .csproj for AdditionalFiles.
        private const string SceneDataFilePath = "Assets/scenes.json"; // Recommended: place in Assets for easy access by generator

        static SceneDataGenerator()
        {
            // Register for scene build settings changes.
            // This is a good place to trigger regeneration.
            EditorBuildSettings.sceneListChanged += GenerateSceneDataFile;
            // Also generate on Unity startup to ensure the file exists.
            // This ensures the scenes.json is present even if build settings haven't changed.
            GenerateSceneDataFile();
        }

        // IPreprocessBuildWithReport interface method: called before a build starts.
        public int callbackOrder { get { return 0; } } // Determines the order of callbacks

        public void OnPreprocessBuild(BuildReport report)
        {
            // Ensure the scene data is up-to-date right before a build.
            Debug.Log("OnPreprocessBuild: Ensuring scene data is up-to-date.");
            GenerateSceneDataFile();
        }

        [MenuItem("Tools/Generate Scene Enum Data")]
        public static void GenerateSceneDataFile()
        {
            Debug.Log("Generating scene data for SceneEnumGenerator...");

            // JsonUtility requires a root object to serialize a list directly.
            // We'll wrap the list in a simple container class.
            var sceneListContainer = new SceneListContainer();
            sceneListContainer.scenes = new List<SceneInfo>();

            // Iterate through all scenes defined in the Editor Build Settings.
            foreach (var scene in EditorBuildSettings.scenes)
            {
                sceneListContainer.scenes.Add(new SceneInfo
                {
                    path = scene.path,
                    enabled = scene.enabled
                });
            }

            // Serialize the list of scene infos to JSON using Unity's JsonUtility.
            // JsonUtility.ToJson does not have a direct 'WriteIndented' option,
            // but it generally produces readable JSON.
            string jsonString = JsonUtility.ToJson(sceneListContainer, true); // 'true' for pretty print

            // Ensure the directory exists.
            string directory = Path.GetDirectoryName(SceneDataFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write the JSON string to the specified file path.
            File.WriteAllText(SceneDataFilePath, jsonString);

            // Refresh the Asset Database so Unity recognizes the new/updated file.
            AssetDatabase.Refresh();

            Debug.Log($"Scene data generated and saved to: {SceneDataFilePath}");
        }

        // Helper class to wrap the list of SceneInfo for JsonUtility serialization.
        // JsonUtility cannot directly serialize List<T>, it needs a class/struct wrapper.
        [System.Serializable]
        private class SceneListContainer
        {
            public List<SceneInfo> scenes;
        }
    }
}
*/
