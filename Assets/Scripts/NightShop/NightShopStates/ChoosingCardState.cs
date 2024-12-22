using GameEvents;
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
                ClientEvents.NightShop.OnCardSelected += OnCardSelected;
                ClientEvents.NightShop.OnCardDeselected += OnCardDeselected;
                
                ClientEvents.Hexagon.OnHideValidHexagonsForPlacement?.Invoke();
            }

            public void ExitState()
            {
                ClientEvents.NightShop.OnCardSelected -= OnCardSelected;
                ClientEvents.NightShop.OnCardDeselected -= OnCardDeselected;
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