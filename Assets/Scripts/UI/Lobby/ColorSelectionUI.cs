using System;
using System.Collections.Generic;
using System.Linq;
using Player;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Lobby
{
    public class ColorSelectionUI : MonoBehaviour
    {
        [SerializeField] private LobbyUI lobbyUI;
        [SerializeField] private ColorSelectionField colorSelectionFieldPrefab;
        [SerializeField] private Transform colorSelectionFieldContainer;
        [SerializeField] private Button continueButton;
        
        private readonly Dictionary<PlayerColor.ColorType, ColorSelectionField> _colorSelectionFieldsByType = new();
        private PlayerColor.ColorType _selectedPlayerColor = PlayerColor.ColorType.None;

        private void OnEnable()
        {
            ColorSelectionField.OnClicked += SelectPlayerColor;
        }
        
        private void OnDisable()
        {
            ColorSelectionField.OnClicked -= SelectPlayerColor;
        }

        public void Initialize()
        {
            var playerColors = PlayerColor.GetAll();
            foreach (var playerColor in playerColors)
            {
                if(playerColor.colorType == PlayerColor.ColorType.None)
                    continue;
                
                var newColorSelectionField = Instantiate(colorSelectionFieldPrefab, colorSelectionFieldContainer);
                newColorSelectionField.SetColor(playerColor.baseColor);
                _colorSelectionFieldsByType.Add(playerColor.colorType, newColorSelectionField);
            }
        }

        public void SetUnavailableColors(List<PlayerColor.ColorType> unavailableColors)
        {
            foreach (var (_, colorSelectionField) in _colorSelectionFieldsByType)
            {
                colorSelectionField.SetAvailable(true);
            }

            foreach (var unavailableColor in unavailableColors)
            {
                if(unavailableColor == PlayerColor.ColorType.None)
                    continue;
                
                _colorSelectionFieldsByType[unavailableColor].SetAvailable(false);
            }
            
            if(unavailableColors.Contains(_selectedPlayerColor))
                Unselect();
        }

        private void SelectPlayerColor(ColorSelectionField selectedColorField)
        {
            var newColorSelection = _colorSelectionFieldsByType.First(field => field.Value == selectedColorField);
            _selectedPlayerColor = newColorSelection.Key;
            selectedColorField.SetSelectedMarking(true);

            continueButton.interactable = true;
        }

        public void SubmitColorSelection()
        {
            lobbyUI.SubmitPlayerColorSelection(_selectedPlayerColor);
        }
        
        private void Unselect()
        {
            if(_selectedPlayerColor != PlayerColor.ColorType.None)
                _colorSelectionFieldsByType[_selectedPlayerColor].SetAvailable(false);
            
            _selectedPlayerColor = PlayerColor.ColorType.None;
            continueButton.interactable = false;
        }
    }
}
