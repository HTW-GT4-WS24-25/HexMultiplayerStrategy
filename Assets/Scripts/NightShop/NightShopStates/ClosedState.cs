namespace NightShop.NightShopStates
{
    public class ClosedState : INightShopState
    {
            public void EnterState()
            {
                GameEvents.NIGHT_SHOP.OnCardDeselected();
            }

            public void ExitState()
            {
                return;
            }
    }
}
