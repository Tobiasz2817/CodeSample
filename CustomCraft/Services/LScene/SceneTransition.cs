using Object = UnityEngine.Object;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using System.Linq;

namespace LScene {
    public static class SceneTransition {
        const int SortOrder = 1000;
        
        static SceneConfig _config;
        static Transform _canvas;
        
        internal static TransitionView View;
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize() {
            var canvasTransform = new GameObject("SceneTransitionCanvas").AddComponent<RectTransform>();
            var canvas = canvasTransform.gameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = SortOrder;
            var scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            canvas.gameObject.AddComponent<GraphicRaycaster>();
            
            Object.DontDestroyOnLoad(canvasTransform);
            
            _canvas = canvasTransform;
        }
        
        public static async Awaitable ShowTransition(TransitionType transitionType) {
            var viewPrefab = _config.TransitionViews.FirstOrDefault(view => view.TransitionType == transitionType);
            View = Object.Instantiate(viewPrefab, _canvas);
            View.Open();
            
            while (!View.IsOpen) 
                await Task.Yield();
        }

        public static async Awaitable HideTransition() {
            View.Close();
            
            while (!View.IsClosed) 
                await Task.Yield();
            
            Object.DestroyImmediate(View.gameObject);
        }
    }
}