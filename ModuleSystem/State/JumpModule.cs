/*using System.Collections;
using UnityEngine;

namespace ModuleSystem.State {
    public class JumpModule : Module, IInitialize {
        [Inject] Rigidbody2D _rb2D;
        [Inject] AnimCore _animCore;
        [Inject] GravityModule _gravityModule;
        [Inject] InputActionReference _jumpInput;
        [Inject] ModuleCoroutine _moduleCoroutine;
        [Inject] AdvanceGroundChecker2D _checker2D;
        
        InputBuffer _jumpBuffer;
        
        [Inject] float _jumpHeight;
        [Inject] float _jumpForce;
        [Inject] int _jumpCount;
        [Inject] float _jumpTimeToApex;
        [Inject] float _jumpFall;
        [Inject] float _jumpCutFall;
        [Inject] float _jumpCutMinTime;
        [Inject] float _jumpCoyoteTime;
        [Inject] float _jumpInputBuffer;
        
        int _currentJumpCount;
        float _currentCoyoteTime;
        
        readonly AnimType _jumpAnim = AnimType.Jump;

        public bool CanJump => _jumpInput.action.triggered && ((_currentCoyoteTime > 0f && _currentJumpCount == 0) || (_currentJumpCount < _jumpCount && _currentJumpCount != 0));
        public bool WasJumpBuffered => _jumpBuffer.WasBuffered;

        public void OnInitialize() {
            _jumpInput.action.Enable();
            _jumpInput.action.performed += (_) => { _jumpBuffer.RefreshBufferTime(); };
            _jumpBuffer = new InputBuffer(_jumpInputBuffer);

            _checker2D.OnGrounded += OnGrounded;
            _checker2D.OnUnGrounded += OnUnGrounded;

            _currentCoyoteTime = _jumpCoyoteTime;
        }

        ~JumpModule() {
            _checker2D.OnGrounded -= OnGrounded;
            _checker2D.OnUnGrounded -= OnUnGrounded;
        }

        void OnGrounded() {
            _currentJumpCount = 0;
            _currentCoyoteTime = _jumpCoyoteTime;
        }

        void OnUnGrounded() {
            _moduleCoroutine.StartCoroutine(TickCoyoteTime());
        }

        public override void OnEntry() {
            if(_animCore != null)
                _animCore.Play((int)_jumpAnim,true);
            
            HandleJump(); 
        }
        
        public override void OnExit() {
            _gravityModule.SetGravity(!_jumpInput.action.triggered ? _jumpCutFall : _jumpFall);
        }

        void HandleJump() {
            _gravityModule.SetGravity();
            
            _currentJumpCount++;
            
            _rb2D.velocity = new Vector2(_rb2D.velocity.x, 0f);
            
            _rb2D.AddForce(_jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
        
        IEnumerator TickCoyoteTime() {
            while (_currentCoyoteTime > 0 && !_checker2D.IsGrounded()) {
                _currentCoyoteTime = Mathf.Clamp(_currentCoyoteTime -= Time.deltaTime, 0f, float.MaxValue);
                yield return null;
            }
        }
    }
}*/