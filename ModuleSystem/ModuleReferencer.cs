using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace ModuleSystem {
    public class ModuleReferencer : MonoBehaviour, IReferencer {
#if ENABLE_INPUT_SYSTEM
        [SerializeField] InputActionReference _movementInput;
        [SerializeField] InputActionReference _jumpInput;
#endif
    }
}