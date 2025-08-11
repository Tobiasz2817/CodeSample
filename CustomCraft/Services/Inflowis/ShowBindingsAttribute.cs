#if UNITY_EDITOR

using UnityEngine.InputSystem;
using System.Reflection;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

namespace Inflowis {
    [AttributeUsage(AttributeTargets.Field)]
    public class ShowBindingsAttribute : PropertyAttribute {
        internal string ActionName;
        
        public ShowBindingsAttribute(string nameOfAction) {
            ActionName = nameOfAction;
        }
    }

    [CustomPropertyDrawer(typeof(ShowBindingsAttribute))]
    public class ShowBindingsPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var target = (ShowBindingsAttribute)attribute;
            
            if (property.propertyType != SerializedPropertyType.Integer) {
                EditorGUI.LabelField(position, "ERROR:", "Type must be a int");
                return;
            }
            
            var inputAsset = property.serializedObject.targetObject?.GetType().
                GetFields(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic | BindingFlags.Public).
                FirstOrDefault((f) => f.Name == target.ActionName);
            
            if (inputAsset == null) {
                EditorGUI.LabelField(position, "ERROR:", "Didn't find variable with passed name");
                return;
            }
            
            var actionReference = inputAsset.GetValue(property.serializedObject.targetObject) as InputActionReference;
            if (!actionReference) {
                EditorGUI.LabelField(position, "ERROR:", "Value type from passed name didn't compare to needed type (InputActionReference)");
                return;
            }
            
            var bindings = actionReference.action.bindings;
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

            int bindingIndex = property.intValue;
            property.intValue = EditorGUILayout.Popup("Binding Index", bindingIndex, namesList);
        }    
    }
}

#endif