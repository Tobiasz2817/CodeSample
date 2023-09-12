using System.Collections;
using GameZone.Multiplayer.Game.Bullet;
using GameZone.Scripts.Animation;
using GameZone.Scripts.Input;
using GameZone.Scripts.Raycasts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameZone.Multiplayer.Game.Gun
{
    public class GunBase : NetworkBehaviour
    {
        [SerializeField] protected float gunDamage = 50;
        [SerializeField] protected float shootPower = 5;
        [SerializeField] protected float shootDelayTime = 0.2f;
        [SerializeField] protected LayerMask hitLayerMask;
        
        [SerializeField] protected BulletBase bulletPrefab;
        [SerializeField] protected Transform shootPoint;
        [SerializeField] protected AnimationUnit animationUnit;
        
        [SerializeField] private InputManager.InputActionKey mousePositionKey = InputManager.InputActionKey.MousePosition;
        
        protected InputActionReference mouseInputReference;
        private Vector2 lastMousePosition;

        protected bool canShoot = true;
        
        protected virtual void Awake() {
            mouseInputReference = InputManager.Instance.GetInput(mousePositionKey);
        }

        public override void OnNetworkSpawn() {
            if (!IsOwner) return;
            
            animationUnit.OnTriggerEvent += InputCommissionShoot;
            animationUnit.OnReleasedBind += InputReleased;
            mouseInputReference.action.performed += ReadMousePosition;
        }
        
        public override void OnNetworkDespawn() {
            if (!IsOwner) return;
            
            animationUnit.OnTriggerEvent -= InputCommissionShoot;
            animationUnit.OnReleasedBind -= InputReleased;
            mouseInputReference.action.performed -= ReadMousePosition;
        }
        
        protected void InputCommissionShoot(AnimationUnit animationUnit) {
            if (!canShoot) return;
            StartCoroutine(BeatShootDelayTimer());
            CommissionShoot(lastMousePosition);

            canShoot = false;
        }
        
        protected virtual void InputReleased(AnimationUnit animationUnit) { }
        
        protected void ReadMousePosition(InputAction.CallbackContext obj) {
            lastMousePosition = obj.ReadValue<Vector2>();
        }

        protected virtual IEnumerator BeatShootDelayTimer() {
            yield return new WaitForSeconds(shootDelayTime);
            canShoot = true;
        }

        protected virtual void CommissionShoot(Vector2 lastMousePosition) {
            var raycastHit = CameraRay.Instance.GetRay(lastMousePosition,hitLayerMask,shootPoint.position.y,1000);
            var direction = raycastHit.point;
            Quaternion rotation = Quaternion.LookRotation((direction - shootPoint.position).normalized);
            rotation.z = 0;
            rotation.x = 0;
            Vector3 calculateDirection = (direction - shootPoint.position).normalized;

            var shoot = new ShootDependencies() {
                startPosition = shootPoint.position,
                startRotation = Quaternion.identity,
                direction = direction,
                normalizeDirection = calculateDirection,
                rotateDirection = rotation
            };

            CommissionShootRequestServerRpc(shoot,OwnerClientId);
        }
            
        
        
        [ServerRpc(RequireOwnership = false)]
        protected virtual void CommissionShootRequestServerRpc(ShootDependencies shootDependencies, ulong ownershipId) {
            NetworkObject newProjectile = NetworkPooler.Instance.GetNetworkObject(PrefabsReference.Instance.GetPrefab(bulletPrefab.GetBulletType()), shootDependencies.startPosition, shootDependencies.rotateDirection);
            
            if(!newProjectile.IsSpawned)
                newProjectile.SpawnWithOwnership(ownershipId);

            //var projectile = newProjectile;
            //var bullet = projectile.GetComponent<BulletBase>();
            //bullet.transform.rotation = shootDependencies.rotateDirection;
        }
        
    }
    
    public struct ShootDependencies : INetworkSerializable
    {
        public Vector3 startPosition;
        public Quaternion startRotation;
        public Vector3 normalizeDirection;
        public Vector3 direction;
        public Quaternion rotateDirection;
    
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref startPosition);
            serializer.SerializeValue(ref startRotation);
            serializer.SerializeValue(ref direction);
            serializer.SerializeValue(ref normalizeDirection);
            serializer.SerializeValue(ref rotateDirection);
        }
    }
}