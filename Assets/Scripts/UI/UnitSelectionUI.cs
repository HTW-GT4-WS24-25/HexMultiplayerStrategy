using TMPro;
using Unit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitSelectionUI : MonoBehaviour
    {
        [SerializeField] private Slider unitSlider;
        [SerializeField] private TextMeshProUGUI unitAmountText;
        [SerializeField] private UnitController unitController;
        
        //Update Slider On Unit Selection
        private void OnEnable()
        {
            unitSlider.onValueChanged.AddListener(UpdateUnitCount);
        }

        private void OnDisable()
        {
            unitSlider.onValueChanged.RemoveListener(UpdateUnitCount);
        }
        
        private void UpdateUnitCount(float count)
        {
            GameEvents.UNIT.OnUnitSelectionSliderUpdate.Invoke((int)count);
            unitAmountText.text = count.ToString("0");
        }

        public void SetSliderMaximum(int unitCount)
        {
            unitSlider.maxValue = unitCount;
            unitSlider.value = unitCount;
        }

    }
}
