using System.Collections.Generic;
using CoreUtility;
using UnityEngine;
using Inflowis;
using System;

namespace UI.Popups {
    [CreateAssetMenu(menuName = "Content/Config/Rebind")]
    internal class RebindConfig : ConfigScriptableObject {
        [SerializeField] RebindMapData[] _mapData;
        [SerializeField] BindView _bindPrefab;
        [SerializeField] Sprite _noBindingSprite;
        public RebindMapData[] MapData => _mapData;
        public BindView BindPrefab => _bindPrefab;
        public Sprite NoBindingSprite => _noBindingSprite;

        internal readonly string[] ExcludingControls = {
            "<Gamepad>/escape",
            "<Gamepad>/anyKey",
            "<Mouse>/position",
            "<Mouse>/delta",
        };
        
#if UNITY_EDITOR
        void OnValidate() =>
            Array.Sort(_mapData, new DeviceComparer());
        
        class DeviceComparer : IComparer<RebindMapData> {
            public int Compare(RebindMapData x, RebindMapData y) {
                if (ReferenceEquals(x, y)) return 0;
                if (y is null) return 1;
                if (x is null) return -1;
                if (x.Device == y.Device) return 0;
                bool isHighest = x.Device > y.Device;
                return isHighest ? 1 : -1;
            }
        }
#endif
    }
}