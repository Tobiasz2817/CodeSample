using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using UnityEngine;
using Utilities;


namespace GameZone.Multiplayer.UGS.Lobby.Core
{
    using Unity.Services.Lobbies.Models;
    using InvokeCondition;  
    using LobbyContains;  

    public class LobbyManager : SingletonPersistent<LobbyManager>
    {
        private Lobby currentLobby;
        private bool isInvoke = false;

        private void OnEnable() {
            CreateRoomHandler.OnCreateRoomSucceed += SetNewLobby;
            GetLobbyHandler.OnGetLobbySucceed += SetNewLobby;
            JoinRoomHandler.OnJoinRoomSucceed += SetNewLobby;
            DeleteRoomHandler.OnDeleteRoomSucceed += ResetLobby;
            KickedFromLobbyHandler.OnKickedFromLobbySucceed += ResetLobby;
            RemovePlayerHandler.OnRemoveSelfSucceed += ResetLobby;
        }

        private void OnDisable() {
            CreateRoomHandler.OnCreateRoomSucceed -= SetNewLobby;
            GetLobbyHandler.OnGetLobbySucceed -= SetNewLobby;
            JoinRoomHandler.OnJoinRoomSucceed -= SetNewLobby;
            DeleteRoomHandler.OnDeleteRoomSucceed -= ResetLobby;
            KickedFromLobbyHandler.OnKickedFromLobbySucceed -= ResetLobby;
            RemovePlayerHandler.OnRemoveSelfSucceed -= ResetLobby;
        }

        private void SetNewLobby(Lobby obj) {
            currentLobby = obj;
        }

        private void ResetLobby() {
            currentLobby = null;
        }


        private Dictionary<ActionKey, ILobbyInvoker> invokers = new Dictionary<ActionKey, ILobbyInvoker>();

        private void Start() {
            var references = CreateReferences();
            foreach (var lobbyHandler in references)
                invokers.Add(lobbyHandler.ActionKey, lobbyHandler);

            Debug.Log("Count Handlers: " + references.Count);
        }

        private List<LobbyHandler> CreateReferences() {
            var list = new List<LobbyHandler>();

            list.Add(new CreateRoomHandler(this));
            list.Add(new DeleteRoomHandler(this));
            list.Add(new GetLobbyHandler(this));
            list.Add(new HeartbeatHandler(this));
            list.Add(new JoinRoomHandler(this));
            list.Add(new RemovePlayerHandler(this));
            list.Add(new UpdateLobbyHandler(this));
            list.Add(new CreateRelayConnectionHandler(this));
            list.Add(new JoinRelayConnectionHandler(this));
            list.Add(new JoinRandomRoomHandler(this));
            new KickedFromLobbyHandler();
            new HostMigrationHandler();
            return list;
        }
        
        public async void InvokeCouple(ActionKey[] actionKeys, LobbyParametersData[] lobbyData) {
            if (actionKeys.Length != lobbyData.Length) return;
            if (!AuthenticationService.Instance.IsAuthorized) return;
            if (IsInvoke()) return;

            SetIsInvoke(true);

            for (int i = 0; i < actionKeys.Length; i++)
                await InvokeLobbyAction(actionKeys[i], lobbyData[i], InvokeConditionTypes.OnlyWaitUntilRateLimit);

            SetIsInvoke(false);
        }


        public async Task InvokeWithoutCondition(ActionKey actionKey, LobbyParametersData lobbyParametersData) {
            if (!AuthenticationService.Instance.IsAuthorized) return;
            if (!invokers[actionKey].GetRateLimit().CanInvoke()) return;
            if (!invokers[actionKey].CanEntry()) return;

            await BaseInvokeAction(actionKey, lobbyParametersData);
        }

        public async Task InvokeLobbyAction(ActionKey actionKey, LobbyParametersData lobbyParametersData,
            InvokeCondition invokeCondition = null) {
            if (!AuthenticationService.Instance.IsAuthorized) return;
            if (!CanEntryExtension(actionKey)) return;

            SetOnBasedConditionWhenIsNull(ref invokeCondition);

            if (invokeCondition.IsBasedOnIsInvoke())
                if (invokeCondition.IsWaitUntilIsInvoke())
                    await TaskEx.WaitUntil(() => !IsInvoke());
                else if (IsInvoke())
                    return;

            if (!invokeCondition.IsWaitUntilRateLimitDown())
                if (!CanEntryRateLimit(actionKey))
                    return;


            if (invokeCondition.IsBasedOnIsInvoke()) SetIsInvoke(true);

            await TaskEx.WaitUntil(() => CanEntryRateLimit(actionKey));
            await BaseInvokeAction(actionKey, lobbyParametersData);

            if (invokeCondition.IsBasedOnIsInvoke()) SetIsInvoke(false);
        }

        private async Task BaseInvokeAction(ActionKey actionKey, LobbyParametersData lobbyParametersData) {
            Debug.Log("Invoke Handler");
            try {
                invokers[actionKey].GetRateLimit().InvokeCounting();
                await invokers[actionKey].MakeAction(lobbyParametersData);
            }
            catch (LobbyServiceException e) {
                Debug.Log(e.Message);
            }
            finally {
                invokers[actionKey].GetRateLimit().ResetCalls();
            }
        }

        public void SetOnBasedConditionWhenIsNull(ref InvokeCondition invokeCondition) {
            if (invokeCondition != null) return;

            invokeCondition = InvokeConditionTypes.OnlyIsInvoke;
        }

        public void SetIsInvoke(bool state) {
            isInvoke = state;
        }

        public bool CanEntryRateLimit(ActionKey actionKey) => invokers[actionKey].GetRateLimit().CanInvoke();
        public bool CanEntryExtension(ActionKey actionKey) => invokers[actionKey].CanEntry();


        public Player GetPlayer() {
            return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
                {
                    LobbyContains.KEY_PLAYER_NAME,
                    new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, PlayerData.nickName)
                },
            });
        }

        public Player GetPlayer(string nickName, int iconIndex) {
            return new Player(AuthenticationService.Instance.PlayerId, null, new Dictionary<string, PlayerDataObject> {
                {
                    LobbyContains.KEY_PLAYER_NAME,
                    new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, nickName)
                }, {
                    LobbyContains.KEY_PLAYER_ICON_INDEX,
                    new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, iconIndex.ToString())
                },
            });
        }

        public bool IsInvoke() => isInvoke;
        public Lobby GetLobby() => currentLobby;
        public string GetLobbyId() => currentLobby.Id;
        public bool IsLobbyHost() => IsLobbyHost(currentLobby);
        public bool IsLobbyHost(Lobby lobby) => IsLobbyExist(lobby) && lobby.HostId == AuthenticationService.Instance.PlayerId;
        
        public bool IsLobbyExist() => IsLobbyExist(currentLobby);
        public bool IsLobbyExist(Lobby lobby) => lobby != null;
        
        public int GetCountPlayersInLobby() => GetCountPlayersInLobby(currentLobby);
        public int GetCountPlayersInLobby(Lobby lobby) => IsLobbyExist(lobby) ? lobby.Players.Count : 0;
        public bool IsLastPlayer() => IsLobbyExist() && currentLobby.Players.Count <= 1;

        public bool IsPlayerInLobby() => IsPlayerInLobby(currentLobby);
        public bool IsPlayerInLobby(Lobby lobby) {
            if (IsLobbyExist(lobby) && lobby.Players != null)
                foreach (Player player in lobby.Players)
                    if (player.Id == AuthenticationService.Instance.PlayerId)
                        return true;

            return false;
        }
    }
}







