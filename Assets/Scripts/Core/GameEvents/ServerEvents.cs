using Core.HexSystem;
using Core.HexSystem.Hex;
using Core.Unit.Group;
using Unity.Collections;
using UnityEngine.Events;

namespace Core.GameEvents
{
    public static class ServerEvents
    {
        public static readonly PlayerEvents Player = new ();
        public static readonly DayNightCycleEvents DayNightCycle = new();
        public static readonly NightShopEvents NightShop = new();
        public static readonly UnitEvents Unit = new();

        public class PlayerEvents
        {
            public UnityAction<ulong, FixedString32Bytes> OnPlayerConnected;
            public UnityAction<ulong, int> OnPlayerColorChanged;
            public UnityAction<AxialCoordinates, ulong> OnInitialPlayerUnitsPlaced;
            public UnityAction<ulong, int> OnPlayerScoreChanged;
        }
    
        public class DayNightCycleEvents
        {
            public UnityAction OnTurnEnded;
            public UnityAction OnGameEnded;
        }

        public class NightShopEvents
        {
            public UnityAction OnAllPlayersReadyForDawn;
        }

        public class UnitEvents
        {
            public UnityAction<ulong> OnUnitGroupWithIdDeleted;
            public UnityAction<UnitGroup, AxialCoordinates> OnUnitGroupReachedHexCenter;
            public UnityAction<UnitGroup, AxialCoordinates> OnUnitGroupReachedNewHex;
            public UnityAction<UnitGroup> OnUnitGroupLeftHexCenter;
            public UnityAction<UnitGroup, UnitGroup> OnCombatTriggered;
            public UnityAction<UnitGroup> OnUnitGroupShouldReceiveMoveSpeedUpdate;
            public UnityAction<HexagonData, int> OnUnitsShouldBeSpawnedOnBarrackHex;
        }
    }
}