using System.Collections.Generic;
using HexSystem;
using NightShop.NightShopStates;
using UI.NightShop;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace NightShop
{
    public class NightShopManager : MonoBehaviour
    {
        [SerializeField] private NightShopUI nightShopUI;
        [SerializeField] private UnitPlacement _unitPlacement;
        [SerializeField] private MoneyController moneyController;
        [SerializeField] private GridData _gridData;
        [SerializeField] private List<Card> cards;
    
        public Card selectedCard;
        public Hexagon selectedHexagon;
    
        private readonly NightShopStateManager _stateManager = new();
    
        private void Start()
        {
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += ToggleNightShop;
            nightShopUI.Initialize(cards);
            _stateManager.ChangeState(new ChoosingCardState(this));
        }
    
        void ToggleNightShop(DayNightCycle.CycleState cycleState)
        {
            if (cycleState == DayNightCycle.CycleState.Day)
            {
                nightShopUI.transform.gameObject.SetActive(false);
                _stateManager.ChangeState(new ClosedState());
            }
            else
            {
                nightShopUI.transform.gameObject.SetActive(true);
                _stateManager.ChangeState(new ChoosingCardState(this));
            }
        }
    
        public void HandleSelectedCard(Card card)
        {
            selectedCard = card;
            _stateManager.ChangeState(new ChoosingHexagonState(this));
        }

        public void HandleDeselectedCard()
        {
            selectedCard = null;
            _stateManager.ChangeState(new ChoosingCardState(this));
        }

        public void HandleSelectHexagon(Hexagon hexagon)
        {
            if (!CheckIfHexagonValidForPlacement(hexagon)) return;

            selectedHexagon = hexagon;
            moneyController.HandlePurchaseCommand(selectedCard.cost);
        }

        bool CheckIfHexagonValidForPlacement(Hexagon hexagon)
        {
            var playerId = NetworkManager.Singleton.LocalClientId;
            var hexagonData = _gridData.GetHexagonDataOnCoordinate(hexagon.Coordinates);

            if (hexagonData.ControllerPlayerId != playerId)
                return false;

            if (hexagonData.IsWarGround)
                return false;

            return true;
        }
    
        public void OnSuccessfulPurchase()
        {
            _unitPlacement.HandlePlacementCommand(selectedHexagon.Coordinates, 1);

            if (!selectedCard)
            {
                _stateManager.ChangeState(new ChoosingCardState(this));
                return;
            }
            
            _stateManager.ChangeState(new ChoosingHexagonState(this));
        }
        
        public void OnMoneyComparison(bool success)
        {
            if (success)
            {
                _stateManager.ChangeState(new ChoosingHexagonState(this));
                return;
            }
        
            _stateManager.ChangeState(new ChoosingCardState(this));
        }
    
        public void OnFailedPurchase()
        {
            _stateManager.ChangeState(new ChoosingHexagonState(this));
        }
    }
}
