#if UNITY_EDITOR
using Storex;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace LScene.Editor
{
    public class ClearPlayerPrefsOnBuild : IPreprocessBuildWithReport {
        public int callbackOrder => 0;
    
        public void OnPreprocessBuild(BuildReport report) { StorexService.EditorClear<EditorLoaderData>(EditorLoaderData.EditorLoaderDataKey); }
    }
}
#endif