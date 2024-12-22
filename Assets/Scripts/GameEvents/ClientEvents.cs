using HexSystem;
using UI.NightShop;
using Unit;
using UnityEngine;
using UnityEngine.Events;

namespace GameEvents
{
    public static class ClientEvents
    {
        public static readonly HexagonEvents Hexagon = new ();
        public static readonly NightShopEvents NightShop = new ();
        public static readonly InputEvents Input = new();
        public static readonly UnitEvents Unit = new();
        public static readonly DayNightCycleEvents DayNightCycle = new();
    
        public class HexagonEvents
        {
            public UnityAction OnHideValidHexagonsForPlacement;
        }

        public class NightShopEvents
        {
            public UnityAction<Card> OnCardSelected;
            public UnityAction OnCardDeselected;
            public UnityAction<int> OnMoneyAmountChanged;
            public UnityAction<bool> OnLocalPlayerChangedReadyForDawnState;
            public UnityAction<int, int> OnReadyPlayersChanged;
            public UnityAction<AxialCoordinates, int> OnUnitPlacementCommand;
        }
    
        public class InputEvents
        {
            public UnityAction<Hexagon> OnHexSelectedForUnitSelectionOrMovement;
            public UnityAction<Hexagon> OnHexSelectedDuringNightShop;
            public UnityAction<float> OnZoomInput;
            public UnityAction<Vector2> OnDragInput;
            public UnityAction<Vector2> OnCameraMoveInput;
            public UnityAction<float> OnCameraTurnInput;
        }

        public class DayNightCycleEvents
        {
            public UnityAction<DayNightCycle.CycleState> OnSwitchedCycleState;
        }

        public class UnitEvents
        {
            public UnityAction<UnitGroup> OnUnitGroupSelected;
            public UnityAction OnUnitGroupDeselected;
            public UnityAction<int> OnUnitSelectionSliderUpdate;
            public UnityAction<int> OnUnitCountOfSelectedChanged;
        }
    }
}

