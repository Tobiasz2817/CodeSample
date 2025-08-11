#if UNITY_EDITOR

using System;
using System.Linq;
using CustomInspector;
using UnityEditor;
using UnityEngine;

namespace CoreSystems.Editor {
    [CustomEditor(typeof(ConfigInjectorData))]
    public class CustomInjectorEditor : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            SerializedProperty showLogsProperty = serializedObject.FindProperty(nameof(ConfigInjectorData.ShowLogs));
            SerializedProperty assemblies = serializedObject.FindProperty(nameof(ConfigInjectorData.Assemblies));
            serializedObject.Update();
            
            GUILayout.Space(5f);
            EditorGUILayout.PropertyField(showLogsProperty);
            
            GUILayout.Space(5f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                EditorGUILayout.TextField("Config Injection Assemblies", EditorStyles.boldLabel);
                
                for (int i = 0; i < assemblies.arraySize; i++) {
                    var element = assemblies.GetArrayElementAtIndex(i);

                    var buttonStyle = new GUIStyle(EditorStyles.popup) {
                        alignment = TextAnchor.MiddleCenter,
                    };
                    
                    if (GUILayout.Button(element.stringValue, buttonStyle))
                    {
                        var assembliesStrings = AppDomain.CurrentDomain
                            .GetAssemblies()
                            .Select(a => a.GetName().Name)
                            .Where(assemblyName => !IsUnityAssembly(assemblyName))
                            .Distinct()
                            .ToArray();
                        
                        CustomWindow.Show(GUILayoutUtility.GetLastRect(), assembliesStrings, assemblyName =>
                        {
                            assemblies.serializedObject.Update();
                            element.stringValue = assemblyName;
                            assemblies.serializedObject.ApplyModifiedProperties();
                        });
                    }
                }
                
                GUILayout.Space(10f);
                EditorGUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("+")) assemblies.arraySize++;
                    if (GUILayout.Button("-")) assemblies.arraySize = assemblies.arraySize > 0 ? assemblies.arraySize - 1 : 0;
                    
                } EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            serializedObject.ApplyModifiedProperties();
        }
        
        bool IsUnityAssembly(string assemblyName)
        {
            return
                assemblyName.StartsWith("Unity") ||
                assemblyName.StartsWith("UnityEditor") ||
                assemblyName.StartsWith("UnityEngine") ||
                assemblyName.StartsWith("Unity.") ||
                assemblyName.StartsWith("UnityEditor.") ||
                assemblyName.StartsWith("System") || 
                assemblyName.StartsWith("Mono.") || 
                assemblyName == "mscorlib" ||
                assemblyName == "netstandard";
        }
    }
}

#endif