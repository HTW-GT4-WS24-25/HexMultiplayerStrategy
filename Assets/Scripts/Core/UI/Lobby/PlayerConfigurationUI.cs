using System;
using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Core.Unit.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.UI.Lobby
{
    public class PlayerConfigurationUI : MonoBehaviour
    {
        [SerializeField] private LobbyUI lobbyUI;
        [SerializeField] private TMP_Dropdown factionSelectionDropdown;
        [SerializeField] private ColorSelectionField colorSelectionFieldPrefab;
        [SerializeField] private Transform colorSelectionFieldContainer;
        [SerializeField] private Button continueButton;

        private readonly Dictionary<PlayerColor.ColorType, ColorSelectionField> _colorSelectionFieldsByType = new();
        private PlayerColor.ColorType _selectedPlayerColor = PlayerColor.ColorType.None;
        private Dictionary<string, UnitModel.ModelType> _modelTypesByName = new();

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
            var factions = Enum.GetValues(typeof(UnitModel.ModelType));
            foreach (var faction in factions)
            {
                _modelTypesByName.Add(faction.ToString(), (UnitModel.ModelType)faction);
            }
            var factionOptions = _modelTypesByName.Select(factionData => new TMP_Dropdown.OptionData(factionData.Key)).ToList();
            factionSelectionDropdown.options = factionOptions;
            
            var playerColors = PlayerColor.GetAll();
            foreach (var playerColor in playerColors)
            {
                if (playerColor.Type == PlayerColor.ColorType.None)
                    continue;

                var newColorSelectionField = Instantiate(colorSelectionFieldPrefab, colorSelectionFieldContainer);
                newColorSelectionField.SetColor(playerColor.BaseColor);
                _colorSelectionFieldsByType.Add(playerColor.Type, newColorSelectionField);
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
                if (unavailableColor == PlayerColor.ColorType.None)
                    continue;

                _colorSelectionFieldsByType[unavailableColor].SetAvailable(false);
            }

            if (unavailableColors.Contains(_selectedPlayerColor))
                Unselect();
        }

        private void SelectPlayerColor(ColorSelectionField selectedColorField)
        {
            if (_selectedPlayerColor != PlayerColor.ColorType.None)
                _colorSelectionFieldsByType[_selectedPlayerColor].SetAsSelected(false);

            var newColorSelection = _colorSelectionFieldsByType.First(field => field.Value == selectedColorField);
            _selectedPlayerColor = newColorSelection.Key;
            selectedColorField.SetAsSelected(true);

            continueButton.interactable = true;
        }

        public void SubmitPlayerSelection()
        {
            var selectedPlayerFactionName = factionSelectionDropdown.options[factionSelectionDropdown.value].text;
            var selectedPlayerFaction = _modelTypesByName[selectedPlayerFactionName];
            lobbyUI.SubmitPlayerSelection(_selectedPlayerColor, selectedPlayerFaction);
        }

        private void Unselect()
        {
            if (_selectedPlayerColor != PlayerColor.ColorType.None)
            {
                var unselectColorField = _colorSelectionFieldsByType[_selectedPlayerColor];
                unselectColorField.SetAvailable(false);
                unselectColorField.SetAsSelected(false);
            }

            _selectedPlayerColor = PlayerColor.ColorType.None;
            continueButton.interactable = false;
        }
    }
}