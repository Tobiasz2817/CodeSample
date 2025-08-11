using UnityEngine;
using System;
namespace Ability {
    [CreateAssetMenu]
    public class AbilityDataScriptableObject : ScriptableObject {
        public AbilityData Data;

        void OnValidate() =>
            Data.Id = GetHashCode();
    }
    
    [Serializable]
    public struct AbilityData {
        public int Id;
        public float Cooldown;
        public string Animation;
        public Sprite Sprite;
        [SerializeReference] public IAbility Ability;
        [Ability] public string AbilityName;
    }
}