using HexSystem;
using UnityEngine;
using UnityEngine.Events;

namespace GameEvent
{
    public static class GameEvents
    {
        public static readonly DemoEvents Demo = new();
        public static readonly UnitEvents Unit = new();

        public class DemoEvents{
            public UnityAction<float> DemoEvent;
        }

        public class UnitEvents
        {
            public UnityAction<Unit, AxialCoordinate> OnUnitNextHexReached;
        }
    }
}
