/*using UnityEngine;

namespace ModuleSystem.State {
    public class MoveModule : Module, IInitialize {
        [Inject] Rigidbody2D _rb2D;
        [Inject] AnimCore _animCore;
        [Inject] AdvanceGroundChecker2D _checker2D;
        [Inject] InputActionReference _movementInput;
        
        [Inject] float _moveMaxSpeed;
        [Inject] float _moveDecel;
        [Inject] float _moveAccel;
        [Inject] float _moveAirAcceleration;
        [Inject] float _moveAirDeceleration;

        float _moveT = 1f;
        float _idleDelay = 0.05f;
        float _idleCurrent = 0.05f;
        int _lastX = 0;
        bool _isInAir = false;
        Vector3 _dir;
        
        readonly AnimType _idleAnim = AnimType.Idle;
        readonly AnimType _moveAnim = AnimType.Move;

        public void OnInitialize() {
            _movementInput.action.Enable();

            if (_checker2D) {
                _checker2D.OnGrounded += () => { _isInAir = false; };
                _checker2D.OnUnGrounded += () => { _isInAir = true; };
            }
        }
        
        public override void OnEntry() {
            var x = Mathf.Abs((int)_movementInput.action.ReadValue<Vector3>().x);
            _animCore.Play(x > 0 ? (int)_moveAnim : (int)_idleAnim,true, looping: true);
        }

        public override void OnTick() {
            _dir = _movementInput.action.ReadValue<Vector3>();
            HandleAnimation(_dir);
            HandleFlip(_dir);
        }
        
        public override void OnFixedTick() => Move(_dir.x, _isInAir);
        
        void Move(float dir, bool isInAir) {
            var targetSpeed = dir * _moveMaxSpeed;
            targetSpeed = Mathf.Lerp(_rb2D.velocity.x, targetSpeed, _moveT);

            var accelRate = Mathf.Abs(targetSpeed) > 0.01f ? _moveAccel : _moveDecel;
            accelRate *= isInAir ? Mathf.Abs(targetSpeed) > 0.01f ? _moveAirAcceleration : _moveAirDeceleration : 1f;
            
            var speedDif = targetSpeed - _rb2D.velocity.x;
            var moveSpeed = speedDif * accelRate;
            
            _rb2D.AddForce(moveSpeed * Vector2.right, ForceMode2D.Force);
        }
        
        void HandleAnimation(Vector3 dir) {
            var x = dir.x;
            if (Mathf.Approximately(x, _lastX)) return;
            
            
            
            _animCore.Play(x > 0 ? (int)_moveAnim : (int)_idleAnim);
        }
        
        void HandleFlip(Vector3 dir) {
            var flipValue = dir.x > 0 ? 1 : -1;
            if (dir.x == 0 || IsFlipped(flipValue)) return;

            _rb2D.transform.localScale = _rb2D.transform.localScale.With(x: flipValue);
        }
        
        bool IsFlipped(float dir) => Mathf.Approximately(_rb2D.transform.localScale.x, dir);
        
    }
}*/