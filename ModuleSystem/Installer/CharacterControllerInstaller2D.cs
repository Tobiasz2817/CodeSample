/*using ModuleSystem.State;
using UnityEngine;

namespace ModuleSystem.Installer {
    public class CharacterControllerInstaller2D : MonoBehaviour, IModuleInstaller {
        [SerializeField] Rigidbody2D _rb2D;
        [SerializeField] AdvanceGroundChecker2D _checker2D;
        
        public void OnInstallModules(IStateManager installer) {
            var gravityModule = new GravityModule();
            var jumpModule = new JumpModule();
            var moveModule = new MoveModule();
            var fallModule = new FallModule();
            
            installer.AddModule(gravityModule);

            installer.AddModule(moveModule);
            installer.AddModule(jumpModule);
            installer.AddModule(fallModule);
            
            installer.SetDefaultState(moveModule);
            
            installer.AddTransition(moveModule, jumpModule, new Predicate(() => jumpModule.CanJump));
            installer.AddTransition(moveModule, fallModule, new Predicate(() => _rb2D.velocity.y < 0));
            installer.AddTransition(jumpModule, jumpModule, new Predicate(() => jumpModule.CanJump));
            installer.AddTransition(jumpModule, fallModule, new Predicate(() => _rb2D.velocity.y < 1));
            installer.AddTransition(fallModule, jumpModule, new Predicate(() => (_checker2D.IsGrounded() && jumpModule.WasJumpBuffered) || jumpModule.CanJump));
            installer.AddTransition(fallModule, moveModule, new Predicate(() => _checker2D.IsGrounded()));
            
            installer.AddExpansion(moveModule, gravityModule);
            installer.AddExpansion(jumpModule, moveModule);
            installer.AddExpansion(jumpModule, gravityModule);
            installer.AddExpansion(fallModule, moveModule);
            installer.AddExpansion(fallModule, gravityModule);
        }
    }
}*/