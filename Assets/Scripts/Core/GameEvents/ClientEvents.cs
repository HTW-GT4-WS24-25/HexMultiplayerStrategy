using Core.Buildings;
using Core.HexSystem;
using Core.HexSystem.Hex;
using Core.UI.InGame.NightShop;
using Core.Unit.Group;
using UnityEngine;
using UnityEngine.Events;

namespace Core.GameEvents
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
            public UnityAction<AxialCoordinates, BuildingType> OnBuildingPlacementCommand;
            public UnityAction<AxialCoordinates> OnBuildingUpgradeCommand;
        }
    
        public class InputEvents
        {
            public UnityAction<Hexagon> OnHexSelectedForUnitMovement;
            public UnityAction<UnitGroup> OnUnitGroupSelected;
            public UnityAction<Hexagon> OnHexSelectedDuringNightShop;
            public UnityAction<float> OnZoomInput;
            public UnityAction<Vector2> OnDragInput;
            public UnityAction<Vector2> OnCameraMoveInput;
            public UnityAction<float> OnCameraTurnInput;
            public UnityAction PauseTogglePressed;
        }

        public class DayNightCycleEvents
        {
            public UnityAction<DayNightCycle.DayNightCycle.CycleState> OnSwitchedCycleState;
        }

        public class UnitEvents
        {
            public UnityAction OnUnitGroupDeselected;
            public UnityAction<int> OnUnitSelectionSliderUpdate;
            public UnityAction<int> OnUnitCountOfSelectedChanged;
        }
    }
}

