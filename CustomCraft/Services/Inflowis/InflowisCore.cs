using UnityEngine.InputSystem;
using CoreUtility;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Inflowis {
    public static class InflowisCore {
        public static PlayerInput PlayerInput { private set; get; }

        internal static InflowisConfig Config;

        internal const string ConfigName = "InflowisConfig";
        
        public static void Initialize() {
            Config = Resources.Load<InflowisConfig>(ConfigName);
            
            PlayerInput = PersistentFactory.CreateComponent<PlayerInput>();
            PlayerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
            PlayerInput.actions = Config.InputActionAsset;
            PlayerInput.defaultControlScheme = Config.DefaultScheme;
            PlayerInput.defaultActionMap = Config.DefaultMap;
            PlayerInput.ActivateInput();

            const int ID = 0;
            LoadMapBindingOverrides(ID);
            
            Application.quitting += () => {
                SaveMapBindingOverrides(ID);
            };

            int gameplayMapId = (int)MapType.Gameplay;
            GameInput.Initialize(PlayerInput.actions.actionMaps[gameplayMapId]);
            
            int uiMapId = (int)MapType.UI;
            UIInput.Initialize(PlayerInput.actions.actionMaps[uiMapId]);
            
            PlayerInput.currentActionMap.Enable();
        }

        public static void EnableMap(MapType newMapType, bool disableOthers = false) {
            var maps = Config.InputActionAsset.actionMaps;

            int mapId = (int)newMapType;
            maps[mapId].Enable();

            if (!disableOthers) 
                return;
            
            for (int i = 0; i < maps.Count; i++) {
                if(i == mapId)
                    continue;
                    
                maps[i].Disable();
            }
        }

        static void LoadMapBindingOverrides(int mapId) {
            var firstMap = PlayerInput.actions.actionMaps[mapId];
            string mapJson = PlayerPrefs.GetString(firstMap.id.ToString(), string.Empty);
            if (string.IsNullOrEmpty(mapJson)) 
                return;
            
            firstMap.LoadBindingOverridesFromJson(mapJson);
        }
        
        static void SaveMapBindingOverrides(int mapId) {
            var map = PlayerInput.actions.actionMaps[mapId];
            PlayerPrefs.SetString(map.id.ToString(), map.SaveBindingOverridesAsJson());
        }
    }
} 