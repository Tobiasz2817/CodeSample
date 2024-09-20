namespace ModuleSystem {
    public interface IModule : IModuleState {
        int Id { set; get; }
    }

    public interface IModuleState {
        void OnEntry() { }
        void OnExit() { }
        void OnTick() { }
        void OnFixedTick() { } 
    }

    public interface IInitialize {
        void OnInitialize();
    }
    
    public interface ICondition {
        bool Evaluate();
    }
    
    public interface ITransition {
        int ToId { get; }
        ICondition Condition { get; }
    }
    
    public interface IComponentInjection {
        UnityEngine.Object[] GetComponentInjections();
    }

    public interface IReferencer { }
    
    public interface IModuleInjection {
        void ProcessInjection(IStateManager stateManager);
    }
    
    public interface IModuleInstaller {
        void OnInstallModules(IStateManager installer);
    }
}