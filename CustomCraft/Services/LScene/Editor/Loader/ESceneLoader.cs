#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Reflection;
using UnityEditor;
using System.Linq;
using System;
using System.IO;

#if FISHNET
using System.Linq;
using UnityEngine;
using FishNet.Managing;
using LScene.Network;
using Unity.Multiplayer.Playmode;
using FishNet.Transporting;
#endif

namespace LScene.Editor
{
    [InitializeOnLoad]
    public static class ESceneLoader
    {
#if FISHNET
        const string CloneWindowTag = "CLONE";
#endif
        
        static EditorLoaderData? _data;
        public static EditorLoaderData Data => _data ??= EditorLoaderData.LoadData();

        static ESceneLoader()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange state) {
            var data = EditorLoaderData.LoadData();
            
            switch (state) {
                case PlayModeStateChange.ExitingEditMode: {
                    if (!Data.CustomLoading) return;
                    
                    List<string> scenesToLoad = new List<string>();

                    for (int i = 0; i < SceneManager.sceneCount; i++)
                    {
                        var scene = SceneManager.GetSceneAt(i);
                        if (!scene.isLoaded) continue;
                        
                        scenesToLoad.Add(scene.path);
                    }
                    
                    scenesToLoad = scenesToLoad
                        .OrderBy(s => !Enum.TryParse(typeof(MainScene), Path.GetFileNameWithoutExtension(s), out _))
                        .ToList();
                    
                    data.ScenesToLoadPath = scenesToLoad.ToArray();
                        
                    EditorLoaderData.SaveData(data);
                    EditorSceneManager.OpenScene(data.InitScenePath);
                }
                    break;
                case PlayModeStateChange.EnteredEditMode: {
                    if (!data.CustomLoading 
#if FISHNET
                        || IsCloneWindow()
#endif
                        ) return;
                    
                    var scenes = data.ScenesToLoadPath;
                    for (int i = 0; i < scenes.Length; i++)
                    {
                        OpenSceneMode mode = i == 0 ? OpenSceneMode.Single :  OpenSceneMode.Additive;
                        
                        EditorSceneManager.OpenScene(scenes[i], mode);
                    }
                }
                    break;
            }
        }

        public static void RunLoadingSequence()
        {
            switch (Data.NetworkLoading)
            {
#if FISHNET
                case true:
                {
                    if (Data.AutomaticConnection)
                    {
                        const string Host = "HOST";
                        var mpm = CurrentPlayer.ReadOnlyTags();
                        if (mpm.Contains(Host)) 
                            ENetworkLoader.AsHost();
                        else ENetworkLoader.AsClient();
                    }
                    else
                    {
                        var networkManager = UnityEngine.Object.FindFirstObjectByType<NetworkManager>();
                        if (networkManager == null)
                        {
                            Debug.LogError("NetworkManager not found, HUD will not function.");
                            return;
                        }
                    
                        networkManager.ServerManager.OnServerConnectionState -= ServerManager_OnServerConnectionState;
                        networkManager.ServerManager.OnServerConnectionState += ServerManager_OnServerConnectionState;
                    }
                }
                    break;
#endif
                case false when !Data.ScenesToLoadPath.Contains(Data.InitScenePath):
                {
                    _= SceneLoader.ChangeScenes(Data.ScenesToLoadPath);
                }
                    break;
            }
        } 
        
#if FISHNET
        static void ServerManager_OnServerConnectionState(ServerConnectionStateArgs state)
        {
            if (state.ConnectionState != LocalConnectionState.Started)
                return;
            
            _= FishnetSceneLoader.LoadGlobalScene(Data.ScenesToLoadPath[0]);
        }
            
        static bool IsCloneWindow() => CurrentPlayer.ReadOnlyTags().Contains(CloneWindowTag);
#endif

        static void ClearConsoleLogs() {
            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");
            method?.Invoke(new object(), null);
        }
    }
}

#endif