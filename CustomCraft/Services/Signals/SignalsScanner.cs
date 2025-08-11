using System.Collections.Generic;
using System.Linq;
using CoreUtility;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Signals {
    [DefaultExecutionOrder(-1000)]
    public class SignalsScanner : MonoBehaviour {
        [SerializeField] internal List<MonoBehaviour> Methods = new();

        void Awake() {
            // Add feature of tmp allocation method reference (MethodInfo is not serialized)
            SignalsCore.Registry(Methods);
            Destroy(gameObject);
        }

#if UNITY_EDITOR
        
        internal void Scanning(bool sceneScanning = true) {
            Methods.Clear();

            var targets = sceneScanning
                ? FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(mono =>
                    Utility.IsUserScript(mono) && !PrefabUtility.IsPartOfPrefabInstance(mono.transform.root.gameObject))
                : transform.root.GetComponentsInChildren<MonoBehaviour>().
                    Where(mono => Utility.IsUserScript(mono) && !PrefabUtility.IsPartOfPrefabInstance(mono.transform.root.gameObject));;
            
            foreach (var target in targets) {
                var methods = SignalsCore.GetMethods(target.GetType()).ToArray();

                if (methods.Length <= 0) 
                    continue;
                
                if (!Methods.Contains(target)) 
                    Methods.Add(target);
            }
        }
        
#endif
    }
}