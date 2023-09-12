using GameZone.Scripts.Input;
using UnityEngine.InputSystem;
using UnityEngine;

namespace GameZone.Scripts.Animation
{
    public class AnimationPlayerInput : AnimationInput
    {
        [SerializeField] protected InputManager.InputActionKey inputKeyBind;

        private InputActionReference inputBind;
        private void Awake() {
            inputBind = InputManager.Instance.GetInput(inputKeyBind);
        }

        protected virtual void OnEnable() {
            inputBind.action.performed += InvokeAttackBind;
            inputBind.action.canceled += InvokeAttackBind;
            inputBind.action.Enable();
        }

        protected virtual void OnDisable() {
            inputBind.action.performed -= InvokeAttackBind;
            inputBind.action.canceled -= InvokeAttackBind;
            inputBind.action.Disable();
        }
    
        private void InvokeAttackBind(InputAction.CallbackContext callbackContext) {
            if (callbackContext.performed) 
                OnPress?.Invoke();
            else if (callbackContext.canceled)
                OnRealesed?.Invoke();
        }
    }
}