using HexSystem;
using Unit;
using UnityEngine.Events;

public static class GameEvents
{
    public static readonly DemoEvents DEMO = new();
    public static readonly InputEvents INPUT = new();
    public static readonly DayNightCycleEvents DAY_NIGHT_CYCLE = new();
    public static readonly UnitEvents UNIT = new();

    public class DemoEvents
    {
        public UnityAction<float> DemoEvent;
    }

    public class InputEvents
    {
        public UnityAction<Hexagon> OnHexSelectedForUnitSelectionOrMovement;
        public UnityAction OnUnitGroupDeselected;
    }

    public class DayNightCycleEvents
    {
        public UnityAction OnSwitchedToNight;
        public UnityAction OnSwitchedToDay;
    }

    public class UnitEvents
    {
        public UnityAction<UnitGroup> OnUnitGroupSelected;
        public UnityAction<int> OnUnitSelectionSliderUpdate;
        public UnityAction<UnitGroup> OnUnitGroupDeleted;
    }
}