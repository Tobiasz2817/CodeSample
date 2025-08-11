using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if FISHNET
using FishNet;
using FishNet.Managing.Scened;
#endif


namespace LScene
{
#if FISHNET
    public static class FishnetSceneLoader
    {
        public static async Awaitable LoadGlobalScene(string newScene, ReplaceOption option = ReplaceOption.All)
        {
            SceneLookupData targetScene = new SceneLookupData(newScene);

            List<string> sceneToLoad = new List<string> {
                targetScene.Name,
            };
                
            SceneLoadData sld = new SceneLoadData(sceneToLoad) {
                ReplaceScenes = option,
                PreferredActiveScene = new PreferredScene(targetScene)
            };
            
            InstanceFinder.SceneManager.LoadGlobalScenes(sld);
            SceneLoader.Scene = newScene;

            while (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != targetScene.Name) 
                await Task.Yield();
        }
    }
    
#endif
}