using UnityEngine.InputSystem;
using GameZone.Scripts.Input;
using Unity.Netcode;
using UnityEngine;

namespace GameZone.Multiplayer.Game.Movement.Animate
{
    public class AnimateMovementDoubleAxis : NetworkBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private string animatorMovementKeyX = "MoveX";
        [SerializeField] private string animatorMovementKeyZ = "MoveZ";
        [SerializeField] private InputManager.InputActionKey movementKey = InputManager.InputActionKey.Movement;
        
        private NetworkVariable<Vector2> movement = new NetworkVariable<Vector2>(default,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
        private InputActionReference movementInput;

        private void Awake() {
            movementInput = InputManager.Instance.GetInput(movementKey);
        }
        
        #region MP

        public override void OnNetworkSpawn() {
            if (!IsOwner) return;
        
            movementInput.action.performed += ReadMovement;
            movementInput.action.canceled += ReadMovement;
            movementInput.action.Enable();
        }

        public override void OnNetworkDespawn() {
            if (!IsOwner) return;

            movementInput.action.performed -= ReadMovement;
            movementInput.action.canceled -= ReadMovement;
            movementInput.action.Disable();
        }
        
        private void ReadMovement(InputAction.CallbackContext obj) {
            movement.Value = obj.ReadValue<Vector2>();
        }
        

        #endregion

        private Vector2 CalculateVelocity(Vector2 newVector3) {
            var input = new Vector3(newVector3.x,0,newVector3.y).normalized;
            var velocityZ = Vector3.Dot(input, transform.forward);
            var velocityX = Vector3.Dot(input, transform.right);
            
            return new Vector2(velocityX, velocityZ);
        }
        
        private void UpdateAnim(Vector2 velocity) {
            anim.SetFloat(animatorMovementKeyZ,velocity.y,0.1f,Time.deltaTime);
            anim.SetFloat(animatorMovementKeyX,velocity.x,0.1f,Time.deltaTime);
        }

        private void Update() {
            var velocity = CalculateVelocity(movement.Value);
            UpdateAnim(velocity);
        }
        
        
        /*#region SinglePlayerTesting

        private Vector2 inputMovementSingle;
        
        private void OnEnable() {
            movementInput.action.performed += ReadMovement;
            movementInput.action.canceled += ReadMovement;
            movementInput.action.Enable();
        }
        
        private void Update() {
            UpdateStates(inputMovementSingle);
        }

        private void ReadMovement(InputAction.CallbackContext obj) {
            inputMovementSingle = obj.ReadValue<Vector2>();
        }
        
        #endregion*/
    }
}

