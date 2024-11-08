using HexSystem;
using UI.NightShop;
using Unit;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Events;

public static class GameEvents
{
    public static readonly InputEvents INPUT = new();
    public static readonly DayNightCycleEvents DAY_NIGHT_CYCLE = new();
    public static readonly UnitEvents UNIT = new();
    public static readonly NetworkServerEvents NETWORK_SERVER = new();
    public static readonly NightShopEvents NIGHT_SHOP = new();

    public class InputEvents
    {
        public UnityAction<ClientHexagon> OnHexSelectedForUnitSelectionOrMovement;
        public UnityAction<ClientHexagon> OnHexSelectedDuringNightShop;
        public UnityAction<float> OnZoomInput;
        public UnityAction<Vector2> OnDragInput;
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
        public UnityAction<UnitGroup> OnUnitGroupDeleted;
    }

    public class NightShopEvents
    {
        public UnityAction<Card> OnCardSelected;
        public UnityAction OnCardDeselected;
        public UnityAction<int> OnMoneySpent;
        public UnityAction<int> OnMoneyAmountChanged;
    }
    
    // These events are only meant to be called and received from the server
    public class NetworkServerEvents
    {
        public UnityAction<ulong, FixedString32Bytes> OnPlayerConnected;
        public UnityAction<ulong, int> OnPlayerColorChanged;
        public UnityAction<AxialCoordinates, ulong> OnHexChangedController;
    }
}