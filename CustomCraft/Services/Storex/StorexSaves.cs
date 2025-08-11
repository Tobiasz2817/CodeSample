using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using System.IO;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Storex {
    public static class StorexSaves {
        internal static HashSet<string> Saves => _saves;
        internal static string SelectedSave => _save;
        
        static HashSet<string> _saves = new();
        static string _save = "Save_01";
        
        internal const int SavesCount  = 5;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void LoadSaves() {
            Utility.TryCreateDictionary(Constants.FullPath);
            
            var saves = Directory
                .GetDirectories(Constants.FullPath)
                .OrderByDescending(Directory.GetLastWriteTime)
                .ToArray();

            foreach (var save in saves)
                _saves.Add(Path.GetFileName(Utility.NormalizePath(save)));
        }
        
        
#if UNITY_EDITOR
        
        [MenuItem("Tools/SaveSystem/Clear Saves")]
        public static void ClearStorageFullSaves() => ClearPath(Constants.FullPath);
        
        [MenuItem("Tools/SaveSystem/Open Saves File")]
        public static void OpenSaveFile() => System.Diagnostics.Process.Start(Constants.SubFolder);
        
#endif
                
        public static void ClearCurrentSave() {
            Assert.IsNull(_save, "You can't clear save without first set the save");
            
            ClearPath($"{Constants.FullPath}/{_save}");
        }
        
        public static void ClearSave(string saveName) {
            bool findSave = false;
            foreach (var save in _saves) {
                if (save != saveName) 
                    continue;

                ClearPath($"{Constants.FullPath}/{save}");
                findSave = true;
            }

            if (!findSave) {
#if UNITY_EDITOR
                Debug.LogWarning("No found save");
#endif
            }
        }
        
        public static void SetSave(string saveName) => _save = saveName;
        
        /// <summary>
        /// It's create save folder, when folder exist will delete him 
        /// </summary>
        /// <param name="saveName"> name save, when is null taking current set save</param>
        public static void CreateSave(string saveName = null) { 
            string save = saveName ?? _save;

            string savePath = $"{Constants.FullPath}/{save}";
            if (Directory.Exists(savePath)) 
                ClearPath(savePath);
            else if (_saves.Count + 1 > SavesCount) 
                ClearPath($"{Constants.FullPath}/{_saves.Last()}");

            Directory.CreateDirectory(savePath);
            LoadSaves();
        }

        static void ClearPath(string fullPath) {
            Directory.Delete(fullPath, true);
            File.Delete(fullPath + ".meta");
        }
    }
}