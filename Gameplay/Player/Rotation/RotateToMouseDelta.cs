using UnityEngine.InputSystem;
using GameZone.Scripts.Input;
using Unity.Netcode;
using UnityEngine;


namespace GameZone.Multiplayer.Game.Rotation
{
    public class RotateToMouseDelta : NetworkBehaviour
    {
        [SerializeField] private float rotateSpeed = 5f;
        [SerializeField] private float maxRotationSpeed = 25f;
        
        [SerializeField] private InputManager.InputActionKey mouseKey = InputManager.InputActionKey.MouseDelta;
        [SerializeField] private Transform targetRotation;

        private InputActionReference mouseDeltaReference;

        private Vector3 lookAtPosition;
        private Vector3 lastLookAtPosition;

        private void Awake() {
            mouseDeltaReference = InputManager.Instance.GetInput(mouseKey);
        }

        public override void OnNetworkSpawn() {
            this.enabled = IsOwner;
            if (!IsOwner) return;


            mouseDeltaReference.action.performed += UpdateRotate;
            mouseDeltaReference.action.Enable();
        }

        public override void OnNetworkDespawn() {
            if (!IsOwner) return;

            mouseDeltaReference.action.performed -= UpdateRotate;
            mouseDeltaReference.action.Disable();
        }

        private void UpdateRotate(InputAction.CallbackContext input) {
            var delta = input.ReadValue<Vector2>();
            lookAtPosition.y = 0;
            lookAtPosition.y = delta.x;
        }

        private void LateUpdate() {
            if (lookAtPosition == lastLookAtPosition) return;

            float speedMultiplier = Mathf.Abs(lookAtPosition.y);
            float currentRotationSpeed = Mathf.Lerp(rotateSpeed, maxRotationSpeed, speedMultiplier);

            float yRotation = targetRotation.eulerAngles.y + lookAtPosition.y * currentRotationSpeed * Time.deltaTime;
            targetRotation.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, yRotation, targetRotation.eulerAngles.z);
            
            lastLookAtPosition = lookAtPosition;
        }


        #region SinglePlayerTesting

        private void OnEnable() {
            mouseDeltaReference.action.performed += UpdateRotate;
            mouseDeltaReference.action.Enable();
        }

        #endregion
    }
}