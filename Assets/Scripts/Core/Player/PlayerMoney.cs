namespace Core.Player
{
    public class PlayerMoney
    {
        private int _currentMoney;

        public bool CanSpendMoney(int amount)
        {
            return _currentMoney >= amount;
        }
        
        public bool AttemptToPurchase(int amount)
        {
            if (CanSpendMoney(amount))
            {
                Set(_currentMoney -= amount);
                return true;
            };

            return false;
        }

        public int Get()
        {
            return _currentMoney;
        }

        public void Set(int amount)
        {
            _currentMoney = amount;
        }

        public void Increase(int amount)
        {
            _currentMoney += amount;
        }
    }
}