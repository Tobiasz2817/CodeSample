using UnityEngine.InputSystem;
using GameZone.Scripts.Input;
using Cinemachine;
using UnityEngine;

namespace GameZone.Scripts.Camera
{
    public class AimComposerMoveY : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private float speedAimY = 2;
    
        private CinemachineComposer aim;
        private InputActionReference deltaInputReference;
    
        private void Awake() {
            aim = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineComposer>();
            deltaInputReference = InputManager.Instance.GetInput(InputManager.InputActionKey.MouseDelta);
        }
    
        private void OnEnable() {
            deltaInputReference.action.performed += UpdateCamera;
            deltaInputReference.action.canceled += UpdateCamera;
            deltaInputReference.action.Enable();
        }
    
        private void OnDisable() {
            deltaInputReference.action.performed -= UpdateCamera;
            deltaInputReference.action.canceled -= UpdateCamera;
            deltaInputReference.action.Disable();
        }

        private void UpdateCamera(InputAction.CallbackContext obj) {
            var mouseDelta = obj.ReadValue<Vector2>();
            UpdateAxis(mouseDelta.y);
        }

        private void UpdateAxis(float yAxis) {
            var clamp = Mathf.Clamp(yAxis,-1, 1);
            if (clamp == 0) return;


            aim.m_TrackedObjectOffset.y +=(speedAimY * Time.deltaTime * clamp);
        }
    }

}

