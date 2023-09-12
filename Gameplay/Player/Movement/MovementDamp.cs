using UnityEngine;

namespace GameZone.Multiplayer.Game.Movement.Extension
{

    public class MovementDamp : Core.Movement
    {
        [SerializeField] private Vector3 velocity;
        [SerializeField] private float smoothTime = 0.1f;
        [SerializeField] private float maxSpeed = 15f;

        public override Vector3 MoveTo(Vector3 direction) {
            Vector3 damp = Vector3.SmoothDamp(target.position, direction, ref velocity, smoothTime, maxSpeed,
                Time.deltaTime);
            target.position = damp;
            return damp;
        }

        public override void ModificateSpeed(float speedValue) {
            smoothTime = speedValue;
        }
    }
}