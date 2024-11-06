
using System;
using UnityEngine;

namespace Player
{
    public class PlayerMoney : MonoBehaviour
    {
        [SerializeField] private int moneyPerRound;
        private int _currentMoney;

        private void OnEnable()
        {
            GameEvents.NIGHT_SHOP.OnMoneySpent += SpendMoney;
            GameEvents.DAY_NIGHT_CYCLE.OnSwitchedCycleState += HandleDayNightCycleSwitched;
        }

        private void HandleDayNightCycleSwitched(DayNightCycle.CycleState newDayNightCycle)
        {
            if(newDayNightCycle == DayNightCycle.CycleState.Night)
                ResetMoney();
        }

        private void SpendMoney(int amount)
        {
            if (_currentMoney - amount < 0) return;
            
            _currentMoney -= amount;
            GameEvents.NIGHT_SHOP.OnMoneyAmountChanged(_currentMoney);
        }

        private void ResetMoney()
        {
            _currentMoney = moneyPerRound;
            GameEvents.NIGHT_SHOP.OnMoneyAmountChanged(_currentMoney);
        }
    }
}
