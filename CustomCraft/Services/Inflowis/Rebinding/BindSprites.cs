using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Inflowis;
using UnityEngine;

namespace UI.Popups {
    public class BindSprites : MonoBehaviour {
        Dictionary<DeviceName, List<Sprite>> _bindSprites = new();

        static BindSprites _instance;

        async void Awake() {
            _instance = this;
            
            foreach (DeviceName device in Enum.GetValues(typeof(DeviceName))) {
                _bindSprites.TryAdd(device, new List<Sprite>());
                
                var deviceSprites = await InputDatabase.LoadIconsAsync(device);
                foreach (var sprite in deviceSprites) 
                    _bindSprites[device].Add(sprite);
            }
        }

        void OnDestroy() {
            _bindSprites?.Clear();
            _instance = null;
        }

        public static bool IsDeviceInitialized(DeviceName device) => _instance && _instance._bindSprites.ContainsKey(device);
        public static bool IsContainDevice(DeviceName device) => _instance && _instance._bindSprites[device].Count > 0;

        public static Sprite Get(DeviceName device, string path) {
            if (!_instance._bindSprites.ContainsKey(device)) 
                return null;
            
            string filterPath = FilterPath(path);
            return _instance._bindSprites[device].FirstOrDefault(sprite => string.Equals(sprite.name.Replace(" ", ""), filterPath,
                StringComparison.CurrentCultureIgnoreCase));
        }

        static string FilterPath(string path) => path.Replace("<", "").Replace(">", "").Replace("/", " ").Replace(" ", "");
    }
}