using CoreUtility;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Ability {
    internal static class AbilitiesCore
    {
        internal static AbilityConfig Config;
        internal const string PathKey = "AbilityConfigKey";  
        
        internal static AbilityData[] Abilities;

        #region Initialize

        [RuntimeInitializeOnLoadMethod]
        static void Initialize() {
            var config = LoadConfig();
            if(!config.LoadInMemory) 
                return; 
            
            _ = FetchAbilities(config);
        }
        
        internal static AbilityConfig LoadConfig() {
            Config = ConfigHandler.LoadConfig<AbilityConfig>(PathKey);
            return Config;
        }

        internal static async Task<AbilityData[]> FetchAbilities(AbilityConfig abilityConfig = null) {
            if (Abilities != null && Abilities.Length > 0)
                return Abilities;
            
            var config = abilityConfig ?? LoadConfig();
            if (config.PathList == null || config.PathList.Length == 0) {
#if UNITY_EDITOR
                Debug.LogWarning("Abilities search path list is empty. Fill the path in AbilityConfig");
#endif
                return Abilities;
            }
            
            var abilities = (await AddressableLoad.Import<AbilityData>(config.SearchType, config.PathList)).ToArray();
            SortAbilities(ref abilities);
            
            Abilities = abilities;
            return Abilities;
        }

        #endregion

        #region Config Utils

        [InitializeOnLoadMethod]
        static void CreateConfig() {
            var path = EditorPrefs.GetString(PathKey, null);
            
            if (!string.IsNullOrEmpty(path) && File.Exists(path)) 
                return;

            var configData = new ConfigData(configName: "AbilityConfig");
            var config = ConfigHandler.CreateConfig<AbilityConfig>(PathKey, ref configData);
            config.FileConfig = configData;
        }

        internal static void MoveConfig(ref ConfigData configData) => ConfigHandler.MoveConfig(PathKey, ref configData);
        internal static void ChangeConfigName(ref ConfigData configData) => ConfigHandler.ChangeConfigName(PathKey, ref configData);

        #endregion

        internal static int GetAbilityId(Func<AbilityData, bool> condition) {
            for (var i = 0; i < Abilities.Length; i++) 
                if (condition.Invoke(Abilities[i]))
                    return i;

            return -1;
        }

        internal static AbilityData GetAbility(string typeName) =>
            Abilities.FirstOrDefault((ability) => string.Equals(ability.Ability.GetType().Name, typeName));
        
        internal static AbilityData GetAbility(int id) =>
            Abilities[id];
        
        /// <summary>
        /// Sort abilities by type name and change it on id 
        /// </summary>
        static void SortAbilities(ref AbilityData[] abilities) =>
            Array.Sort(abilities, (x, y) => -String.Compare(x.GetType().Name, y.GetType().Name, StringComparison.Ordinal));
    }
}