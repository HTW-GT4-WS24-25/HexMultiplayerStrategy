using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using Networking.Host;
using Unity.Netcode;
using UnityEngine;

namespace NightShop
{
    public class MoneyController : NetworkBehaviour
    {
        [SerializeField] private NightShopManager nightShopManager;
        [SerializeField] private int moneyPerRound;
        
        private IncomeHandler _incomeHandler;

        private readonly Dictionary<long, PurchaseRequestResponseResult> _clientPurchaseRequestResultsById = new();

        private const int PurchaseRequestPollDelayInMs = 25;

        #region Server

        public override void OnNetworkSpawn()
        {
            if (!IsServer) 
                return;
            
            ClientEvents.DayNightCycle.OnSwitchedCycleState += OnSwitchedCycleState;

            _incomeHandler = new IncomeHandler();
                
            GrantPlayersRoundMoney();
        }

        private void OnSwitchedCycleState(DayNightCycle.CycleState cycleState)
        {
            if (cycleState == DayNightCycle.CycleState.Night)
                GrantPlayersRoundMoney();
        }

        private void GrantPlayersRoundMoney()
        {
            _incomeHandler.GrantMoneyToAllPlayers(moneyPerRound);
            OnReceivedRoundMoneyClientRpc(moneyPerRound);
        }
        
        [Rpc(SendTo.Server)]
        private void RequestPurchaseRpc(int cost, long requestId, ulong playerId)
        {
            var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId);
            var succeeded = playerData.PlayerMoney.AttemptToPurchase(cost);
            
            var clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { playerId }
                }
            };
            
            HandlePurchaseRequestResponseClientRpc(requestId, succeeded, clientRpcParams);
        }
        
        [Rpc(SendTo.Server)]
        private void RequestMoneyComparisonRpc(int cost, ulong playerId)
        {
            var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId); 
            var playerMoney = playerData.PlayerMoney;
                
            bool success = playerMoney.CanSpendMoney(cost);
                
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { playerId }
                }
            };
                
            OnMoneyComparisonClientRpc(success, clientRpcParams);
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
        private void OnReceivedRoundMoneyClientRpc(int money)
        {
            ClientEvents.NightShop.OnMoneyAmountChanged?.Invoke(money);
        }
        
        [ClientRpc]
        private void OnMoneyComparisonClientRpc(bool success, ClientRpcParams clientRpcParams = default)
        {
            nightShopManager.OnMoneyComparison(success);
        }

        private class PurchaseRequestResponseResult
        {
            public bool Accepted { get; set; }
        }
        
        #endregion
        
    }
}