#if UNITY_EDITOR

using System.Linq;
using UnityEditor.Compilation;
using UnityEditor;

namespace Inflowis {
    public class RecompileOnAssetSave : AssetPostprocessor {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            if (importedAssets.Any(asset => asset.EndsWith(".inputactions"))) {
                CompilationPipeline.RequestScriptCompilation();
            }
        }
    }
}

#endif