using System;
using System.Collections.Generic;
using LScene.Editor;
using UnityEngine;

namespace LScene {
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "Content/Config/Scene")]
    public class SceneConfig : ScriptableObject {
        [Space] 
        [Space] 
        [SceneId] 
        [SerializeField] internal int CoreScene;
        
        [HideInInspector] 
        [SerializeField] internal string CoreSceneName;
        
        [Space]
        [SerializeField] internal List<Transitions> SceneTransitions = new();
        
        [Space]
        [Header("Scene Transition")]
        [SerializeField] internal TransitionView[] TransitionViews;
    }

    [Serializable]
    struct Transitions {
        [SerializeField] internal TransitionType Transition;
        [SerializeField] internal string FromScene;
        [SerializeField] internal string ToScene;
    }
}