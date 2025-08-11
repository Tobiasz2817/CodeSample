using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inflowis {
    public class RebindExample : MonoBehaviour {
        [SerializeField] InputActionReference _reference;

        InputActionRebindingExtensions.RebindingOperation _rebinding;

        void OnEnable() {
            _reference.action.performed += (x) => {
                Debug.Log("Process bind");
            };
            _reference.action.Enable();
        }

        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) {
                _reference.action.Disable();
                string prefBind = _reference.action.bindings[0].effectivePath;
                _reference.action.PerformInteractiveRebinding().
                    OnMatchWaitForAnother(0.1f).
                    OnComplete((x) => {
                        Debug.Log($"From: {prefBind} to: {x.action.bindings[0].effectivePath}");   
                        _reference.action.Enable();
                    }).
                    Start();
                
                Debug.Log("Entry space");
            }
        }
    }
}