using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace GameZone.Multiplayer.Game.Spawners.Player
{
    public class SceneLoadedPlayersCreator : NetworkBehaviour
    {
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private Vector3 startPosition;

        private List<NetworkObject> activePlayers = new List<NetworkObject>();

        public override void OnNetworkSpawn() {
            this.enabled = IsServer;
            if (!IsServer) return;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SpawnCommission;
            NetworkManager.Singleton.OnClientDisconnectCallback += RemoveSpawnCommission;
        }

        public override void OnNetworkDespawn() {
            if (!IsServer) return;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted -= SpawnCommission;
            NetworkManager.Singleton.OnClientDisconnectCallback -= RemoveSpawnCommission;
        }


        private void SpawnCommission(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted,
            List<ulong> clientstimedout) {
            foreach (var client in clientscompleted) {
                SpawnPlayer(client);
            }
        }

        private void RemoveSpawnCommission(ulong obj) {
            foreach (var player in activePlayers) {
                if (player == null || player.NetworkObjectId == obj) {
                    activePlayers.Remove(player);
                }
            }
        }

        private void SpawnPlayer(ulong clientId) {
            var player = Instantiate(playerPrefab, startPosition, Quaternion.identity);
            player.SpawnAsPlayerObject(clientId);
        }

    }
}