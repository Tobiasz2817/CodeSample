#if UNITY_EDITOR

using System;
using UnityEditor;
using UnityEngine;

namespace LScene.Editor {
    
    [CustomEditor(typeof(SceneLoaderExample))]
    public class SceneLoaderExampleEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            var example = (SceneLoaderExample)target; 
            
            foreach (MainScene mainScene in Enum.GetValues(typeof(MainScene))) {
                if (GUILayout.Button("Load Scene: " + mainScene)) {
                    example.LoadScene(mainScene);
                }
            }
            
            foreach (LevelScene levelScene in Enum.GetValues(typeof(LevelScene))) {
                if (GUILayout.Button("Load Level: " + levelScene)) {
                    example.LoadLevel(levelScene);
                }
            }
        }
    }
}

#endif