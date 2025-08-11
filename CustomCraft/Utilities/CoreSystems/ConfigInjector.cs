using System.Collections.Generic;
using CoreUtility.Extensions;
using Object = System.Object;
using System.Reflection;
using UnityEngine;
using CoreUtility;
using System.Linq;
using System;

#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

namespace CoreSystems {
    static class ConfigInjector {
        static readonly string _name = "Config";
        
#if UNITY_EDITOR
        [DidReloadScripts]
        static void CreateConfig() => PersistentFactory.CreateConfig<ConfigInjectorData>("InjectorConfig");
#endif
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Inject() {
            var assembliesName = new[] {
                "CoreUtility",
                "SceneLoader",
                "CharControl2D",
                "Ability",
                "Inflowis",
                "CoreSystems",
                "Game"
            };
            
            var assemblies = new List<Assembly>();
            foreach (var assemblyName in assembliesName) {
                Assembly assembly = null;

                try
                {
                    assembly = Assembly.Load(assemblyName);
                }
                catch (Exception)
                {
                    continue;
                }
                
                if (assembly == null) {
#if UNITY_EDITOR
                    Debug.LogWarning("Didn't find assemblyName: " + assembliesName + 
                                     "/ if you think its good change in ConfigHandler available assemblies");
#endif
                    continue;
                }
                
                assemblies.Add(assembly);
            }
            
            var resources = Resources.LoadAll("");
            var configs = new List<Object>();
            foreach (var resource in resources) {
                if(resource is not ScriptableObject config) 
                    continue;
                
                if (!config.name.EndsWith(_name)) 
                    continue;
                
                configs.Add(config);
            }
            
            var assemblyTypes = assemblies.
                SelectMany(a => a.GetTypes());

            foreach (var type in assemblyTypes) {
                var fields = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public).
                    Where(field => field.FieldType.Name.EndsWith(_name)).
                    Where(field => field.IsStatic).
                    Where(field => field.FieldType.Inherits(typeof(ScriptableObject)));
                
                foreach (var field in fields)
                foreach (var config in configs.Where(config => config.GetType().Name == field.FieldType.Name)) 
                    field.SetValue(type, config);
            }
        }
    }
}