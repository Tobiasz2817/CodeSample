using UnityEngine;

namespace ModuleSystem {
    public abstract class ModuleMono : MonoBehaviour, IModule {
        [Inject] public int Id { set; get; }
        
        public virtual void OnEntry() { }
        public virtual void OnExit() { }
        public virtual void OnTick() { }
        public virtual void OnFixedTick() { } 
    }
    
    public class Module : IModule {
        [Inject] public int Id { set; get; }
    
        public virtual void OnEntry() { }
        public virtual void OnExit() { }
        public virtual void OnTick() { }
        public virtual void OnFixedTick() { }
    }
}