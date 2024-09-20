/*using UnityEngine;

namespace ModuleSystem.State {
    public class GravityModule : Module, IInitialize {
        [Inject] Rigidbody2D _rb2D;
        [Inject] AdvanceGroundChecker2D _checker2D;
        
        [Inject] float _gravityScale;
        [Inject] float _maxFall;
        
        public void OnInitialize() {
            _checker2D.OnGrounded += () => { SetGravity(); };
        }

        public override void OnFixedTick() => ProcessGravity();
        void ProcessGravity() => _rb2D.velocity = _rb2D.velocity.With(y: Mathf.Max(_rb2D.velocity.y, -_maxFall));

        public void SetGravity(float gravityMultiply = 1) {
            _rb2D.gravityScale = _gravityScale * gravityMultiply;
        }
    }
}*/