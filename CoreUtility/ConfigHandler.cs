using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CoreUtility {
    public static class ConfigHandler {
        public static T CreateConfig<T>(string configKey, ref ConfigData configData) where T : ScriptableObject {
            EditorPrefs.DeleteKey(configKey);
            
            if (!Directory.Exists(configData.CurrentConfigPath))
                Directory.CreateDirectory(configData.CurrentConfigPath);
            
            T asset = ScriptableObject.CreateInstance<T>();

            var fullPath = Path.Combine(configData.CurrentConfigPath, configData.CurrentConfigName + ".asset");
            AssetDatabase.CreateAsset(asset, fullPath);
            EditorPrefs.SetString(configKey, fullPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            configData.SaveName();
            configData.SavePath();
            return asset;
        }
        
        public static T LoadConfig<T>(string configKey) where T : ScriptableObject {
            var path = EditorPrefs.GetString(configKey, null);
            return !string.IsNullOrEmpty(path) ? AssetDatabase.LoadAssetAtPath<T>(path) : null;
        }

        public static bool MoveConfig(string configKey, ref ConfigData configData) {
            var path = EditorPrefs.GetString(configKey, null);
            
            if (string.IsNullOrEmpty(path)) {
#if UNITY_EDITOR
                Debug.LogWarning("No config found or the config path is not set.");      
#endif
                return false;
            }

            if (configData.CurrentConfigPath == configData.ConfigPath) {
#if UNITY_EDITOR
                Debug.Log("Config is already in the correct location.");
#endif
                return false;
            }

            var newPath = Path.Combine(configData.ConfigPath, configData.ConfigName + ".asset");
            var moveResult = AssetDatabase.MoveAsset(path, newPath);
            var successResult = string.IsNullOrEmpty(moveResult);

            if (!successResult) {
#if UNITY_EDITOR
                Debug.LogWarning("Failed to move config: " + moveResult);
#endif
                return false;
            }
            
            configData.SavePath();
            EditorPrefs.SetString(configKey, newPath);
#if UNITY_EDITOR
            Debug.Log("Config moved successfully.");
#endif
            return true;
        }
        
        
        public static bool ChangeConfigName(string configKey, ref ConfigData configData) {
            var path = EditorPrefs.GetString(configKey, null);
            
            if (string.IsNullOrEmpty(path)) {
#if UNITY_EDITOR
                Debug.LogWarning("No config found");
#endif      
                return false;
            }

            if (configData.CurrentConfigName == configData.ConfigName) {
#if UNITY_EDITOR
                Debug.Log("Config new and old have the same names");
#endif
                return false;
            }

            var newConfigName = path.Replace(configData.CurrentConfigName, configData.ConfigName);
            var result = AssetDatabase.RenameAsset(path, configData.ConfigName);
            
            if (!string.IsNullOrEmpty(result)) {
#if UNITY_EDITOR
                Debug.LogWarning("Failed to rename config: " + result);
#endif
                return false;
            }
            
            configData.SaveName();
            EditorPrefs.SetString(configKey, newConfigName);
#if UNITY_EDITOR
            Debug.Log("Config rename successfully.");
#endif
            return true;
        }
    }
    
    [Serializable]
    public class ConfigData {
        [SerializeField] 
        internal string ConfigPath = "Assets/Resources";
        internal string CurrentConfigPath;
        
        [SerializeField] 
        internal string ConfigName = "Config";
        internal string CurrentConfigName;
        
        public ConfigData(string configPath = "Assets/Resources", string configName = "Config") {
            ConfigPath = configPath;
            ConfigName = configName;
            
            SaveName();
            SavePath();
        }
        
        internal void SavePath() => CurrentConfigPath = ConfigPath;
        internal void SaveName() => CurrentConfigName = ConfigName;
        internal bool IsProperPath() => ConfigPath == CurrentConfigPath;
        internal bool IsProperName() => ConfigName == CurrentConfigName;
    }
}