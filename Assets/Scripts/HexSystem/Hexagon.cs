using System;
using GameEvents;
using Player;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        [SerializeField] private HexBorderLine borderLine;
        [SerializeField] private HexHighlighting highlighting;
        [SerializeField] private HexConqueringEffect conqueringEffect;
        
        public bool isTraversable;
        public AxialCoordinates Coordinates { get; private set; }
        
        private GameObject _topping;
        private DayNightCycle.CycleState _currentCycleState;

        private void OnEnable()
        {
            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleDayNightCycleSwitch;
        }

        private void OnDisable()
        {
            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleDayNightCycleSwitch;
        }

        public void Initialize(AxialCoordinates coordinate)
        {
            Coordinates = coordinate;
            
            if (borderLine != null)
                borderLine.Initialize();
        }

        public void SetTopping(GameObject topping)
        {
            if (_topping != null)
                Destroy(_topping.gameObject);
            
            _topping = topping;
            
            if (_topping != null)
                Instantiate(_topping, transform.position, QuaternionUtils.GetRandomHexRotation());
        }

        public void AdaptBorderToPlayerColor(PlayerColor playerColor)
        {
            borderLine.HighlightBorderWithColor(playerColor);
            conqueringEffect.PlayConqueringEffect(playerColor.BaseColor);
        }

        public void HighlightAsValidForPlacement()
        {
            highlighting.ApplyAvailabilityHighlightNight();
        }

        public void UnhighlightAsValidForPlacement()
        {
            highlighting.DisableHighlight();
        }

        public void HighlightOnMouseOver()
        {
            if (_currentCycleState == DayNightCycle.CycleState.Day)
            {
                highlighting.ApplyMouseOverHighlightDay(); 
            }
            else
            {
                highlighting.ApplyMouseOverHighlightNight();
            }
        }

        public void DisableMouseOverHighlight()
        {
            if (_currentCycleState == DayNightCycle.CycleState.Day)
            {
                highlighting.DisableHighlight(); 
            }
            else
            {
                highlighting.DisableMouseOverNightHighlight();
            }
        }
        
        private void HandleDayNightCycleSwitch(DayNightCycle.CycleState cycleState)
        {
            _currentCycleState = cycleState;
            if(_currentCycleState == DayNightCycle.CycleState.Day)
                UnhighlightAsValidForPlacement();
        }
    }
}