#if UNITY_EDITOR
using Storex;

namespace LScene.Editor
{
    [System.Serializable]
    public struct EditorLoaderData
    {
        internal const string EditorLoaderDataKey = "EDITOR_LOADER_DATA";

        public string[] ScenesToLoadPath;
        public string InitScenePath;
        public bool CustomLoading;
        public bool NetworkLoading;
        public bool AutomaticConnection;
        
        internal void SaveData() => StorexService.EditorSave(this, EditorLoaderDataKey);
        internal static void SaveData(EditorLoaderData data) => StorexService.EditorSave(data, EditorLoaderDataKey);
        internal static EditorLoaderData LoadData() => StorexService.EditorLoad<EditorLoaderData>(EditorLoaderDataKey);
    }
}
#endif