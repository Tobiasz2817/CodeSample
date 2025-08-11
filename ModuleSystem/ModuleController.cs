using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CoreUtility.Extensions;
using DICore;
using Sirenix.Utilities;
using UnityEngine;

namespace ModuleSystem {
    [DefaultExecutionOrder(-500)]
    [RequireComponent(typeof(ModuleBlackboard))]
    public class ModuleController : MonoBehaviour, IStateManager {
        public bool readModules;
        public bool displayDebug;

        readonly List<IModule> _modules = new();
        readonly Dictionary<int, HashSet<ITransition>> _transitions = new();
        readonly Dictionary<int, HashSet<int>> _expansionStates = new();

        readonly HashSet<ITransition> _anyTransitions = new();

        // TODO: Make internal, debuger issue
        public IModule Current;
        public IModule PreviousState;
        public IModule DefaultState;


        protected virtual void Awake() {
            ModuleInstaller();
            ModuleReader();
            ModuleInjection();
            ModuleReferencing();
            ModuleInitializer();
            DefaultStateInitializer();
            SetState(DefaultState);
        }

        public void Update() {
            var transition = GetTransition();
            if (transition != null)
                ChangeState(transition.ToId);

            Current?.OnTick();
            ProcessExpandState((module) => module?.OnTick());
        }

        public void FixedUpdate() {
            Current?.OnFixedTick();
            ProcessExpandState((module) => module?.OnFixedTick());
        }

        public void SetState(IModule state) {
            Current = state;
            Current?.OnEntry();
        }

        public void SetDefaultState(IModule state) {
            DefaultState = state;
        }

        void ChangeState(int newId) {
            var previousModule = Current;
            var nextModule = _modules[newId];

            previousModule?.OnExit();
            //ProcessExpandState((module) => module.OnExit());

            Current = nextModule;
            PreviousState = previousModule;
            
            SetState(Current);
            //ProcessExpandState((module) => module.OnEntry());
        }

        void ProcessExpandState(Action<IModule> action) {
            if (Current == null || !_expansionStates.TryGetValue(Current.Id, out var ids)) return;
            foreach (var id in ids) 
                action?.Invoke(_modules[id]);
        }


        ITransition GetTransition() {
            var any = _anyTransitions?.FirstOrDefault((value) => value.Condition.Evaluate());
            if (any != null) return any;
            
            return Current != null && _transitions.TryGetValue(Current.Id, out var transitions)
                ? transitions.Where((value) => value.Condition.Evaluate()).Select((trans) => trans).FirstOrDefault()
                : null;
        }

        public void AddModule(IModule module) {
            InjectId(module, _modules.Count);
            _modules.Add(module);
        }

        public void RemoveModule(IModule module) => _modules.Remove(module);

        public void AddTransition(IModule from, IModule to, ICondition condition) {
            if (!GetId(from, out var fromId)) return;
            if (!GetId(to, out var toId)) return;

            _transitions.TryAdd(fromId, new HashSet<ITransition>());
            _transitions[fromId].Add(new Transition(toId, condition));
        }

        public void AddAnyTransition(IModule to, ICondition condition) {
            if (!GetId(to, out var toId)) return;

            _anyTransitions.Add(new Transition(toId, condition));
        }

        public void AddExpansion(IModule target, params IModule[] expansionStates) {
            if (!GetId(target, out var id)) return;

            _expansionStates.TryAdd(id, new HashSet<int>());
            _expansionStates[id].AddRange(expansionStates.Select((state) => state.Id));
        }
        


        #region Initialize
        void ModuleInstaller() {
            var installers = GetComponents<IModuleInstaller>();
            foreach (var installer in installers)
                installer.OnInstallModules(this);
        }
        
        void ModuleReader() {
            if (!readModules) return;
            
            foreach (var state in GetComponents<IModule>()) {
                AddModule(state);
            }
        }
        
        void ModuleInjection() {
            var injectors = GetComponents<IModuleInjection>();
            foreach (var injector in injectors)
                injector.ProcessInjection(this);
        }
        
        void ModuleReferencing() {
            var referencer = GetComponents<IReferencer>();
            if (referencer == null) return;
            
            DI.CreateData<IModule, IReferencer>().
                WithProviders(referencer).
                WithPFlag(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).
                WithIAttribute(typeof(Inject)).
                WithInjections(_modules).
                WithCondition((m, r) => m.Item2.Name.ToLower().ToNonAlNum() == r.Item2.Name.ToLower().ToNonAlNum()).
                Inject();
        }
        
        void ModuleInitializer() {
            foreach (var module in _modules.Where(module => module.GetType().Inherits(typeof(IInitialize))))
                ((IInitialize)module).OnInitialize();
        }
        
        void InjectId(IModule module, int id) {
            module.Id = id;
            
            //TODO: Injection
            // Remember change bindings flags to research also base classes
        }
        
        protected virtual void DefaultStateInitializer() {
            if (DefaultState != null) return;            
            DefaultState = _modules.Count > 0 ? _modules[0] : new EmptyState();
        }

        #endregion


        #region Collections Callback
        public IModule GetModule(int id) => _modules[id];
        public int? GetId(IModule module) => _modules.GetIndex(module);
        public IEnumerable<IModule> GetStates() => _modules;
        public List<IModule> GetModules() => _modules;


        public bool GetId(IModule module, out int id) {
            id = _modules.GetIndex(module);
            return id != -1;
        }
        
        public T GetModule<T>() where T : IModule {
            foreach (var module in _modules) {
                if (module.GetType() == typeof(T)) {
                    return (T)module;
                }
            }

            return default;
        }
        
        public bool IsModuleUsageAsExtension(IModule module) => 
            Current != null && 
            _expansionStates.TryGetValue(Current.Id, out var exp) && exp.Any((id) => id == module.Id);

        #endregion
    }
}