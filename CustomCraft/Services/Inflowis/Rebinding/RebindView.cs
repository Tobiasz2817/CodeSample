using Object = UnityEngine.Object;
using UnityEngine;
using System.Linq;
using Inflowis;
using System;

namespace UI.Popups {
    public class RebindView {
        static RebindConfig _config;
        readonly Transform _bindsContainer;
        
        BindView[] _views;
        internal event Action<int, int> OnBindPress;
        
        public RebindView(Transform container) {
            _bindsContainer = container;
        }

        internal void CreateView(DeviceName device) {
            var mapData = _config.MapData.FirstOrDefault(m => m.Device == device);
            if (!mapData)
                throw new Exception($"Device '{device}' does not exist");

            var binds = mapData.Binds;
            
            _views = new BindView[binds.Length];
            for (int i = 0; i < binds.Length; i++) {
                var bind = binds[i];
                int index = i;
                int bindIndex = bind.BindingIndex;
                var bindInstance = Object.Instantiate(_config.BindPrefab, _bindsContainer);
                _views[i] = bindInstance;

                // Update name, check name for modifier composite
                var action = bind.Action?.action;
                if (action != null) {
                    var actionName = action.name;

                    var binding = action.bindings[bindIndex];
                    bool isPartOfModifier = action.bindings.Count > 0 && action.bindings[0].name.Contains("Modifier");
                    bool isPartOfComposite = binding.isPartOfComposite;

                    if (!isPartOfModifier && isPartOfComposite)
                        actionName += $" {binding.name}";

                    Sprite rebindImage = BindSprites.Get(device, binding.effectivePath);
                    bindInstance.UpdateImage(rebindImage);
                    bindInstance.UpdateName(actionName);
                    bindInstance.AddBindingCallback(() => OnBindPress?.Invoke(index, bindIndex));
                }
            }
        }

        internal void ClearView() {
            for (int i = 0; i < _bindsContainer.childCount; i++) 
                Object.Destroy(_bindsContainer.GetChild(i).gameObject);
        }
        
        internal void StartRebinding(int id) => _views[id].StartRebinding();
        internal void StopRebinding(int id) => _views[id].StopRebinding();
        internal void UpdateImage(int id, Sprite image) =>_views[id].UpdateImage(image);
    }
}