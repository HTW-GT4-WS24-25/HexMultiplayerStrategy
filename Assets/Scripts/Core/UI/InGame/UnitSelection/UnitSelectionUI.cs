using System;
using Core.GameEvents;
using Core.Unit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.InGame.UnitSelection
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
            var intCount = (int)count;
            ClientEvents.Unit.OnUnitSelectionSliderUpdate.Invoke(intCount);
            unitAmountText.text = intCount.ToString("0");
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
            
            if(unitCount == 1)
                gameObject.SetActive(false);
        }
    }
}
