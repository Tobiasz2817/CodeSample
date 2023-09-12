using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine;
using System;


namespace GameZone.Scripts.Scene
{
    public class SceneLoader : MonoBehaviour
    {
        public static Action<SceneLoadDependencies> OnStartLoading; 
        public static Action<SceneLoadDependencies> OnSceneLoaded; 
    
        protected WaitForEndOfFrame EndOfFrame;
    
        [SerializeField] protected SceneLoadDependencies sceneLoadDependencies;
    
        private void Awake()
        {
            DontDestroyOnLoad(this);
            EndOfFrame = new WaitForEndOfFrame();   
        }
    
        public virtual void LoadScene(SceneLoadDependencies sceneLoadDependencies = null) {
            if (sceneLoadDependencies == null)
                sceneLoadDependencies = this.sceneLoadDependencies;
            StartCoroutine(LoadYourAsyncScene(sceneLoadDependencies));
        }
    
        IEnumerator LoadYourAsyncScene(SceneLoadDependencies sceneLoadDependencies)
        {
            OnStartLoading?.Invoke(sceneLoadDependencies);
        
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneLoadDependencies.GetSceneName());

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        
            yield return EndOfFrame;
        
            OnSceneLoaded?.Invoke(sceneLoadDependencies);
        }

        public void Destroy() {
            Destroy(gameObject);
        }
    }


    [Serializable]
    public class SceneLoadDependencies
    {
        [SerializeField] private string nameScene;

        public string GetSceneName() => nameScene;
    }
}
