using UnityEngine.Assertions;
using CoreUtility.Extensions;
using System.Collections;
using AnimationView;
using UnityEngine;
using CoreUtility;
using DG.Tweening;

namespace CharControl2D {
    public class CharController {
        Rigidbody2D _rb2D;
        Transform _modelTransform;
        Coroutine _calculateSmoothDir;
        
        readonly CharView _view;
        readonly CharModel _model;
        readonly RequestSystem _request;

        CharControllerData Data => _model.CtrData;

        float _elapsed;

#region Requests

        RequestData _jump;
        RequestData _cutJump;
        RequestData _flying;
        RequestData _dash;
        RequestData _roll;

#endregion

        #region Initialize / Ticks          
        
        internal CharController(CharView view, CharModel model) {
            _view = view;
            _model = model;
            
            _request = new RequestSystem();
            
            // Locomotion State
            _request.OnEmptyTick += () => {
                if (!_model.CanMove) return;
                
                Flip();
                //CalculateSmoothDir();
                // Start move
            };
            
            _request.OnEmptyFixedTick += () => {
                if (!_model.CanMove) return;
                if (_view.GetLocomotion() > 0.5f) return;

                // movement 
            };

            InitializeStates();
        }
        
        // TODO: Make smooth
        void CalculateDir() => _view.UpdateLocomotion(_model.LocomotionDir);
        public void CalculateSmoothDir(float dir) {
            float currentLocomotion = _view.GetLocomotion();
            float targetLocomotion = _model.LocomotionDir;
            
            
            if (currentLocomotion > 0 && Mathf.Approximately(targetLocomotion, -1) ||
                currentLocomotion < 0 && Mathf.Approximately(targetLocomotion, 1)) {
                {
                    Debug.Log("RESET TIME");
                    _model.SmoothedDirX = 0f;
                    _elapsed = 0f;
                }
            }
            else {
                // Smooth calculate
                float t = Mathf.Clamp01(_elapsed / Data.InputSmoothTime);
                float easedT = MEasing.Evaluate(MEase.OutSine, t);

                float delta = _model.LocomotionDir - _model.SmoothedDirX;
                _model.SmoothedDirX += delta * easedT;
                _elapsed += Time.deltaTime;
            }
            
            _view.UpdateLocomotion(_model.SmoothedDirX);
        }

        internal void InitializeReferences(Rigidbody2D rb2D, Transform modelTransform) {
            _rb2D = rb2D;
            _rb2D.gravityScale = _model.GravityScale;
            
            modelTransform ??= rb2D.transform.root.Find("Model");
            modelTransform ??= rb2D.transform.root.GetChild(0).transform;
            
            _modelTransform = modelTransform;
        }

