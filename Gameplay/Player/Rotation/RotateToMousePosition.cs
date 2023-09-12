using GameZone.Scripts.Raycasts;
using Unity.Netcode;
using UnityEngine;


namespace GameZone.Multiplayer.Game.Rotation
{
    public class RotateToMousePosition : NetworkBehaviour
    {
        [SerializeField] private Transform targetRotation;

        private Vector3 lookAtPosition;
        
        public override void OnNetworkSpawn() {
            this.enabled = IsOwner;
            if (!IsOwner) return;

            CameraRay.OnDetected += UpdateRotate;
        }

        public override void OnNetworkDespawn() {
            if (!IsOwner) return;

            CameraRay.OnDetected -= UpdateRotate;
        }
        
        private void UpdateRotate(RaycastHit hit) {
            lookAtPosition = new Vector3( hit.point.x, 
                this.targetRotation.position.y, 
                hit.point.z );
        }

        public void RotateTo(Vector3 rotateTo) {
            targetRotation.LookAt(rotateTo);
        }

        public Vector3 GetLookInput() => lookAtPosition;


        /*#region SinglePlayerTesting

        private void OnEnable() {
            CameraRay.OnDetected  += (hit) => {
                UpdateRotate(hit);
                RotateTo(lookAtPosition);
            };
        }

        #endregion*/
    }

}
