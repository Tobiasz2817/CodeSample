using System.Collections.Generic;

namespace ModuleSystem {
    public interface IStateManager {
        // TODO: Resize about methods
        public void SetDefaultState(IModule state);
        public void AddModule(IModule module);
        public void RemoveModule(IModule module);
        public void AddTransition(IModule from, IModule to, ICondition condition);
        public void AddAnyTransition(IModule to, ICondition condition);
        public void AddExpansion(IModule target, params IModule[] expansionStates);
        public List<IModule> GetModules();
        public T GetModule<T>() where T: IModule;
    }
}