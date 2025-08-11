#if UNITY_EDITOR

using System.Linq;
using UnityEditor;

namespace LScene {
    public class ConfigureOnSceneAssetModify : AssetPostprocessor {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            string path = LScene.SceneLoader.ScenesPath;
            
            bool nameChange = movedAssets.Any(asset => 
                asset.StartsWith(path) && 
                movedFromAssetPaths.Any(ma => ma.StartsWith(path)));

            bool fileDeleted = deletedAssets.Any(asset => asset.StartsWith(path));
            bool fileChanged = importedAssets.Any(asset => asset.StartsWith(path));
            
            if(nameChange || fileChanged || fileDeleted)
                LScene.SceneLoader.ConfigureSceneBuilder();
        }
    }
}

#endif