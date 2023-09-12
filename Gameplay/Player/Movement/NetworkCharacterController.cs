using GameZone.Multiplayer.Game.Movement;
using GameZone.Multiplayer.Game.Rotation;
using Unity.Netcode;
using UnityEngine;


namespace GameZone.Multiplayer.Game.CharacterSplitter
{
    
    public class NetworkCharacterController : NetworkBehaviour
    {
        [SerializeField] private CharacterMovement characterMovement;
        [SerializeField] private RotateToMousePosition characterRotator;
        [SerializeField] private NetworkMovementComponent predictionComponent;


        private void OnEnable() {
            predictionComponent.OnMoveTo += MovePlayer;
            predictionComponent.OnRotateTo += RotatePlayer;
            predictionComponent.OnChangeStateController += ChangeStateMovementComponent;
        }

        private void OnDisable() {
            predictionComponent.OnMoveTo -= MovePlayer;
            predictionComponent.OnRotateTo -= RotatePlayer;
            predictionComponent.OnChangeStateController -= ChangeStateMovementComponent;
        }

        private void Update() {
            if (IsClient && IsLocalPlayer) 
                predictionComponent.ProcessLocalPlayerMovement(
                    characterMovement.GetMovementInput(),
                    characterRotator.GetLookInput());
            else
                predictionComponent.ProcessSimulatedPlayerMovement();
        }

        private void MovePlayer(Vector3 moveTo) {
            characterMovement.MoveTo(moveTo * predictionComponent.GetTickRate());
        }
    
        private void RotatePlayer(Vector3 lookAt) {
            characterRotator.RotateTo(lookAt);
        }
    
        private void ChangeStateMovementComponent(bool state) {
            characterMovement.ChangeStateMovement(state);
        }
    }

}
