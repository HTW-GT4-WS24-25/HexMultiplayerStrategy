using GameEvents;

namespace NightShop.NightShopStates
{
    public class ClosedState : INightShopState
    {
            public void EnterState()
            {
                ClientEvents.NightShop.OnCardDeselected?.Invoke();
                ClientEvents.Hexagon.OnHideValidHexagonsForPlacement?.Invoke();
            }

            public void ExitState()
            {
                return;
            }
    }
}
