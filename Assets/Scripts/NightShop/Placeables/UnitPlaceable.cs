using HexSystem;
using Networking.Host;
using UnityEngine;

namespace NightShop.Placeables
{
    [CreateAssetMenu(fileName = "UnitPlaceable", menuName = "Placeables/Units/UnitPlaceable", order = 0)]
    public class UnitPlaceable : Placeable
    {
        private ulong _playerId;

        public override void Initialize(ulong playerId)
        {
            _playerId = playerId;
        }

        public override bool IsHexValidForPlacement(HexagonData hexagon)
        {
            return hexagon.ControllerPlayerId == _playerId;
        }

        public override void Place(HexagonData hexagon)
        {
            const int unitCountToPlace = 1; // Maybe get this value from playerData in the future
            GameEvents.ClientEvents.NightShop.OnUnitPlacementCommand?.Invoke(hexagon.Coordinates, unitCountToPlace);
        }
    }
}