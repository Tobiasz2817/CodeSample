#if UNITY_EDITOR
using CoreUtility;
using UnityEditor;
using UnityEditor.Build;

namespace Inflowis {
    [InitializeOnLoad]
    public class DefineSymbolAdder {
        const string Symbol = "INFLOWIS";
        
        static DefineSymbolAdder() =>
            Utility.TryAddDefineSymbol(NamedBuildTarget.Standalone, Symbol);
    }
}
#endif