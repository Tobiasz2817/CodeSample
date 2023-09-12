using UnityEngine.InputSystem;
using Utilities;
using System;

namespace GameZone.Scripts.Input
{
    public class InputManager : Singleton<InputManager>
    {
        public UnitInput unitInput;

        public InputActionReference GetInput(InputActionKey inputKey) {
            foreach (var input in unitInput.GetInputs()) {
                if (input.key == inputKey)
                    return input.reference;
            }

            throw new NullReferenceException("Didn't find key, please check list of inputs");
        }

        public enum InputActionKey
        {
            Movement,
            MousePosition,
            MouseDelta,
            LeftMouse,
            RightMouse
        }
    }
}

