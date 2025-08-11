using System.IO;
using UnityEngine;

namespace Storex {
    public static class Utility {
        internal static void TryCreateDictionary(string path) {
            if (!Directory.Exists(path)) 
                Directory.CreateDirectory(path);
        }
        
        internal static string NormalizePath(string path) => path.Replace('\\', '/');
    }
    
    public static class Constants {
        // Set as empty to create folder before Assets folder
        const string DirectoryName = "StorageData";
        const string EditorDirectoryName = "EditorStorage";
        
        public const string Save01 = "Save-01";
        internal static readonly string SubFolder = Application.persistentDataPath;
        internal static readonly string FullPath = $"{SubFolder}/{DirectoryName}";
        internal static readonly string EditorPath = $"{SubFolder}/{EditorDirectoryName}";
    }
}