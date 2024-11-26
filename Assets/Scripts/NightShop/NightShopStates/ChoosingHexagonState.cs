using HexSystem;
using UI.NightShop;
using UnityEngine;

namespace NightShop.NightShopStates
{ 
    public class ChoosingHexagonState : INightShopState
    {
        private NightShopManager _nightShopManager;

            public ChoosingHexagonState(NightShopManager nightShopManager)
            {
                _nightShopManager = nightShopManager;
            }
            public void EnterState()
            {
                GameEvents.NIGHT_SHOP.OnCardSelected += OnCardSelected;
                GameEvents.NIGHT_SHOP.OnCardDeselected += OnCardDeselected;
                
                GameEvents.INPUT.OnHexSelectedDuringNightShop += OnHexSelectedDuringNightShop;
                GameEvents.HEXAGON.OnShowValidHexagonsForPlacement?.Invoke(); // should be some type / requirement
            }
            
            public void ExitState()
            {
                GameEvents.NIGHT_SHOP.OnCardSelected -= OnCardSelected;
                GameEvents.NIGHT_SHOP.OnCardDeselected -= OnCardDeselected;
                                
                GameEvents.INPUT.OnHexSelectedDuringNightShop -= OnHexSelectedDuringNightShop;
                GameEvents.HEXAGON.OnHideValidHexagonsForPlacement?.Invoke();
            }

            private void OnHexSelectedDuringNightShop(Hexagon hexagon)
            {
                _nightShopManager.HandleSelectHexagon(hexagon);
            }
            
            private void OnCardSelected(Card card)
            {
                _nightShopManager.HandleSelectedCard(card);
            }
            
            private void OnCardDeselected()
            {
                _nightShopManager.HandleDeselectedCard();
            }
        }
    }
