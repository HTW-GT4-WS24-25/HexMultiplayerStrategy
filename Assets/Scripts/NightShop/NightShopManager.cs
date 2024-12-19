using System.Collections.Generic;
using GameEvents;
using HexSystem;
using NightShop.NightShopStates;
using TMPro;
using UI.NightShop;
using Unity.Netcode;
using UnityEngine;

namespace NightShop
{
    public class NightShopManager : MonoBehaviour
    {
        [SerializeField] private NightShopUI nightShopUI;
        [SerializeField] private UnitPlacement unitPlacement;
        [SerializeField] private MoneyController moneyController;
        [SerializeField] private GridData gridData;
        [SerializeField] private List<Card> cards;
        [SerializeField] private TextMeshProUGUI readyPlayersText;
        [SerializeField] private TextMeshProUGUI readyButtonText;
        
        private Card _selectedCard;
        private Hexagon _selectedHexagon;
        private bool _readyForDawn;
    
        private readonly NightShopStateManager _stateManager = new();

        private const string ReadyButtonTextReady = "Waiting...";
        private const string ReadyButtonTextUnready = "Ready for Dawn?";

        private void OnEnable()
        {
            ClientEvents.NightShop.OnReadyPlayersChanged += HandleReadyPlayersChanged;
        }

        private void OnDisable()
        {
            ClientEvents.NightShop.OnReadyPlayersChanged -= HandleReadyPlayersChanged;
        }

        private void Start()
        {
            ClientEvents.DayNightCycle.OnSwitchedCycleState += ToggleNightShop;
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
                _readyForDawn = false;
                UpdateReadyButtonText();
            }
        }
    
        public void HandleSelectedCard(Card card)
        {
            _selectedCard = card;
            _stateManager.ChangeState(new ChoosingHexagonState(this));
        }

        public void HandleDeselectedCard()
        {
            _selectedCard = null;
            _stateManager.ChangeState(new ChoosingCardState(this));
        }

        public void HandleSelectHexagon(Hexagon hexagon)
        {
            if (!CheckIfHexagonValidForPlacement(hexagon)) return;

            _selectedHexagon = hexagon;
            moneyController.HandlePurchaseCommand(_selectedCard.cost);
        }

        bool CheckIfHexagonValidForPlacement(Hexagon hexagon)
        {
            var playerId = NetworkManager.Singleton.LocalClientId;
            var hexagonData = gridData.GetHexagonDataOnCoordinate(hexagon.Coordinates);
    
            if (hexagonData.ControllerPlayerId != playerId)
                return false;

            return true;
        }
    
        public void OnSuccessfulPurchase()
        {
            unitPlacement.HandlePlacementCommand(_selectedHexagon.Coordinates, 1);

            if (!_selectedCard)
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

        public void OnReadyForDawnToggled()
        {
            _readyForDawn = !_readyForDawn;
            UpdateReadyButtonText();
            ClientEvents.NightShop.OnLocalPlayerChangedReadyForDawnState(_readyForDawn);
        }

        private void UpdateReadyButtonText()
        {
            readyButtonText.text = _readyForDawn ? ReadyButtonTextReady : ReadyButtonTextUnready;
        }
        
        private void HandleReadyPlayersChanged(int readyPlayers, int maxPlayers)
        {
            readyPlayersText.text = $"{readyPlayers}/{maxPlayers} Players ready";
        }
    }
}
