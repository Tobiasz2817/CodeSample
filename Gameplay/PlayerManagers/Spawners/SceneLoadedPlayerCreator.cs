using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GameZone.Multiplayer.Game.Spawners.Player
{
    public class SceneLoadedPlayerCreator : NetworkBehaviour
    {
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private Vector3 startPosition;
    
        private List<NetworkObject> activePlayers = new List<NetworkObject>();
    
        public override void OnNetworkSpawn() {
            this.enabled = IsServer;
            if (!IsServer) return;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SpawnCommission;
            NetworkManager.Singleton.OnClientDisconnectCallback += RemoveSpawnCommission;
        }

        public override void OnNetworkDespawn() {
            if (!IsServer) return;
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= SpawnCommission;
            NetworkManager.Singleton.OnClientDisconnectCallback -= RemoveSpawnCommission;
        }


        private void SpawnCommission(ulong clientid, string scenename, LoadSceneMode loadscenemode) {
            SpawnPlayer(clientid);
        }
    
        private void RemoveSpawnCommission(ulong obj) {
            foreach (var player in activePlayers) {
                if (player == null || player.NetworkObjectId == obj) {
                    activePlayers.Remove(player);
                }
            }
        }

        private void SpawnPlayer(ulong clientId) {
            var player = Instantiate(playerPrefab,startPosition,Quaternion.identity);
            player.SpawnAsPlayerObject(clientId);
        }

    }
}


