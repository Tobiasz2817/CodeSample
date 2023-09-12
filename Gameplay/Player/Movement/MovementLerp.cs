using UnityEngine;

namespace GameZone.Multiplayer.Game.Movement.Extension
{

    public class MovementLerp : Core.Movement
    {
        public float lerpT = 12f;

        public override Vector3 MoveTo(Vector3 direction) {
            Vector3 lerp = Vector3.Lerp(target.position, direction, lerpT * Time.deltaTime);
            target.position = lerp;
            return lerp;
        }

        public override void ModificateSpeed(float speedValue) {
            lerpT = speedValue;
        }
    }

}