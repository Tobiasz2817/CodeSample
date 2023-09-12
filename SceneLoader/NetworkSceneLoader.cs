using UnityEngine.SceneManagement;
using GameZone.Scripts.Scene;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace GameZone.Multiplayer.Scene
{
    public class NetworkSceneLoader : SceneLoader
    {
        public override void LoadScene(SceneLoadDependencies sceneLoadDependencies = null) {
            if (sceneLoadDependencies != null)
                this.sceneLoadDependencies = sceneLoadDependencies;

            LoadNetworkScene();
        }
    
        private void LoadNetworkScene() {
            NetworkManager.Singleton.SceneManager.OnLoad += SceneLoaded;
            if(NetworkManager.Singleton.IsHost)
                NetworkManager.Singleton.SceneManager.LoadScene(sceneLoadDependencies.GetSceneName(),LoadSceneMode.Single);
        }
    
        private void SceneLoaded(ulong clientid, string scenename, LoadSceneMode loadscenemode, AsyncOperation asyncoperation) {
            StartCoroutine(LoadYourAsyncNetworkScene(asyncoperation));
            NetworkManager.Singleton.SceneManager.OnLoad -= SceneLoaded;
        }
    
        IEnumerator LoadYourAsyncNetworkScene(AsyncOperation asyncoperation) 
        {
            OnStartLoading?.Invoke(sceneLoadDependencies);
        
        
            while (!asyncoperation.isDone)
            {
                yield return null;
            }
            Debug.Log("Is Done");
        
            yield return EndOfFrame;
        
            OnSceneLoaded?.Invoke(sceneLoadDependencies);
        }


    }
}

