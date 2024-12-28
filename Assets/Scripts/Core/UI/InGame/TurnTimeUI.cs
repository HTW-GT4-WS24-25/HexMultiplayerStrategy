using Core.GameEvents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.InGame
{
    public class TurnTimeUI : MonoBehaviour
    {
        [SerializeField] private Image roundTimerImage;
        [SerializeField] private TextMeshProUGUI roundTimerText;
        [SerializeField] private Color dayColor;
        [SerializeField] private Color nightColor;

        private void OnEnable()
        {
            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleCycleStateSwitch;
        }

        private void OnDisable()
        {
            ClientEvents.DayNightCycle.OnSwitchedCycleState -= HandleCycleStateSwitch;
        }
    
        public void UpdateTurnTime(float newTurnTime, float maxTurnTime)
        {
            roundTimerImage.fillAmount = 1 - newTurnTime / maxTurnTime;
            roundTimerText.text = ((int)maxTurnTime - Mathf.FloorToInt(newTurnTime)).ToString();
        }

        private void HandleCycleStateSwitch(DayNightCycle.DayNightCycle.CycleState newCycleState)
        {
            roundTimerImage.color = newCycleState == DayNightCycle.DayNightCycle.CycleState.Day ? dayColor : nightColor;
        }
    }
}