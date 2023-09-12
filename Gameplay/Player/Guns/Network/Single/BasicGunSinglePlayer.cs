using GameZone.Scripts.Raycasts;
using UnityEngine;

namespace GameZone.Multiplayer.Game.Gun
{
    public class BasicGunSinglePlayer : GunBase
    {
        private void OnEnable() {
            animationUnit.OnTriggerEvent += InputCommissionShoot;
            animationUnit.OnReleasedBind += InputReleased;
            mouseInputReference.action.performed += ReadMousePosition;
        }

        private void OnDisable() {
            animationUnit.OnTriggerEvent -= InputCommissionShoot;
            animationUnit.OnReleasedBind -= InputReleased;
            mouseInputReference.action.performed -= ReadMousePosition;
        }

        protected override void CommissionShoot(Vector2 lastMousePosition) {
            var raycastHit = CameraRay.Instance.GetRay(lastMousePosition,hitLayerMask,shootPoint.position.y,1000);
            var direction = raycastHit.point;
            Quaternion rotation = Quaternion.LookRotation((direction - shootPoint.position).normalized);
            rotation.z = 0;
            rotation.x = 0;

            var bullet = Instantiate(bulletPrefab, shootPoint.position, rotation);
        }
    }
}
