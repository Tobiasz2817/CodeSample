using GameZone.Scripts.Loader;
using UnityEngine;

namespace GameZone.Scripts.Scene
{
    public class ContinueLoaderOnNewScene : MonoBehaviour
    { 
        [SerializeField] private ActionLoader actionLoader;

        private SpinningLoader spinningLoader;
        private void Awake() {
            spinningLoader = FindObjectOfType<SpinningLoader>();
        }

        private void OnEnable() {
            SceneLoader.OnSceneLoaded += LoadNewAction;
            actionLoader.OnInvokeAction += UpdateInterface;
            actionLoader.OnEndLoad += DisableInterface;
        }

        private void OnDisable() {
            SceneLoader.OnSceneLoaded -= LoadNewAction;
            actionLoader.OnInvokeAction -= UpdateInterface;
            actionLoader.OnEndLoad -= DisableInterface;
        }
    
        private void LoadNewAction(SceneLoadDependencies obj) {
            actionLoader.LoadActions();
        }
    
        private void UpdateInterface(string obj) {
            spinningLoader.displayText.text = obj;
        }
    
        private void DisableInterface() {
            spinningLoader.CloseLoadingUI();
        }
    }

}
