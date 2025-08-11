#if UNITY_EDITOR

using CoreUtility.Extensions;
using CustomInspector;
using UnityEditor;
using UnityEngine;

namespace LScene.Editor {
    [CustomEditor(typeof(SceneConfig))]
    public class SceneConfigEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SceneConfig.CoreScene)));
            
            GUILayout.Space(10f);
            
            var scenes = EditorBuildSettings.scenes;
            string[] scenesName = new string[scenes.Length];
            for (int i = 0; i < scenesName.Length; i++) {
                string path = scenes[i].path;
                int backSpaceIndex = 0;
                for (int j = path.Length - 1; j >= 0; j--) {
                    if (path[j] == '/') {
                        backSpaceIndex = j;
                        break;
                    }
                }

                string sceneName = path.Substring(backSpaceIndex, path.Length - backSpaceIndex);
                sceneName = sceneName.RemoveExtension();
                sceneName = sceneName.ToNonAlNum();
                scenesName[i] = sceneName;
            }
            
            StructInspector.DrawFromTo(
                serializedObject,
                nameof(SceneConfig.SceneTransitions), 
                nameof(Transitions.FromScene), 
                nameof(Transitions.ToScene), 
                nameof(Transitions.Transition), 
                scenesName,
                "Transitions", "Add custom transition");
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(SceneConfig.TransitionViews)));
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif