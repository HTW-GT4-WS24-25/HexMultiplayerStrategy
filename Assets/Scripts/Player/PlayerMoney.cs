
using System;
using UnityEngine;

namespace Player
{
    public class PlayerMoney : MonoBehaviour
    {
        [SerializeField] private int moneyPerRound;
        private int currentMoney;

        private void OnEnable()
        {
            GameEvents.NIGHT_SHOP.OnMoneySpent += SpendMoney;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedToNight += ResetMoney;
        }
        
        void SpendMoney(int amount)
        {
            if (currentMoney - amount < 0) return;
            
            currentMoney -= amount;
            GameEvents.NIGHT_SHOP.OnMoneyAmountChanged(currentMoney);
        }

        void ResetMoney()
        {
            currentMoney = moneyPerRound;
            GameEvents.NIGHT_SHOP.OnMoneyAmountChanged(currentMoney);
        }
    }
}
