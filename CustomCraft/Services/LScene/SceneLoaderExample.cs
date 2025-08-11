using UnityEngine;

namespace LScene {
    public class SceneLoaderExample : MonoBehaviour {
        void Awake() {
            DontDestroyOnLoad(this);
        }

        public void LoadScene(MainScene scene) {
            LScene.SceneLoader.ChangeScene(scene);
        }

        public void LoadLevel(LevelScene level) {
            LScene.SceneLoader.ChangeScene(level);
        }
    }
}