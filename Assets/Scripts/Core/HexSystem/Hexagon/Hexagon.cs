using Core.GameEvents;
using Core.HexSystem.VFX;
using Core.Player;
using UnityEngine;

namespace Core.HexSystem.Hexagon
{
    public class Hexagon : MonoBehaviour
    {
        [SerializeField] private HexBorderLine borderLine;
        [SerializeField] private HexHighlighting highlighting;
        [SerializeField] private HexConqueringEffect conqueringEffect;
        
        public bool isTraversable;
        public AxialCoordinates Coordinates { get; private set; }
        
        private Topping _topping;
        private DayNightCycle.DayNightCycle.CycleState _currentCycleState;

        private void OnEnable()
        {
            ClientEvents.DayNightCycle.OnSwitchedCycleState += HandleDayNightCycleSwitch;
            ClientEvents.Hexagon.OnHideValidHexagonsForPlacement += DisableHighlight;
        }

        private void OnDisable()
        {
            ClientEvents.DayNightCycle.OnSwitchedCycleState -= HandleDayNightCycleSwitch;
            ClientEvents.Hexagon.OnHideValidHexagonsForPlacement -= DisableHighlight;
        }

        public void Initialize(AxialCoordinates coordinate)
        {
            Coordinates = coordinate;
            
            if (borderLine != null)
                borderLine.Initialize();
        }

        public void SetTopping(Topping toppingPrefab)
        {
            if (_topping != null)
                Destroy(_topping.gameObject);
            
            if (toppingPrefab != null)
                _topping = Instantiate(toppingPrefab, transform.position, QuaternionUtils.GetRandomHexRotation());
        }

        public void SetToppingLevel(int newLevel)
        {
            _topping.SetLevel(newLevel);
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

        public void HighlightOnMouseOver()
        {
            if (_currentCycleState == DayNightCycle.DayNightCycle.CycleState.Day)
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
            if (_currentCycleState == DayNightCycle.DayNightCycle.CycleState.Day)
            {
                highlighting.DisableHighlight(); 
            }
            else
            {
                highlighting.DisableMouseOverNightHighlight();
            }
        }
        
        private void HandleDayNightCycleSwitch(DayNightCycle.DayNightCycle.CycleState cycleState)
        {
            _currentCycleState = cycleState;
            DisableHighlight();
        }

        private void DisableHighlight()
        {
            highlighting.DisableHighlight();
        }
    }
}