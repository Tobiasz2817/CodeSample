using UnityEngine.InputSystem;  
using UnityEngine;  
  
namespace Inflowis {  
    [CreateAssetMenu(menuName = "Content/Config/Inflows", fileName = "InflowisConfig")]  
    [DefaultExecutionOrder(-1)]  
    public class InflowisConfig : ScriptableObject {  
        [Header("Single")]
        [Space]  
        [SerializeField]  
        internal InputActionAsset InputActionAsset;  
        [SerializeField]   
        [ControlScheme(nameof(InputActionAsset))]  
        internal string DefaultScheme;  
        [SerializeField]  
        [Map(nameof(InputActionAsset))]  
        internal string DefaultMap;  
  
        [Header("Local Co-op")]
        [Space]
        [SerializeField]
        internal PlayerInputManager InputManagerPrefab; 
  
#if UNITY_EDITOR  
        // TODO: different way to set the config reference in Unity Editor Mode (Resources.Load is to slow)  
        void OnValidate() => InflowisCore.Config = this;  
#endif  
    }  
}