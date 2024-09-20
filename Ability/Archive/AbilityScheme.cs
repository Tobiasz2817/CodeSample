using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CoreUtility;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Ability.Archive {
    public class AbilitySystem : MonoBehaviour {
        
    }
    
    [RequireComponent(typeof(AbilitySystem))]
    public class AbilityInput : MonoBehaviour { 
        AbilitySystem _system;
        
        
    }
    
    [RequireComponent(typeof(AbilitySystem))]
    public class AbilityButtonHandler : MonoBehaviour {
        AbilitySystem _system;
        
        
    }
    
    public class AbilityController {
        AbilityView _view;
        AbilityModel _model;

        public void UpdateView(int id) {
            
        }
    }
    
    public class AbilityView {
        AbilityUI[] _uis;

        public AbilityView(AbilityUI[] uis) {
            _uis = uis;
        }

        public void UpdateView(AbilityData[] abilities) {
            
        }

        public void UpdateFill(int id, AbilityData data) {
            var ui = _uis[id];
            ui.UpdateImage(data.Sprite); 
        }

        public void FillDuration(int id, AbilityData data) {
            var ui = _uis[id];
            
        }
    }
    
    public class AbilityModel {
        // Contains only available Abilities
        public AbilityData[] Abilities;

        public AbilityData? GetAbility(Type abilityType) =>
            Abilities.FirstOrDefault((ab) => ab.Ability.GetType() == abilityType);

        public void AddAbility(AbilityData ability) {
            
        }
        
        public void RemoveAbility(AbilityData ability) {
            
        }
        
        public void ModifyAbility(AbilityData oldAbility, AbilityData newAbility) {
            
        }
        
        public void LoadAbilities() { }
        public void SaveAbilities() { }
    }
    
    public class AbilityUI : MonoBehaviour {
        public Image Img;
        public Button Button;
        public int Id;

        public Action<int> OnPress;

        void Start() {
            Button.onClick.AddListener(() => OnPress?.Invoke(Id));
        }

        public void UpdateImage(Sprite sprite) =>
            Img.sprite = sprite;

        public void FadeDuration() {
            // TODO:
        }
    }
}
