using SceneManager = UnityEngine.SceneManagement.SceneManager;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreUtility;
using UnityEngine;
using System.Linq;
using System;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

namespace LScene {
    // TODO: Make async loading couple scenes until is scene transition working
    // Scene Loader loading the main scenes and levels
    // You can use one scene transition per one scene change
    public static class SceneLoader {
        static readonly SceneConfig _sceneConfig;
        internal const string ScenesPath = "Assets/Scenes/Release";
        
        internal static string Scene;
        internal static bool IsLoading;
        
        static readonly HashSet<LevelScene> _levels = new();

        [RuntimeInitializeOnLoadMethod]
        static void Initialize() => Scene = SceneManager.GetActiveScene().name;

        public static async Awaitable TChangeScene(MainScene mainScene, bool reloadLevels = false) {
            await SceneTransition.ShowTransition(GetTransition(mainScene.ToString()));

            await ChangeScene(mainScene, reloadLevels);
            
            _= SceneTransition.HideTransition();
        }
        
        /// <summary>
        /// Features:
        /// - Support only core reload levels
        /// - Changing main scenes with transition
        /// - Base un loading levels
        /// </summary>
        /// <param name="newScene"> Main scene id </param>
        /// <param name="reloadLevels"> Does core scene loading levels ? </param>
        public static async Awaitable ChangeScene(MainScene newScene, bool reloadLevels = true) {
            Scene = newScene.ToString();
            
            foreach (var level in _levels) 
                _ = SceneManager.UnloadSceneAsync(level.ToString());

            string coreScene = _sceneConfig.CoreSceneName;
            bool isReloadLevels = reloadLevels && coreScene == Scene;
            if (!isReloadLevels)
                _levels.Clear();
            
            await LoadSceneAsync(Scene, LoadSceneMode.Single);
            
            var sceneAtIndex = SceneManager.GetSceneByName(Scene);
            SceneManager.SetActiveScene(sceneAtIndex);
            
            if (isReloadLevels) {
                foreach (var level in _levels) {
                    string levelName = level.ToString();
                    await LoadSceneAsync(levelName);
                    await LoadLevelDependencies(level);
                }
            }
        }
        
        public static async Awaitable TChangeScene(LevelScene levelScene, bool unloadOthers = false) {
            await SceneTransition.ShowTransition(GetTransition(levelScene.ToString()));

            await ChangeScene(levelScene, unloadOthers);
            
            _= SceneTransition.HideTransition();
        }
        
        /// <summary>
        /// Features:
        /// - Showing transition while scene is changing
        /// - Support core scene handler means only core scenes can have levels
        /// - Support reload self-level
        /// - When we load level and Core is not loaded will load it
        /// </summary>
        /// <param name="levelScene"> Scene level id </param>
        /// <param name="unloadOthers"> unload levels, when it's false reload the same level </param>
        public static async Awaitable ChangeScene(LevelScene levelScene, bool unloadOthers = false) {
            string levelName = levelScene.ToString();
            
            // Reload current level
            if (_levels.Remove(levelScene)) 
                _ = SceneManager.UnloadSceneAsync(levelName);

            // Unloading levels
            if (unloadOthers) {
                foreach (var level in _levels) 
                    _ = SceneManager.UnloadSceneAsync(level.ToString());
            
                _levels.Clear();
            }
            
            // It depends on core scene
            // level's need core scene
            string coreScene = _sceneConfig.CoreSceneName;
            if (Scene != coreScene) {
                string oldScene = Scene;
                await LoadSceneAsync(coreScene);
                _ = SceneManager.UnloadSceneAsync(oldScene);
                
                Scene = coreScene;
            }
            
            await LoadSceneAsync(levelName);
            _levels.Add(levelScene);
            
            await LoadLevelDependencies(levelScene);
        }

        /// <summary>
        /// Loading scene by name, better performance is with enum as parameter methods
        /// </summary>
        /// <param name="newScene"></param>
        /// <param name="unloadOthers"></param>
        /// <param name="reloadLevels"></param>
        public static async Awaitable ChangeScene(string newScene, bool unloadOthers = false, bool reloadLevels = true) {
            // Check if it's level scene
            bool isLevel = false;
            foreach (string enumValue in Enum.GetNames(typeof(LevelScene)))
                isLevel = isLevel || string.Equals(enumValue, newScene);

            switch (isLevel)
            {
                case true:
                {
                    LevelScene levelScene = (LevelScene)Enum.Parse(typeof(LevelScene), newScene);
                    await ChangeScene(levelScene, unloadOthers);
                }
                    break;
                
                case false:
                {
                    MainScene mainScene = (MainScene)Enum.Parse(typeof(MainScene), newScene);
                    await ChangeScene(mainScene, reloadLevels);
                }
                    break;
            }
        }
        
