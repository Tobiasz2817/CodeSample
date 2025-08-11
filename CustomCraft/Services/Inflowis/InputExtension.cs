using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inflowis {
    public static class InputExtension {
        public static string GetBind(this InputActionReference actionReference) {
            if (actionReference == null || actionReference.action == null) {
#if UNITY_EDITOR
                Debug.LogWarning("InputActionReference is null or does not reference a valid action.");
#endif
                return string.Empty;
            }

            return GetBind(actionReference.action);
        }
        
        public static string GetBind(this InputAction inputAction) {
            foreach (var binding in inputAction.bindings)
            {
                if (!binding.isComposite)
                {
                    var control = inputAction.controls.FirstOrDefault();
                    if (control == null) 
                        continue;

                    return control.shortDisplayName ?? control.displayName;
                }
                
                // Is Composite
                var compositeControls = inputAction.controls.
                    Select(control => control.shortDisplayName ?? control.displayName).
                    ToList();

                if (compositeControls.Count <= 0) 
                    continue;
                
                return string.Join("+", compositeControls);
            }

            return string.Empty;
        }

        public static string GetBind(string actionName, string groups, MapType mapType = MapType.Gameplay) {
            var asset = InflowisCore.Config.InputActionAsset;
            
            var map = asset.actionMaps[(int)mapType];

            foreach (var action in map) {
                if (action.name != actionName) 
                    continue;
                
                foreach (var binding in action.bindings)
                    if (!binding.name.Contains("modifier") && binding.groups.Contains(groups)) {
                        var parts = binding.effectivePath.Split('/');
                        
                        return string.Join(" ", parts.Skip(1));
                    }
            }
            
            return string.Empty;
        }
        
        public static InputActionMap GetOrAddMap(this InputActionAsset asset, string name) {
            return asset.actionMaps.FirstOrDefault(map => map.name.Equals(name)) ?? 
                   asset.AddActionMap(name);
        }
    }
}