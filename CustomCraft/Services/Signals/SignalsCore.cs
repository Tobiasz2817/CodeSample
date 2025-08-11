using System.Collections.Generic;
using System.Linq.Expressions;
using CoreUtility.Extensions;
using UnityEngine.Scripting;
using System.Reflection;
using System.Linq;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Signals {
    // Usage Requires:
    // - Preserve attribute on method, protect code for stripping in build for not used code
    // TODO:
    // Functionality with returned types e.g. bool IsOpenState(); 
    // Can allow only one receive event handler
    // Make int research -> fastes option
    public static class SignalsCore {
        static Dictionary<string, SignalEvent> Events;

        #region Initialize

#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void InitializeEditor() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
    
        static void OnPlayModeStateChanged(PlayModeStateChange state) {
            if (state != PlayModeStateChange.ExitingPlayMode)
                return;
            
            Events.ForEach(@event => @event.Value.Clear());                
            Events.Clear();
            Debug.Log("Clear the signals ...");
        }
#endif
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize() => Events = SignalsEvents();

        #endregion
        #region Processing

        internal static void Registry(IList<MonoBehaviour> targets) {
            foreach (var target in targets) {
                var targetType = target.GetType();

                foreach (var methodInfo in GetMethods(targetType)) 
                    RegistryMethod(target, methodInfo);
            }
        }
        
        internal static void RegistryMethod(object target, MethodInfo methodInfo) =>
            Events[methodInfo.Name].AddEvent(CreateDelegateForMethod(target, methodInfo));

        public static void RaiseSignal(string methodName, params object[] args) =>
            Events[methodName].NotifyEvent(args);

        #endregion
        
        #region Utility

        internal static IEnumerable<MethodInfo> GetMethods(Type targetType) {
            const BindingFlags Flags = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            
            return targetType.GetMethods(Flags).
                Where(method => Attribute.IsDefined(method, typeof(PreserveAttribute))).
                Where(method => (Events ?? SignalsEvents()).ContainsKey(method.Name)).
                Where(method => method.EqualParameters(GetISignalMethod(method.Name), true));
        }
        
        static Dictionary<string, SignalEvent> SignalsEvents() =>
            typeof(ISignals).GetMethods()
                .ToDictionary(method => method.Name, _ => new SignalEvent());
        
        static MethodInfo GetISignalMethod(string methodName) =>
            typeof(ISignals).GetMethods().FirstOrDefault((m) => m.Name.Equals(methodName));
        
        static Delegate CreateDelegateForMethod(object target, MethodInfo methodInfo) {
            var parameters = methodInfo.GetParameters();
            Type delegateType = methodInfo.ReturnType == typeof(void)
                ? Expression.GetActionType(parameters.Select(p => p.ParameterType).ToArray())
                : Expression.GetFuncType(parameters.Select(p => p.ParameterType).ToArray().Add(methodInfo.ReturnType));

            return Delegate.CreateDelegate(delegateType, target, methodInfo);
        }
        
        #endregion
    }
}