        public static async Awaitable ChangeScenes(string[] scenesPath) {
            string first = scenesPath[0];
            
            await SceneTransition.ShowTransition(GetTransition(Path.GetFileNameWithoutExtension(first)));
            
            foreach (var scene in scenesPath)
                await ChangeScene(Path.GetFileNameWithoutExtension(scene));

            _= SceneTransition.HideTransition();
        }
        
        
        public static TransitionType GetTransition(string nextScene) {
            return _sceneConfig.SceneTransitions.
                Where(scene => scene.FromScene == Scene && scene.ToScene == nextScene).
                Select(scene => scene.Transition).
                DefaultIfEmpty(TransitionType.Default).
                First();
        }
        
        static async Awaitable LoadSceneAsync(string scene, LoadSceneMode mode = LoadSceneMode.Additive) {
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(scene, mode);
            
            while (!loadOperation!.isDone) 
                await Task.Yield();
        }

        static async Awaitable LoadLevelDependencies(LevelScene level) {
            // make action async loader for some action like... loading the world
            GameObject referenceToScene = new GameObject("ReferenceToScene");
            int lastSceneId = SceneManager.sceneCount - 1;
            var scene = SceneManager.GetSceneAt(lastSceneId);
            SceneManager.MoveGameObjectToScene(referenceToScene, scene);
                
            await DependencyLoader.LoadDependencies(level, referenceToScene);
            
            Object.DestroyImmediate(referenceToScene);
        }
        
#if UNITY_EDITOR
        
        [DidReloadScripts]
        static void CreateConfig() => PersistentFactory.CreateConfig<SceneConfig>("SceneConfig");
        
        
        [InitializeOnLoadMethod]
        internal static void ConfigureSceneBuilder() {
            // Load Assets
            List<EditorBuildSettingsScene> curScenes = EditorBuildSettings.scenes.ToList();
            for (int i = curScenes.Count - 1; i >= 0; i--)
                if (!curScenes[i].enabled)
                    curScenes.RemoveAt(i);
            
            EditorBuildSettings.scenes = curScenes.ToArray();

            var newScenes = new HashSet<EditorBuildSettingsScene>();
            foreach (var scene in EditorBuildSettings.scenes) 
                newScenes.Add(scene);
            
            List<(string path, SceneAsset scene)> scenes = GetAssetScenes();
            
            // Load references to editor build
            Array mainScenesArray = Enum.GetValues(typeof(MainScene));
            MainScene[] mainScenes = new MainScene[mainScenesArray.Length];
            for (int i = 0; i < mainScenes.Length; i++) 
                mainScenes[i] = (MainScene)mainScenesArray.GetValue(i);
            
            foreach (var sceneType in mainScenes) {
                string sceneTypeName = sceneType.ToString();
                foreach (var sceneData in scenes) {
                    var existScene = newScenes.FirstOrDefault(x => x.path == sceneData.path);
                    if (existScene != null) 
                        continue;
                    
                    if (sceneData.scene.name.Contains(sceneTypeName)) {
                        newScenes.Add(new EditorBuildSettingsScene(sceneData.path, true));
                        break;
                    }
                }
            }
            
            // Load references to editor build
            Array levelSceneArray = Enum.GetValues(typeof(LevelScene));
            LevelScene[] levelScene = new LevelScene[levelSceneArray.Length];
            for (int i = 0; i < levelScene.Length; i++) 
                levelScene[i] = (LevelScene)levelSceneArray.GetValue(i);
            
            foreach (var sceneType in levelScene) {
                string sceneTypeName = sceneType.ToString();
                foreach (var sceneData in scenes) {
                    var existScene = newScenes.FirstOrDefault(x => x.path == sceneData.path);
                    if (existScene != null) 
                        continue;
                    
                    if (sceneData.scene.name.Contains(sceneTypeName)) {
                        newScenes.Add(new EditorBuildSettingsScene(sceneData.path, true));
                        break;
                    }
                }
            }
            
            EditorBuildSettings.scenes = newScenes.ToArray();
        }
        static List<(string path, SceneAsset scene)> GetAssetScenes() {
            var scenes = new List<(string path, SceneAsset scene)>();
            
            var guids = AssetDatabase.FindAssets("t:Scene", new[] { ScenesPath });
            foreach (var guid in guids) {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var scene = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                
                scenes.Add((path, scene));
            }

            return scenes;
        }

#endif
    }

    public static class DependencyLoader {
        public static async Awaitable LoadDependencies(LevelScene sceneId, GameObject target) {
            switch (sceneId) {
                case (int)LevelScene.Demo: {
                    SceneTransition.View.StatusText = "Loading dependencies...";
                    await Awaitable.WaitForSecondsAsync(1f);
                    SceneTransition.View.StatusText = "";
                    
                    await Awaitable.WaitForSecondsAsync(1f);
                }
                    return;
            }
            
            Object.Destroy(target);
        }
    }
    
    public enum MainScene {
        Boot,
        Menu,
        LocalMenu,
        Core,
    }

    public enum LevelScene {
        Demo,
        DriveRun,
    }
}