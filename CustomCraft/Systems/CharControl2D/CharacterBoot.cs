using AnimationEvents;
using AnimationView;
using CoreUtility.Extensions;
using UnityEngine.Scripting;
using UnityEngine;
using Inflowis;

namespace CharControl2D {
    public class CharacterBoot : MonoBehaviour {
        [Header("Settings")] 
        [Header("Data")] 
        [SerializeField] CharViewData _viewData;
        [SerializeField] CharControllerData _controllerData;
        
        [Header("Data")]
        [Header("Collision")]
        [SerializeField] LayerMask _collisionMask;
        [SerializeField] float _collisionThreshold = 0.1f;
        
        
        [Header("References")]
        [SerializeField] Transform _modelTransform;
        
        CharView _view;
        CharModel _model;
        CharInput _input;
        CharController _controller;
        
        Rigidbody2D _rb2D;
        CollisionForge _col2D;

        void Awake() {
            Initialize();
            Connect();
        }

        void Update() {
            _col2D.Tick();
            _controller.Tick();
        }

        void FixedUpdate() => _controller.FixedTick();
        
        void Initialize() {
            var col2D = gameObject.GetOrAddInParent<BoxCollider2D>().WithLayer(_collisionMask);
            _rb2D = gameObject.GetOrAddInParent<Rigidbody2D>().Configure();
            _col2D = new CollisionForge(col2D, _collisionMask);
            
            _model = new CharModel(_controllerData, _viewData);
            
            _view = new CharView(_model);
            _controller = new CharController(_view, _model);
            _controller.InitializeReferences(_rb2D, _modelTransform);
        }
        
        void Connect() {
            ConnectCollisions();
            ConnectInput();
            ConnectWithController();
            ConnectWithAnimation();
        }

        void ConnectWithController() {
            _col2D.OnCollisionEntry += _controller.CollisionEntry;
            _col2D.OnCollisionExit += _controller.CollisionExit;
        }

        void ConnectWithAnimation() {
            _model.IsMovementBlocked = true;
            
            StaticAnimationEventReceiver.SubscribeCallback(AnimationEventType.MakeStepStart, () => {
                _controller.MakeStep();
            });
            
            StaticAnimationEventReceiver.SubscribeCallback(AnimationEventType.MakeStepEnd, () => {
            });
            
            StaticAnimationEventReceiver.SubscribeCallback(AnimationEventType.MakeStep, () => {
                _controller.MakeStep();
            });
            
            StaticAnimationEventReceiver.SubscribeCallback(AnimationEventType.BlockMovement, () => {
                _model.IsMovementBlocked = true;
                
                _controller.ForceStopMovement();
            });
            
            StaticAnimationEventReceiver.SubscribeCallback(AnimationEventType.ResumeMovement, () => {
                _model.IsMovementBlocked = false;
            });

            AnimationService.OnEnd += () => {
                _model.IsMovementBlocked = false;
            };
        }

        void ConnectCollisions() {
            _col2D.OnCollisionStay += (d, u, lr) => {
                _model.IsGrounded = d;
                _model.IsCelling = u;
                _model.WallDir = lr;
            };
        }

        void ConnectInput() {
            GameInput.OnMove += moveDir => {
                _model.DirX = moveDir.x;
                
                _controller.CalculateSmoothDir(_model.DirX);
            };

            GameInput.OnJump += _controller.RequestJump;
            GameInput.OnCutJump += _controller.RequestCutJump;

            GameInput.OnDash += _controller.RequestDash;
            GameInput.OnRoll += _controller.RequestRoll;
        }
        
#if UNITY_EDITOR
        void OnDrawGizmos() => _col2D?.OnDrawGizmos();
#endif
        
        #region Signals

        // Processed by Signals
        [Preserve]
        void OnMovementCondition(bool condition) { }

        #endregion
    }
}