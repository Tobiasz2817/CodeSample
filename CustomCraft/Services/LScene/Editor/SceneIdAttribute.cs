#if UNITY_EDITOR

using System;
using CoreUtility.Extensions;
using UnityEditor;
using UnityEngine;

namespace LScene.Editor {
    [AttributeUsage(AttributeTargets.Field)]
    public class SceneIdAttribute : PropertyAttribute { }
    
    [CustomPropertyDrawer(typeof(SceneIdAttribute))]
    public class SceneIdDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if (property.propertyType != SerializedPropertyType.Integer) {
                EditorGUI.LabelField(position, "Scene IDs must be a integer");
                return;
            }

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
            
            var propertySceneName = property.serializedObject.FindProperty(nameof(SceneConfig.CoreSceneName));
            
            int coreId = property.intValue;
            var popupId = EditorGUI.Popup(position, property.displayName, coreId, scenesName);
            property.intValue = popupId;
            
            propertySceneName.stringValue = scenesName[coreId];
        }
    }
}

#endif