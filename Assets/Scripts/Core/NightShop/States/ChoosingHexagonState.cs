using Core.GameEvents;
using Core.HexSystem.Hexagon;
using Core.UI.InGame.NightShop;

namespace Core.NightShop.NightShopStates
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
            ClientEvents.NightShop.OnCardSelected += OnCardSelected;
            ClientEvents.NightShop.OnCardDeselected += OnCardDeselected;
            
            ClientEvents.Input.OnHexSelectedDuringNightShop += OnHexSelectedDuringNightShop;
        }
        
        public void ExitState()
        {
            ClientEvents.NightShop.OnCardSelected -= OnCardSelected;
            ClientEvents.NightShop.OnCardDeselected -= OnCardDeselected;
            
            ClientEvents.Input.OnHexSelectedDuringNightShop -= OnHexSelectedDuringNightShop;
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
