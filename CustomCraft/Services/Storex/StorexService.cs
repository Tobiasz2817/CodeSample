using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using System.IO;

namespace Storex {
    public static class StorexService {
        static string SavePath => $"{Constants.FullPath}/{StorexSaves.SelectedSave}";
        static readonly string EditorPath = $"{Constants.EditorPath}";

        public static void Save<T>(T data, string fileName = null) {
            Utility.TryCreateDictionary(SavePath);
            
            string fullyJson = JsonConvert.SerializeObject(data, Formatting.Indented);

#if !UNITY_EDITOR
            fullyJson = StorexEncrypter.Encrypt(fullyJson);
#endif

            string filePath = $"{SavePath}/{fileName ?? typeof(T).Name}.json";
            File.WriteAllText(filePath, fullyJson);
        }
        
        public static T Load<T>(string fileName = null) {
            Utility.TryCreateDictionary(SavePath);

            // Load by type name
            string filePath = $"{SavePath}/{fileName ?? typeof(T).Name}.json";
            if (!File.Exists(filePath)) {
#if UNITY_EDITOR
                Debug.LogWarning($"File {fileName} not found in path: {filePath}. It returns default value.");
#endif
                return default;
            }
            
            var fullyJson = File.ReadAllText(filePath);
            
#if !UNITY_EDITOR
            fullyJson = StorexEncrypter.Decrypt(fullyJson);
#endif

            return JsonConvert.DeserializeObject<T>(fullyJson);
        }
        
        public static void EditorSave<T>(T data, string fileName = null) {
            Utility.TryCreateDictionary(EditorPath);
            
            string fullyJson = JsonConvert.SerializeObject(data, Formatting.Indented);

            string filePath = $"{EditorPath}/{fileName ?? typeof(T).Name}.json";
            File.WriteAllText(filePath, fullyJson);
        }
        
        public static T EditorLoad<T>(string fileName = null) {
            Utility.TryCreateDictionary(EditorPath);

            // Load by type name
            string filePath = $"{EditorPath}/{fileName ?? typeof(T).Name}.json";
            if (!File.Exists(filePath)) {
                Debug.LogWarning($"File {fileName} not found in path: {filePath}. It returns default value.");
                return default;
            }
            
            var fullyJson = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(fullyJson);
        }
        
        public static void EditorClear<T>(string fileName = null) {
            string filePath = $"{EditorPath}/{fileName ?? typeof(T).Name}";
            File.Delete(filePath);
        }
    }
}