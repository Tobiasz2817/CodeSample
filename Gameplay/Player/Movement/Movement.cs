using UnityEngine;

namespace GameZone.Multiplayer.Game.Movement.Core
{
    public abstract class Movement : MonoBehaviour
    {
        [SerializeField] protected Transform target;

        public abstract Vector3 MoveTo(Vector3 direction);
        public abstract void ModificateSpeed(float speedValue);

        public virtual void ChangeComponentState(bool newState) {
            this.enabled = newState;
        }
    }
}