#if UNITY_EDITOR

using UnityEngine.InputSystem;
using UnityEngine;
using UnityEditor;
using Inflowis;

namespace UI.Popups.Editor {
    [CustomEditor(typeof(RebindMapData), true)]
    public class RebindMapEditor : UnityEditor.Editor {
        SerializedProperty _bindsProperty;
        SerializedProperty _deviceTypeProperty;
        SerializedProperty _syncAllowedFirstForAll;
        
        void OnEnable() {
            _bindsProperty = serializedObject.FindProperty("_binds");
            _deviceTypeProperty = serializedObject.FindProperty("_deviceType");
            _syncAllowedFirstForAll = serializedObject.FindProperty("_syncAllowedFirstForAll");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(_deviceTypeProperty);
            EditorGUILayout.PropertyField(_syncAllowedFirstForAll);
            
            GUILayout.Space(10);
                
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add")) {
                _bindsProperty.arraySize++;
            }
            
            if (GUILayout.Button("Remove")) {
                if(_bindsProperty.arraySize > 0)
                    _bindsProperty.arraySize--;
            }
            
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Space(10);
           
            for (int index = 0; index < _bindsProperty.arraySize; index++) {
                SerializedProperty bindElement = _bindsProperty.GetArrayElementAtIndex(index);

                SerializedProperty bindingIndexProperty = bindElement.FindPropertyRelative("_bindingIndex");
                SerializedProperty actionProperty = bindElement.FindPropertyRelative("_action");
                SerializedProperty deviceFullProperty = bindElement.FindPropertyRelative("_allowedDevices");

                if (actionProperty.objectReferenceValue is InputActionReference action) {
                    var bindings = action.action.bindings;
                    int bindingsCount = bindings.Count;

                    var namesList = new string[bindingsCount];

                    for (int i = 0; i < bindingsCount; i++) {
                        var bind = bindings[i];
                        
                        if (bind.isComposite) {
                            namesList[i] = string.Empty;
                            continue;
                        }
                        
                        string basePath = bind.effectivePath;
                        string bindType = string.Empty;
                        string deviceType = string.Empty;
                        bool previousSymbol = false;
                        bool firstSlash = true;

                        int x = 0;
                        for (int j = 1; j < basePath.Length - 1; j++) {
                            if (basePath[j] == '>') {
                                deviceType = basePath.Substring(1, j - 1);

                                x = j;
                                break;
                            }
                        }

                        for (int j = x; j < basePath.Length; j++) {
                            if (basePath[j] == '/') {
                                if(!firstSlash) 
                                    bindType += '-';
                                
                                firstSlash = false;
                            }
                            
                            bool isLetter = char.IsLetterOrDigit(basePath[j]);
                            if (isLetter) 
                                bindType += previousSymbol ? char.ToUpper(basePath[j]) : basePath[j];
                            
                            previousSymbol = !isLetter;
                        }
                        
                        string bindGroup = string.IsNullOrEmpty(bind.groups) ? "(Not Assigned)" : bind.groups.TrimStart(';');
                        string bindName = string.IsNullOrEmpty(bind.name) ? "" : bind.name + ": ";

                        namesList[i] = $"{bindName}{bindType} [{deviceType}, {bindGroup}]";
                    }

                    int bindingIndex = bindingIndexProperty.intValue;
                    bindingIndexProperty.intValue = EditorGUILayout.Popup("Binding Index", bindingIndex, namesList);
                }
                
                EditorGUILayout.PropertyField(actionProperty);
                EditorGUILayout.PropertyField(deviceFullProperty);
                
                GUILayout.Space(10);
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif