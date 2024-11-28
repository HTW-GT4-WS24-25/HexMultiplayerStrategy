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
        private PaymentHandler _paymentHandler;
        

        #region Server

        public override void OnNetworkSpawn() {
            if (IsServer) {
                ClientEvents.DayNightCycle.OnSwitchedCycleState += OnSwitchedCycleState;

                _incomeHandler = new();
                _paymentHandler = new();
                
                GrantPlayersRoundMoney();
            }
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
        private void RequestPurchaseRpc(int cost, ulong playerId)
        {
            var playerData = HostSingleton.Instance.GameManager.PlayerData.GetPlayerById(playerId); 
            var remainingMoney = _paymentHandler.TryToPurchase(cost, playerData);
            var success = remainingMoney != cost;
            
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { playerId }
                }
            };
            
            OnPurchaseClientRpc(remainingMoney, success, clientRpcParams);
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
        
        public void HandlePurchaseCommand(int cost)
        {
            RequestPurchaseRpc(cost, NetworkManager.Singleton.LocalClientId);
        }
        
        public void CheckIfPlayerHasEnoughMoney(int cost)
        {
            RequestMoneyComparisonRpc(cost, NetworkManager.Singleton.LocalClientId);
        }
        
        [ClientRpc]
        private void OnReceivedRoundMoneyClientRpc(int money)
        {
            ClientEvents.NightShop.OnMoneyAmountChanged?.Invoke(money);
        }
        
        [ClientRpc]
        private void OnPurchaseClientRpc(int remainingMoney, bool success, ClientRpcParams clientRpcParams = default)
        {
            if (success)
            {
                ClientEvents.NightShop.OnMoneyAmountChanged?.Invoke(remainingMoney);
                nightShopManager.OnSuccessfulPurchase();
            } else
            {
                nightShopManager.OnFailedPurchase();
            }
        }
        
        [ClientRpc]
        private void OnMoneyComparisonClientRpc(bool success, ClientRpcParams clientRpcParams = default)
        {
            nightShopManager.OnMoneyComparison(success);
        }
        
        #endregion
        
    }
}