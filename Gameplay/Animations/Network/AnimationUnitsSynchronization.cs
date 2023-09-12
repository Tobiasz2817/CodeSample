using GameZone.Scripts.Animation;
using Unity.Netcode;
using UnityEngine;

namespace GameZone.Multiplayer.Game.Animation.Synchronization
{
    public class AnimationUnitsSynchronization : NetworkBehaviour
    {
        [SerializeField] private AnimationUnit animationUnit;
        [SerializeField] private Animator animator;


        public override void OnNetworkSpawn() {
            this.enabled = IsOwner;
            if (!IsOwner) return;
            animationUnit.OnStartAnim += AnimateUnit;
            animationUnit.OnExecuteAnim += SetAnimationSpeed;
        }

        public override void OnNetworkDespawn() {
            if (!IsOwner) return;
            animationUnit.OnStartAnim -= AnimateUnit;
            animationUnit.OnExecuteAnim -= SetAnimationSpeed;
        }
        
        private void AnimateUnit(AnimationUnit unit) {
            UpdateAnimationUnitServerRpc(unit.GetCurrentAnimationClip().name,unit.GetAnimationSpeed(), unit.speedAnimationFloat);
        }

        [ServerRpc(RequireOwnership = false)]
        private void UpdateAnimationUnitServerRpc(string nameAnimation, float newSpeed, string speedAnimationFloat) {
            UpdateAnimationUnitClientRpc(nameAnimation,newSpeed,speedAnimationFloat);
        }
    
        [ClientRpc]
        private void UpdateAnimationUnitClientRpc(string nameAnimation,float newSpeed, string speedAnimationFloat) {
            if (IsOwner) return;

            SpeedAnimationSpeed(speedAnimationFloat,newSpeed);
            PlayAnimation(nameAnimation);
        }

        private void PlayAnimation(string nameAnimation) {
            animator.Play(nameAnimation);
        }
        
        
        private void SetAnimationSpeed(AnimationUnit unit) {
            UpdateAnimationUnitSpeedServerRpc(unit.startMotionSpeed, unit.speedAnimationFloat);
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void UpdateAnimationUnitSpeedServerRpc(float newSpeed, string speedAnimationFloat) {
            UpdateAnimationUnitSpeedClientRpc(newSpeed,speedAnimationFloat);
        }
    
        [ClientRpc]
        private void UpdateAnimationUnitSpeedClientRpc(float newSpeed, string speedAnimationFloat) {
            if (IsOwner) return;

            SpeedAnimationSpeed(speedAnimationFloat,newSpeed);
        }

        private void SpeedAnimationSpeed(string speedAnimationFloat,float newSpeed) {
            animator.SetFloat(speedAnimationFloat,newSpeed);
        }
    }

}
