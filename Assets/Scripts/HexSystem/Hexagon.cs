using System;
using GameEvents;
using Player;
using UnityEngine;
using Utils;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        [SerializeField] private HexBorderLine hexBorderLine;
        [SerializeField] private HexHighlighting hexHighlighting;
        
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
            
            if (hexBorderLine != null)
                hexBorderLine.Initialize();
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
            hexBorderLine.HighlightBorderWithColor(playerColor);
        }

        public void HighlightAsValidForPlacement()
        {
            hexHighlighting.ApplyAvailabilityHighlightNight();
        }

        public void UnhighlightAsValidForPlacement()
        {
            hexHighlighting.DisableAvailabilityHighlight();
        }

        public void HighlightOnMouseOver()
        {
            if (_currentCycleState == DayNightCycle.CycleState.Day)
            {
                hexHighlighting.ApplyMouseOverHighlightDay(); 
            }
            else
            {
                hexHighlighting.ApplyMouseOverHighlightNight();
            }
        }

        public void DisableMouseOverHighlight()
        {
            hexHighlighting.DisableMouseOverHighlight();
        }
        
        private void HandleDayNightCycleSwitch(DayNightCycle.CycleState cycleState)
        {
            _currentCycleState = cycleState;
            if(_currentCycleState == DayNightCycle.CycleState.Day)
                UnhighlightAsValidForPlacement();
        }
    }
}