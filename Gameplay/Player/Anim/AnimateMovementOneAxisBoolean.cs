using GameZone.Scripts.Input;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameZone.Multiplayer.Game.Movement.Animate
{
    public class AnimateMovementOneAxisBoolean : NetworkBehaviour
    {
        [SerializeField] private Animator anim;
        [SerializeField] private string animatorBooleanMovementKey = "IsMove";
        [SerializeField] private InputManager.InputActionKey movementKey = InputManager.InputActionKey.Movement;
        
        private NetworkVariable<bool> movement = new NetworkVariable<bool>(default,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
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
            movement.Value = obj.performed;
        }
        
        private void UpdateAnim() {
            anim.SetBool(animatorBooleanMovementKey,movement.Value);
        }

        private void Update() {
            UpdateAnim();
        }

        #endregion



        /*#region SP

        private bool isMoving = false;
        
        public void OnEnable() {
            movementInput.action.performed += ReadMovementSP;
            movementInput.action.canceled += ReadMovementSP;
            movementInput.action.Enable();
        }

        public void OnDisable() {
            movementInput.action.performed -= ReadMovementSP;
            movementInput.action.canceled -= ReadMovementSP;
            movementInput.action.Disable();
        }
        
        private void ReadMovementSP(InputAction.CallbackContext obj) {
            isMoving = obj.performed;
        }

        private void UpdateAnim() {
            anim.SetBool(animatorBooleanMovementKey,isMoving);
        }

        private void Update() {
            UpdateAnim();
        }

        #endregion*/
    }
}