        // TODO: Make system sync values to system without lambda
        void InitializeStates() {
            _flying = RequestBuilder.
                WithCondition(() => !_model.IsGrounded).
                WithRequestTime(0.1f).
                WithSuccess(_view.ViewFalling).
                WithUntilCondition(() => !_model.IsGrounded).
                WithFixedTick(_ => {
                    float factor = _model.IsJumping ? Data.Movement.FlyingAcceleration : Data.Movement.FallingAcceleration;
                    
                    // TODO: It will be never happened because it will interrupt when grounded state ich change and on grounded will set gravity scale to GravityScale 
                    // Maybe request system making unnecessary request on Fixed tick twice at the end
                    _rb2D.gravityScale = !_model.IsGrounded ? _model.IsJumping ? _model.GravityScale : _model.GravityFall : _model.GravityScale;
                    
                    TickMovement(factor, Data.Movement.MaxSpeed);
                    
                    Flip();
                }).
                Build();
            
            //BUG: cut jump on landing broking the jump buffer, only when we holding space while we landing we can make jump based on jump input buffer
            //BUG 2: animation didn't overlap it's playing all time flying when we jump again. FIX IT Make in jump again animation jump again
            _jump = RequestBuilder.
                WithCondition(() => _model.CanJump).
                WithRequestTime(() => Data.Jump.InputBuffer).
                WithSuccess(() => {
                    Jump();
                    _view.ViewJump();
                    
                    // Timer for reset progress time on cut jump
                    StaticTimer.RunCountdown(Data.Jump.TimeToApex,
                        onTick: progress => { _model.JumpProgressTime = 1 - progress; },
                        onComplete: _ => { _model.JumpProgressTime = 0f; },
                        condition: () => _model.IsJumpCut);
                }).
                Build();
            
            _cutJump = RequestBuilder.
                WithCondition(() => _model.CutJumpCondition && _model.IsJumping).
                WithRequestTime(() => Data.Jump.TimeToApex).
                WithSuccess(CutJump).
                WithUntilCondition(() => !_model.IsGrounded).
                WithFixedTick(_ => {
                    TickMovement(Data.Movement.FallingAcceleration, Data.Movement.MaxSpeed);
                    
                    Flip();
                }).
                Build();

            _dash = RequestBuilder.
                WithCondition(() => _model.CanDash).
                WithRequestTime(() => Data.Dash.InputBuffer).
                WithSuccess(() => {
                    ProcessDashCooldown();
                    PrepareDash();
                }).
                WithTickTime(() => Data.Dash.Duration).
                WithEnd(StopDash).
                WithFixedTick(Dash).
                Build();
            
            _roll = RequestBuilder.
                WithCondition(() => _model.CanRoll).
                WithRequestTime(() => Data.Roll.InputBuffer).
                WithSuccess(Roll).
                Build();
        }
        
        internal void Tick() {
            if(!_model.IsColliding && _model.JumpCoyoteTime > 0)
                _model.JumpCoyoteTime = Mathf.Clamp(_model.JumpCoyoteTime - Time.deltaTime, 0f, float.MaxValue);
            
            if (_model.TimeToRoll > 0)
                _model.TimeToRoll -= Time.deltaTime;
            
            _model.IsJumping = _rb2D.linearVelocity.y > 0f;
            _model.IsFalling = _rb2D.linearVelocity.y < 0f;
            
            _request.Tick();

            var yMaxVelocity = _rb2D.linearVelocity.y < -Data.GravityMaxFall ? -Data.GravityMaxFall : _rb2D.linearVelocity.y;
            _rb2D.linearVelocity = _rb2D.linearVelocity.With(y: yMaxVelocity);
        }
        
        internal void FixedTick() => _request.TickFixed();

        #endregion
       
        #region Request Callbacks
        
        internal void RequestCutJump() => _request.AsSingle(_cutJump);
        internal void RequestJump() => _request.AsSequence(_jump, _flying);
        internal void RequestDash() => _request.AsSingle(_dash);
        internal void RequestRoll() => _request.AsSingle(_roll);
        
        #endregion
        
        #region Collision
        
        internal void CollisionEntry(RaycastHit2D raycastHit, bool isGrounded, bool isCelling, int wallDir) {
            if (isGrounded || wallDir != 0) {
                _model.IsJumpCut = false;  
                _model.CurrentJumpCount = 0;
                _model.JumpCoyoteTime = Data.Jump.CoyoteTime;

                if (isGrounded) {
                    _view.EndJump();
                    _rb2D.gravityScale = _model.GravityScale;
                }
            }

            _model.IsDashReset = true;
        }

        // !Warning - Method is a reversion logic -> true mean's that's state changed
        internal void CollisionExit(bool isGrounded, bool isCelling, int wallDir) {
            // It means a player leaves the ground
            if (isGrounded && _model.CurrentJumpCount <= 0) 
                _request.AsSingle(_flying);
        }

        #endregion
        
        // States
        #region Jump
        
        void CutJump() {
            _model.IsJumpCut = true;
            _rb2D.gravityScale = _model.GravityJumpCutFall;
            
        }
        
        void Jump() {
            _model.IsJumpCut = false;
            _model.CurrentJumpCount++;
            _rb2D.gravityScale = _model.GravityScale;

            _rb2D.linearVelocity = _rb2D.linearVelocity.With(y: 0f);
            _rb2D.AddForce(_model.JumpForce * Vector2.up, ForceMode2D.Impulse);
        }

