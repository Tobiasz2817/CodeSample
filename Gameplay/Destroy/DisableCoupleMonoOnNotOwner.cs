using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GameZone.Multiplayer.Game.Destroy
{
    public class DisableCoupleMonoOnNotOwner : NetworkBehaviour
    {
        [SerializeField] private List<MonoBehaviour> scriptsToDestroy;

        public override void OnNetworkSpawn() {
            scriptsToDestroy.ForEach((mono) => mono.enabled = IsOwner);
        }
    }
}