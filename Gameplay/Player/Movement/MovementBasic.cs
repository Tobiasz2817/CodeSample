using UnityEngine;

namespace GameZone.Multiplayer.Game.Movement.Extension
{

    public class MovementBasic : Core.Movement
    {
        public float movementSpeed;

        public override Vector3 MoveTo(Vector3 direction) {
            Vector3 dir = direction * movementSpeed;
            target.Translate(dir);
            return dir;
        }

        public override void ModificateSpeed(float speedValue) {
            movementSpeed = speedValue;
        }
    }

}