        #endregion

        #region Movement
        
        void TickMovement(float factor, float speed) {
            float velocityX = _rb2D.linearVelocity.x;
            
            float targetSpeed = _model.DirX * speed;
            targetSpeed = Mathf.Lerp(velocityX, targetSpeed, factor);

            float moveSpeed = (targetSpeed - velocityX)
                * 1 / Time.fixedDeltaTime * factor;
            
            _rb2D.AddForce(moveSpeed * Vector2.right, ForceMode2D.Force);
        }

        internal void ForceStopMovement() {
            /*float frictionForce = -_rb2D.linearVelocity.x / Time.fixedDeltaTime * 1;
            _rb2D.AddForce(frictionForce * Vector2.right, ForceMode2D.Force);*/
            
            _rb2D.linearVelocity = Vector2.zero;
        }

        internal void MakeStep() {
            float distance = 2f;
            float duration = 0.3f;

            Vector3 stepDirection = Vector2.right * _model.LockedDirection * distance;
            Vector3 targetPosition = (Vector3)_rb2D.position + stepDirection;
            
            _rb2D.DOMove(targetPosition, duration)
                .SetEase(Ease.OutSine);
        }

        #endregion

        #region Flip

        void Flip() {
            int flipDir = (int)_model.LockedDirection;
            float flipValue = flipDir == 1 ? 90f : -90f;
            
            _model.CurrentFlipDir = flipDir;
            _modelTransform.rotation = Quaternion.Euler(0, flipValue, 0f);
        }

        #endregion
        
        #region Dodge Roll

        void Roll() {
            // View rolling
            int rollDir = _model.RoundedDirX.
                Without(0, _model.CurrentFlipDir);
            
            float duration = rollDir == 1 ? Data.Roll.DurationForward : Data.Roll.DurationBackward;
            float distance = rollDir == 1 ? Data.Roll.DistanceForward : Data.Roll.DistanceBackward;
            
            float targetDir = _rb2D.position.x + rollDir * distance;
            
            _model.IsRolling = true;
            _model.TimeToRoll = Data.Roll.Cooldown + duration;
            
            _rb2D.
                DOMoveX(targetDir, duration).
                SetEase(Ease.OutQuad).
                OnComplete(ResetRollStates);
            
            int viewDir = (_model.RoundedDirX * (int)Data.Direction).
                Without(0, 1);
            
            switch (viewDir) {
                case 1: _view.ViewRollForward(); break;
                case -1: _view.ViewRollBackward(); break;
            }
        }
        
        void ResetRollStates() => _model.IsRolling = false;

        #endregion

        // TODO:
        #region Dash

        Coroutine _dashCooldown;

        void Dash(float progress) {
            var child = _rb2D.transform.GetChild(0);
            var target = Mathf.Approximately(child.rotation.y, 90) ? 1 : -1f;
            
            var velocity = Mathf.Clamp(
                Mathf.Clamp01(Data.Dash.Curve.Evaluate(progress / Data.Dash.Duration)) * Data.Dash.Speed * target,
                -Data.Dash.Speed, 
                Data.Dash.Speed);
            
            _rb2D.linearVelocity = new Vector2(velocity, 0);
        }

        IEnumerator DashCooldown() {
            _model.IsDashReady = false;
            yield return new WaitForSeconds(Data.Dash.Cooldown);
            _model.IsDashReady = true;

            if (_model.IsGrounded)
                _model.IsDashReset = true;
        }

        void ProcessDashCooldown() {
            if(_dashCooldown != null)
                StaticCoroutine.AbortCoroutine(_dashCooldown);

            _dashCooldown = StaticCoroutine.RunCoroutine(DashCooldown());
        }

        void PrepareDash() {
            _model.IsDashing = true;
            _model.IsDashReset = false;

            _rb2D.linearVelocity = _rb2D.linearVelocity.With(y: 0f);
        }

        void StopDash() {
            _model.IsDashing = false;
        }

        #endregion
    }
}