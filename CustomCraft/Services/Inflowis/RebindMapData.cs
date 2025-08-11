using UnityEngine.InputSystem;
using UnityEngine;
using System;

namespace Inflowis {
    [CreateAssetMenu(fileName = "RebindMapData", menuName = "Content/Data/RebindData")]
    public class RebindMapData : ScriptableObject {
        [SerializeField] DeviceName _deviceType;
        [SerializeField] bool _syncAllowedFirstForAll = true;
        [SerializeField] ReBindData[] _binds;

        public ReBindData[] Binds => _binds;
        public DeviceName Device => _deviceType;
        void OnValidate() {
            if (_syncAllowedFirstForAll) {
                if (Binds?.Length > 0) {
                    var firstAllowance = Binds[0].AllowedDevices;

                    for (int i = 1; i < Binds.Length; i++) {
                        _binds[i].AllowedDevices = firstAllowance;
                    }
                }
            }
        }
    }
    
    [Serializable]
    public struct ReBindData {
        [SerializeField] int _bindingIndex;
        [SerializeField] InputActionReference _action;
        [SerializeField] DeviceFull _allowedDevices;
        
        public int BindingIndex => _bindingIndex;
        public InputActionReference Action => _action;

        public DeviceFull AllowedDevices {
            internal set => _allowedDevices = value;
            get => _allowedDevices; 
        }
    }
}