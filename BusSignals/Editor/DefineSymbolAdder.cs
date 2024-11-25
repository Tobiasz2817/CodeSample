#if UNITY_EDITOR
using CoreUtility;
using UnityEditor;

namespace BusSignals.Editor {
    [InitializeOnLoad]
    public class DefineSymbolAdder {
        const string SignalsSymbol = "SIGNALS";
        
        static DefineSymbolAdder() =>
            Utility.TryAddDefineSymbol(SignalsSymbol);
    }
}
#endif