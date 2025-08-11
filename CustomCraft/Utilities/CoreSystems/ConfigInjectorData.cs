using System.Collections.Generic;
using UnityEngine;

namespace CoreSystems {
    [CreateAssetMenu(fileName = "ConfigInjectorData", menuName = "Content/Config/InjectorConfig")]
    public class ConfigInjectorData : ScriptableObject {
        public bool ShowLogs;
        
        public List<string> Assemblies = Default;

        static List<string> Default => new (){ 
            "AnimationEvents",
            "AnimationView",
            "CoreSystems",
            "CoreUtility",
            "SceneLoader",
            "CharControl2D",
            "Ability",
            "Inflowis",
            "Game" 
        };
    }
}