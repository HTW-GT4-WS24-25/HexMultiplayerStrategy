using System.Collections.Generic;
using System.Threading.Tasks;
using GameEvents;
using HexSystem;
using Networking.Host;
using NightShop.NightShopStates;
using NightShop.Placeables;
using TMPro;
using UI.NightShop;
using Unity.Netcode;
using UnityEngine;

namespace NightShop
{
    public class NightShopManager : MonoBehaviour
    {
        [SerializeField] private NightShopUI nightShopUI;
        [SerializeField] private MoneyController moneyController;
        [SerializeField] private GridData gridData;
        [SerializeField] private MapBuilder mapBuilder;
        [SerializeField] private List<Card> cards;
        [SerializeField] private TextMeshProUGUI readyPlayersText;
        [SerializeField] private TextMeshProUGUI readyButtonText;
        
        private Card _selectedCard;
        private Placeable _placeableOnSuccessfulPurchase;
        private HexagonData _selectedHexagon;
        private bool _readyForDawn;
        private int _localPlayerMoneyAmount;
    
        private readonly NightShopStateManager _stateManager = new();

        private const string ReadyButtonTextReady = "Waiting...";
        private const string ReadyButtonTextUnready = "Ready for Dawn?";

        private void OnEnable()
        {
            ClientEvents.NightShop.OnReadyPlayersChanged += HandleReadyPlayersChanged;
            ClientEvents.DayNightCycle.OnSwitchedCycleState += ToggleNightShop;
            ClientEvents.NightShop.OnMoneyAmountChanged += HandlePlayerMoneyChanged;
        }

        private void OnDisable()
        {
            ClientEvents.NightShop.OnReadyPlayersChanged -= HandleReadyPlayersChanged;
            ClientEvents.DayNightCycle.OnSwitchedCycleState -= ToggleNightShop;
            ClientEvents.NightShop.OnMoneyAmountChanged -= HandlePlayerMoneyChanged;
        }

        private void HandlePlayerMoneyChanged(int newMoney)
        {
            _localPlayerMoneyAmount = newMoney;
        }

        private void Start()
        {
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
            _placeableOnSuccessfulPurchase = null;
            _selectedCard.placeable.Initialize(NetworkManager.Singleton.LocalClientId);
            ClientEvents.Hexagon.OnHideValidHexagonsForPlacement?.Invoke();
            
            _stateManager.ChangeState(new ChoosingHexagonState(this));
            HighlightHexagonsValidForSelectedCard();
        }

        private void HighlightHexagonsValidForSelectedCard()
        {
            foreach (var coordinate in HexagonGrid.GetHexRingsAroundCoordinates(new AxialCoordinates(0, 0), mapBuilder.MapRings))
            {
                var hexagonData = gridData.GetHexagonDataOnCoordinate(coordinate);
                
                if(_selectedCard.placeable.IsHexValidForPlacement(hexagonData))
                    mapBuilder.Grid.Get(coordinate).HighlightAsValidForPlacement();
            }
        }

        public void HandleDeselectedCard()
        {
            _selectedCard = null;
            _stateManager.ChangeState(new ChoosingCardState(this));
        }

        public void HandleSelectHexagon(Hexagon hexagon)
        {
            var hexagonData = gridData.GetHexagonDataOnCoordinate(hexagon.Coordinates);
            if (!_selectedCard.placeable.IsHexValidForPlacement(hexagonData)) 
                return;
            
            _selectedHexagon = gridData.GetHexagonDataOnCoordinate(hexagon.Coordinates);
            
            PlaceCurrentSelectionOnSelectedHexAsync();
        }
        
        private async Task PlaceCurrentSelectionOnSelectedHexAsync()
        {
            if (_localPlayerMoneyAmount < _selectedCard.cost)
                return;
            
            _placeableOnSuccessfulPurchase = _selectedCard.placeable;
            var purchaseSucceeded = await moneyController.RequestPurchaseAsync(_selectedCard.cost);
            if(!purchaseSucceeded)
                return;
            
            _placeableOnSuccessfulPurchase?.Place(_selectedHexagon);
            _placeableOnSuccessfulPurchase = null;
            
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
            _placeableOnSuccessfulPurchase = null;
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
