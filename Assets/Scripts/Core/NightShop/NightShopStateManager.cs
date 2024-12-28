namespace Core.NightShop
{
    public interface INightShopState
    {
        void EnterState();
        void ExitState();
    }
    
    public class NightShopStateManager
    {
        private INightShopState _currentState;

        public void ChangeState(INightShopState newState)
        {
            _currentState?.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }
    }
}