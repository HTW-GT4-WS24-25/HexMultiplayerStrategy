using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Lobby
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

        public void SetSelectedMarking(bool isSelected)
        {
            selectedMarking.SetActive(isSelected);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isAvailable)
                return;
            
            OnClicked?.Invoke(this);
        }
    }
}
