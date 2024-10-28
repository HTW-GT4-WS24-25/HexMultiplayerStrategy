using HexSystem;
using Unit;
using Unity.Collections;
using UnityEngine.Events;

public static class GameEvents
{
    public static readonly DemoEvents DEMO = new();
    public static readonly InputEvents INPUT = new();
    public static readonly DayNightCycleEvents DAY_NIGHT_CYCLE = new();
    public static readonly UnitEvents UNIT = new();
    public static readonly NetworkServerEvents NETWORK_SERVER = new();

    public class DemoEvents
    {
        public UnityAction<float> DemoEvent;
    }

    public class InputEvents
    {
        public UnityAction<Hexagon> OnHexSelectedForUnitSelectionOrMovement;
    }

    public class DayNightCycleEvents
    {
        public UnityAction OnSwitchedToNight;
        public UnityAction OnSwitchedToDay;
    }

    public class UnitEvents
    {
        public UnityAction<UnitGroup> OnUnitGroupSelected;
        public UnityAction OnUnitGroupDeselected;
        public UnityAction<int> OnUnitSelectionSliderUpdate;
        public UnityAction<UnitGroup> OnUnitGroupDeleted;
    }

    // These events are only meant to be called and received from the server
    public class NetworkServerEvents
    {
        public UnityAction<ulong, FixedString32Bytes> OnPlayerConnected;
    }
}