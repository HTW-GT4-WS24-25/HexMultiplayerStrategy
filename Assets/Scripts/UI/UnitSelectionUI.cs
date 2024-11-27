using System;
using GameEvents;
using TMPro;
using Unit;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitSelectionUI : MonoBehaviour
    {
        [SerializeField] private Slider unitSlider;
        [SerializeField] private TextMeshProUGUI unitAmountText;
        [SerializeField] private UnitController unitController;

        private void OnEnable()
        {
            ClientEvents.Unit.OnUnitCountOfSelectedChanged += SetSliderMaximum;
        }

        private void OnDisable()
        {
            ClientEvents.Unit.OnUnitCountOfSelectedChanged -= SetSliderMaximum;
        }
        
        public void UpdateUnitCount(float count)
        {
            ClientEvents.Unit.OnUnitSelectionSliderUpdate.Invoke((int)count);
            unitAmountText.text = count.ToString("0");
        }
        
        public void SetSliderMaximumAndValue(int unitCount)
        {
            SetSliderMaximum(unitCount);
            unitSlider.value = unitCount;
        }

        private void SetSliderMaximum(int unitCount)
        {
            unitSlider.maxValue = unitCount;
            unitSlider.value = Math.Min(unitSlider.value, unitCount);
        }
    }
}
