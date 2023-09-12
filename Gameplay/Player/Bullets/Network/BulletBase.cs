using Unity.Netcode;
using UnityEngine;

namespace GameZone.Multiplayer.Game.Bullet
{
    public class BulletBase : NetworkBehaviour
    {
        [SerializeField] protected NetworkDirectionPredicitonComponent networkPredictionComponent;
        [SerializeField] protected ObjectPoolType bulletType;
        [SerializeField] protected float bulletSpeed;
        
        protected virtual void OnEnable() {
            networkPredictionComponent.OnMoveTo += UpdatePosition;
        }
    
        protected virtual void OnDisable() {
            networkPredictionComponent.OnMoveTo -= UpdatePosition;
        }

        protected virtual void Update() {
            if(IsOwner) {
                var dir = transform.forward;
                networkPredictionComponent.ProcessLocalPlayerMovement(dir,Vector3.forward);
            }
            else {
                networkPredictionComponent.ProcessSimulatedPlayerMovement();
            }
        }
    
        protected virtual void UpdatePosition(Vector3 position) {
            transform.position += position * bulletSpeed * networkPredictionComponent.GetTickRate();
        }

        public ObjectPoolType GetBulletType() => bulletType;
    }
}