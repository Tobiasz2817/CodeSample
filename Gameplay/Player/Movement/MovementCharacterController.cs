using UnityEngine;

namespace GameZone.Multiplayer.Game.Movement.Extension
{
    public class MovementCharacterController : Core.Movement
    {
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float speed = 15;
        public override Vector3 MoveTo(Vector3 direction) {
            var moveTo = direction * speed;
            characterController.Move(moveTo);
            return moveTo;
        }

        public override void ModificateSpeed(float speedValue) {
            speed = speedValue;
        }

        public override void ChangeComponentState(bool newState) {
            characterController.enabled = newState;
        }
    }
}