using System.Collections.Generic;
using System.Threading.Tasks;
using Core.GameEvents;
using Helper;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;

namespace Core.NightShop
{
    public class MoneyController : NetworkBehaviour
    {
        [SerializeField] private NightShopManager nightShopManager;
        [SerializeField] private int moneyPerRound;

        private readonly Dictionary<long, PurchaseRequestResponseResult> _clientPurchaseRequestResultsById = new();

        private const int PurchaseRequestPollDelayInMs = 25;

        #region Server

        public override void OnNetworkSpawn()
        {
            if (!IsServer) 
                return;
            
            ClientEvents.DayNightCycle.OnSwitchedCycleState += OnSwitchedCycleState;
            GrantPlayersRoundMoney();
        }

        private void OnSwitchedCycleState(DayNightCycle.DayNightCycle.CycleState cycleState)
        {
            if (cycleState == DayNightCycle.DayNightCycle.CycleState.Night)
                GrantPlayersRoundMoney();
        }

        private void GrantPlayersRoundMoney()
        {
            var players = HostSingleton.Instance.GameManager.GetPlayers();
            foreach (var player in players)
            {
                player.Money.Increase(moneyPerRound);
                
                var clientRpcParams = HelperMethods.GetClientRpcParamsToSingleTarget(player.ClientId);
                OnMoneyChangedClientRpc(player.Money.Get(), clientRpcParams);
            }
        }
        
        [Rpc(SendTo.Server)]
        private void RequestPurchaseRpc(int cost, long requestId, ulong playerId)
        {
            var playerData = HostSingleton.Instance.GameManager.GetPlayerByClientId(playerId);
            var succeeded = playerData.Money.AttemptToPurchase(cost);
            
            var clientRpcParams = HelperMethods.GetClientRpcParamsToSingleTarget(playerId);
            OnMoneyChangedClientRpc(playerData.Money.Get(), clientRpcParams);
            HandlePurchaseRequestResponseClientRpc(requestId, succeeded, clientRpcParams);
        }

        #endregion
        
        
        #region Client

        public async Task<bool> RequestPurchaseAsync(int amount)
        {
            var requestId = System.DateTime.UtcNow.Ticks;
            _clientPurchaseRequestResultsById.Add(requestId, null);
            
            RequestPurchaseRpc(amount, requestId, NetworkManager.Singleton.LocalClientId);
            
            Debug.Log("Starts waiting for Request Response");
            while (_clientPurchaseRequestResultsById[requestId] == null)
                await Task.Delay(PurchaseRequestPollDelayInMs);
            
            Debug.Log("Got answer!");
            var gotAccepted = _clientPurchaseRequestResultsById[requestId].Accepted;
            _clientPurchaseRequestResultsById.Remove(requestId);
            return gotAccepted;
        }
        
        [ClientRpc]
        private void HandlePurchaseRequestResponseClientRpc(long requestId, bool requestAccepted, ClientRpcParams clientRpcParams)
        {
            _clientPurchaseRequestResultsById[requestId] = new PurchaseRequestResponseResult {Accepted = requestAccepted};
        }
        
        [ClientRpc]
        private void OnMoneyChangedClientRpc(int money, ClientRpcParams clientRpcParams)
        {
            ClientEvents.NightShop.OnMoneyAmountChanged?.Invoke(money);
        }

        private class PurchaseRequestResponseResult
        {
            public bool Accepted { get; set; }
        }
        
        #endregion
        
    }
}