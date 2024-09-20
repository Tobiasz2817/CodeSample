/*using UnityEngine;

namespace ModuleSystem.State {
    public class FallModule : Module {
        [Inject] AnimCore _animCore;
        [Inject] Rigidbody2D _rb2D;

        readonly AnimType _fallAnim = AnimType.Fall;

        public override void OnEntry() => _animCore.Play((int)_fallAnim, true, duration: 0f);
    }
}*/