using System.Collections.Generic;
using CoreUtility.Extensions;
using DICore;
using UnityEngine;

namespace ModuleSystem {
    [RequireComponent(typeof(ModuleController))]
    public class ModuleDataInjection : MonoBehaviour, IModuleInjection {
        public bool componentInjection = true;
        public bool dataInjection = true;
        public bool modulesInjection = true;
        
        public ScriptableObject dataSource;
        
        public void ProcessInjection(IStateManager stateManager) {
            var targets = stateManager.GetModules();
            if(dataInjection) DataInject(targets);
            if(componentInjection) ComponentInject(targets);
            if(modulesInjection) ModulesInjection(targets);
        }

        void DataInject(List<IModule> modules) {
            DI.CreateData<IModule, ScriptableObject>().
                WithIAttribute(typeof(Inject)).
                WithInjections(modules).
                WithProviders(new []{dataSource}).
                WithCondition((module, data) => 
                    module.Item2.Name.ToLower().ToNonAlNum() ==
                    data.Item2.Name.ToLower().ToNonAlNum()).
                Inject();
        }

        void ComponentInject(List<IModule> modules) {
            var providers = new List<UnityEngine.Object>() {
                GetComponent<Transform>(),
                GetComponent<GameObject>(),
                dataSource
            };
            providers.AddRange(transform.root.GetComponents<Component>());
            providers.AddRange(transform.root.GetComponentsInChildren<Component>());
            
            ReferOutSideInjections(ref providers);
            
            DI.CreateData<IModule, UnityEngine.Object>().
                WithIAttribute(typeof(Inject)).
                WithPAsValue(true).
                WithInjections(modules).
                WithProviders(providers).
                Inject();
        }

        void ModulesInjection(List<IModule> modules) {
            DI.CreateData<IModule, IModule>().
                WithIAttribute(typeof(Inject)).
                WithPAsValue(true).
                WithInjections(modules).
                WithProviders(modules).
                WithCondition((iModule, pModule) => iModule.GetType().Name.ToLower().ToNonAlNum() == pModule.GetType().Name.ToLower().ToNonAlNum()).
                Inject();
        }

        void ReferOutSideInjections(ref List<UnityEngine.Object> providers) {
            var outsideInjections = GetComponents<IComponentInjection>();
            foreach (var injection in outsideInjections) {
                var injections = injection.GetComponentInjections();
                if(injections is not { Length: > 0 }) continue;
                
                providers.AddRange(injections);
            }
        }
    }
}