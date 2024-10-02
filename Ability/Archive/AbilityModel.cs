using System;
using System.Linq;
using System.Threading.Tasks;
using CoreUtility.Extensions;
using GameZone.Storex;
using UnityEngine;

namespace Ability.Archive {
    public class AbilityModel {
        const string AbilitiesFileName = "Abilities";
        
        // Contains only available Abilities
        internal AbilityData[] Abilities;
        internal Action<int, AbilityData> OnValueChanged;

        internal AbilityModel(AbilityData[] bootAbilities = null) {
            LoadAbilities(bootAbilities);
            
            Application.quitting -= SaveAbilities;
            Application.quitting += SaveAbilities;
        }

        public void AddAt(int index, AbilityData ability) {
            if (index >= Abilities.Length)
                return;

            Abilities[index] = ability;
            OnValueChanged?.Invoke(index, ability);
        }
        
        public void RemoveAt(int index) {
            if (index >= Abilities.Length)
                return;

            Abilities[index] = default;
            OnValueChanged?.Invoke(index, default);
        }
        
        public AbilityData? GetAbility(Type abilityType) =>
            Abilities.FirstOrDefault((ab) => ab.Ability.GetType() == abilityType);

        async void LoadAbilities(AbilityData[] bootAbilities) {
            var exceptionArray = new AbilityData[AbilitiesCore.Config.ExceptionAbilitiesCount];
            
            var data = StorexVault.Load<AbilitiesData>(AbilitiesFileName);
            if (data.AbilitiesIds == null) {
                Abilities = bootAbilities ?? exceptionArray;
                return;
            }
            
            Abilities = (await AbilitiesCore.FetchAbilities() ?? exceptionArray).
                Where((abilityData) => data.AbilitiesIds.Contains(abilityData.Id)).
                ToArray();
        }

        void SaveAbilities() =>
            StorexVault.Save(new AbilitiesData { AbilitiesIds = Abilities.Select((data) => data.Id).ToArray() }, AbilitiesFileName);
    }

    [Serializable]
    public struct AbilitiesData {
        public int[] AbilitiesIds;
    } 
}