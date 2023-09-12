using Unity.Netcode;
using UnityEngine;
using Cinemachine;

namespace GameZone.Multiplayer.Game.Camera
{
    public class CinemachineFollowRef : NetworkBehaviour
    {
        [SerializeField] private Transform followTo;
        [SerializeField] private Transform lookAt;
        
        public override void OnNetworkSpawn() {
            if(!IsOwner) return;
                
            var camera = FindObjectOfType<CinemachineVirtualCamera>();
            camera.m_Follow = followTo;
            camera.m_LookAt = lookAt;
        }
    }

}

