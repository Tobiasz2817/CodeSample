using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace GameZone.Scripts.Input
{
    
    [CreateAssetMenu(menuName = "Input")]
    public class UnitInput : ScriptableObject
    {
        [Serializable] 
        public class References
        {
            public InputManager.InputActionKey key;
            public InputActionReference reference;
        }

        [SerializeField] private List<References> inputReferences = new List<References>();

        public List<References> GetInputs() => inputReferences;
    }

}
