using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Core.UI.Lobby
{
    public class ColorSelectionField : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Image colorField;
        [SerializeField] private GameObject unavailableMarking;
        [SerializeField] private GameObject selectedMarking;

        public static event Action<ColorSelectionField> OnClicked;
        
        private bool _isAvailable = true;

        public void SetColor(Color color)
        {
            colorField.color = color;
        }

        public void SetAvailable(bool isAvailable)
        {
            unavailableMarking.SetActive(!isAvailable);
            _isAvailable = isAvailable;
        }

        public void SetAsSelected(bool isSelected)
        {
            selectedMarking.SetActive(isSelected);
            transform.localScale = isSelected ? Vector3.one * 1.1f : Vector3.one;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isAvailable)
                return;
            
            OnClicked?.Invoke(this);
        }
    }
}
