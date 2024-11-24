namespace Player
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
                SetMoney(_currentMoney -= amount);
                return true;
            };

            return false;
        }

        public int GetMoney()
        {
            return _currentMoney;
        }

        public void SetMoney(int amount)
        {
            _currentMoney = amount;
        }
    }
}