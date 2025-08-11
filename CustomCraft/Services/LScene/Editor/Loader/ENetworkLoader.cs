#if UNITY_EDITOR && FISHNET

using System.Threading.Tasks;
using Object = UnityEngine.Object;
using FishNet.Managing;
using LScene.Editor;
using UnityEngine;
using FishNet;

namespace LScene.Network {
    /// <summary>
    /// Class making connection with the system and loading scene with transition
    /// </summary>
    public static class ENetworkLoader {
        static NetworkManager _networkManager;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void AfterLoadScene() =>
            _networkManager = Object.FindFirstObjectByType<NetworkManager>();
        
#if FISHNET
        public static async void AsClient()
        {
            string targetScene = string.Empty;
            
#if UNITY_EDITOR
            targetScene = ESceneLoader.Data.ScenesToLoadPath[0];
#else   
            targetScene = nameof(MainScene.Menu);
#endif
            
            TransitionType transType = SceneLoader.GetTransition(targetScene);
            await SceneTransition.ShowTransition(transType);
            
            _networkManager.ClientManager.StartConnection();
            
            while (!_networkManager.ClientManager.Started)
                await Task.Yield();
            
            await SceneTransition.HideTransition();
        }

        public static async Awaitable AsServer()
        {
            string targetScene = string.Empty;
            
#if UNITY_EDITOR
            targetScene = ESceneLoader.Data.ScenesToLoadPath[0];
#else   
            targetScene = nameof(MainScene.Menu);
#endif
            
            TransitionType transType = SceneLoader.GetTransition(targetScene);
            await SceneTransition.ShowTransition(transType);
            
            _networkManager.ServerManager.StartConnection();
            
            while (!_networkManager.ServerManager.Started)
                await Task.Yield();
            
            await FishnetSceneLoader.LoadGlobalScene(targetScene);
            await SceneTransition.HideTransition();
        }
        
        public static async void AsHost()
        {
            await AsServer();
            
            _networkManager.ClientManager.StartConnection();
        }
    }
#endif
}

#endif
