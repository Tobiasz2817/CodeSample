using UnityEngine;
using System;

namespace Ability {
    [CreateAssetMenu]
    public class AbilityDataScriptableObject : ScriptableObject {
        public AbilityData Data;
    }

    
    [Serializable]
    public struct AbilityData {
        public float Cooldown;
        public string Animation;
        public Sprite Sprite;
        [SerializeReference] public IAbility Ability;
        [Ability] public string AbilityEnum;
    }
}