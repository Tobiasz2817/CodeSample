using GameZone.Scripts.Input;
using UnityEngine.InputSystem;
using Unity.Netcode;
using UnityEngine;


namespace GameZone.Multiplayer.Game.Movement
{
    public class CharacterMovement : NetworkBehaviour
    {
        [SerializeField] private Core.Movement movement;
        [SerializeField] private InputManager.InputActionKey movementKey = InputManager.InputActionKey.Movement;
        
        private InputActionReference movementInputReference;
        private Vector3 movementInput;
    
        private void Awake() {
            movementInputReference = InputManager.Instance.GetInput(movementKey);
        }

        public override void OnNetworkSpawn() {
            this.enabled = IsOwner;
            if (!IsOwner) return;

            movementInputReference.action.performed += CommissionMove;
            movementInputReference.action.canceled += CommissionMove;
            movementInputReference.ToInputAction().Enable();
        }

        public override void OnNetworkDespawn() {
            if (!IsOwner) return;

            movementInputReference.action.performed -= CommissionMove;
            movementInputReference.action.canceled -= CommissionMove;
            movementInputReference.ToInputAction().Disable();
        }

        private void CommissionMove(InputAction.CallbackContext obj) {
            var input = obj.ReadValue<Vector2>();
            movementInput.x = input.x;
            movementInput.y = 0;
            movementInput.z = input.y;
        }
        
        public void MoveTo(Vector3 direction) {
            //movement.MoveTo(transform.position + direction);
            movement.MoveTo(direction);
        }

        public Vector3 GetMovementInput() {
            var input = movementInputReference.action.ReadValue<Vector2>();
            Vector3 movementInput;
            movementInput.x = input.x;
            movementInput.y = 0;
            movementInput.z = input.y;

            return movementInput;
        }

        public void ChangeStateMovement(bool newState) {
            movement.ChangeComponentState(newState);
        }

        /*#region SinglePlayerTesting

        private void Update() {
            if (movementInput == Vector3.zero) return;
            MoveTo(movementInput * Time.deltaTime);
        }

        private void OnEnable() {
            movementInputReference.action.performed += CommissionMove;
            movementInputReference.action.canceled += CommissionMove;
            movementInputReference.ToInputAction().Enable();
        }

        #endregion*/
    }

}

