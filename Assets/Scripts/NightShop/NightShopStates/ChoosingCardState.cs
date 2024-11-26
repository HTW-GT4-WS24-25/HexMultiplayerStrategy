using UI.NightShop;

namespace NightShop.NightShopStates
{
    public class ChoosingCardState : INightShopState
    {
            private NightShopManager _nightShopManager;

            public ChoosingCardState(NightShopManager nightShopManager)
            {
                _nightShopManager = nightShopManager;
            }
            public void EnterState()
            {
                GameEvents.NIGHT_SHOP.OnCardDeselected?.Invoke();
                GameEvents.NIGHT_SHOP.OnCardSelected += OnCardSelected;
                GameEvents.NIGHT_SHOP.OnCardDeselected += OnCardDeselected;
                
                GameEvents.HEXAGON.OnHideValidHexagonsForPlacement?.Invoke();
            }

            public void ExitState()
            {
                GameEvents.NIGHT_SHOP.OnCardSelected -= OnCardSelected;
                GameEvents.NIGHT_SHOP.OnCardDeselected -= OnCardDeselected;
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