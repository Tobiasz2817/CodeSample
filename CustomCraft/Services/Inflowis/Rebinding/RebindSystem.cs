using static UnityEngine.InputSystem.InputActionRebindingExtensions;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem;
using CoreUtility.Extensions;
using System.Collections;
using UnityEngine;
using System.Linq;
using CoreUtility;
using Inflowis;

namespace UI.Popups {
    public class RebindSystem {
        static RebindConfig _config;
        readonly RebindView _view;
        const float ClearTime = 0.2f;
        
        RebindingOperation _rebindingOperation;
        
        DeviceName _processDevice;
        
        public RebindSystem(RebindView view) {
            _view = view;
            _view.OnBindPress += (id, bindId) => {
                // ignore self rebinding let press button
                // e.g. left mouse is selecting with that we can set left mouse by pressing the button image
                if(_rebindingOperation != null)
                    return;
                
                ProcessRebinding(id, bindId);
            };
        }

        public void InitializeDevice(DeviceName device) {
            _rebindingOperation?.Cancel();
            _processDevice = device;
            
            _view.ClearView();
            _view.CreateView(device);
        }
        
        void ProcessRebinding(int id, int bindId) {
            var rebindData = GetBindData(id);
            var action = rebindData.Action.action;
            
            _rebindingOperation?.Cancel();
            action?.Disable();
            _rebindingOperation = new RebindingOperation();
            _rebindingOperation.WithAction(action).
                WithTargetBinding(bindId);

            foreach (var allowedDevice in rebindData.AllowedDevices.Without()) 
                _rebindingOperation.WithControlsExcluding(allowedDevice.ToString());

            foreach (var controlName in _config.ExcludingControls) 
                _rebindingOperation.WithControlsExcluding(controlName);
            
            
            _view.StartRebinding(id);

            _rebindingOperation.OnCancel(operation => {
                _view.StopRebinding(id);
                operation.action.Enable();
                
                StaticCoroutine.RunCoroutine(ClearRebindingOperationAfterTime(ClearTime));
            });
            _rebindingOperation.OnComplete(operation => {
                var newBind = operation.action.bindings[bindId];
                
                // Remove duplicates
                RemoveBindDuplicates(id, newBind.effectivePath, newBind.groups);

                Sprite rebindImage = BindSprites.Get(_processDevice, newBind.effectivePath);
                rebindImage ??= _config.NoBindingSprite;
                _view.UpdateImage(id, rebindImage);
                _view.StopRebinding(id);
                operation.action.Enable();

                StaticCoroutine.RunCoroutine(ClearRebindingOperationAfterTime(ClearTime));
            });

            _rebindingOperation.Start();
        }
        
        public void CheckRebindingInterruption() {
            bool isGamepadPressed = Gamepad.current != null && Gamepad.current.allControls.Any(x => x is ButtonControl { wasPressedThisFrame: true } && !x.synthetic);
            bool isMousePressed = Mouse.current != null && Mouse.current.allControls.Any(x => x is ButtonControl { wasPressedThisFrame: true } && !x.synthetic);
            bool isKeyboardPressed = Keyboard.current != null && Keyboard.current.allControls.Any(x => x is ButtonControl { wasPressedThisFrame: true } && !x.synthetic);
            
            switch (_processDevice) {
                case DeviceName.Keyboard when isGamepadPressed || isMousePressed:
                case DeviceName.Mouse when isKeyboardPressed || isGamepadPressed:
                case DeviceName.DualSense when isMousePressed || isKeyboardPressed:
                case DeviceName.DualShock when isMousePressed || isKeyboardPressed:
                case DeviceName.XboxController when isMousePressed || isKeyboardPressed:
                    _rebindingOperation?.Cancel();
                    break;
            }
        }

        void RemoveBindDuplicates(int actionIndex, string overridePath, string targetGroup) {
            targetGroup = targetGroup.Replace(";", "");
            
            var map = GetDeviceMapData();
            var binds = map.Binds;
            for (int k = 0; k < binds.Length; k++) {
                int bindIndex = binds[k].BindingIndex;
                var action = binds[k].Action.action;
                var binding = action.bindings[bindIndex];

                if(k == actionIndex)
                    continue;
                
                string groups = binding.groups.Replace(";", "");
                
                if (binding.effectivePath == overridePath && 
                    groups == targetGroup) {
                    _view.UpdateImage(k, null);
                    action.ApplyBindingOverride(bindIndex, "");
                    break;
                }
            }
        }

        IEnumerator ClearRebindingOperationAfterTime(float time) {
            yield return new WaitForSecondsRealtime(time);
            _rebindingOperation = null;
        }
        
        RebindMapData GetDeviceMapData() => _config.MapData[(int)_processDevice];
        ReBindData GetBindData(int id) => GetDeviceMapData().Binds[id];
    }
}