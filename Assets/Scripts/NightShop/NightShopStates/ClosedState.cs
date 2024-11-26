namespace NightShop.NightShopStates
{
    public class ClosedState : INightShopState
    {
            public void EnterState()
            {
                GameEvents.NIGHT_SHOP.OnCardDeselected?.Invoke();
            }

            public void ExitState()
            {
                return;
            }
    }
}
