using UnityEngine;

namespace BusSignals.Example {
    public class SignalsExample : MonoBehaviour {
        public float dmg = 5f;
        
        void Awake() =>
            Signals.MovementCondition(false);

        void Update() {
            if (Input.GetKeyDown(KeyCode.Space)) 
                Signals.DamageDeal(dmg);
        }
        
        // Methods processed by signals
        void OnMovementCondition(bool condition) =>
            Debug.Log("OnMovementCondition: " + condition);
        void OnDamageDeal(float damage) =>
            Debug.Log("OnDamageDeal: " + damage);
    }
}