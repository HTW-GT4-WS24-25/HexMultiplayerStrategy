using System.Linq;
using Core.Buildings;
using Core.GameEvents;
using Core.HexSystem.Hex;

namespace Core.NightShop.Placeables
{
    public abstract class BuildingPlaceable : Placeable
    {
        protected abstract HexType[] ValidHexTypes { get; }
        protected abstract BuildingType BuildingType { get; }
        protected ulong PlayerId;
        
        public override void Initialize(ulong playerId)
        {
            PlayerId = playerId;
        }

        public override bool IsHexValidForPlacement(HexagonData hexagon)
        {
            var isControlledByPlayer = hexagon.ControllerPlayerId == PlayerId;
            var isValidHexType = ValidHexTypes.Contains(hexagon.HexType);
            
            if (!isControlledByPlayer || !isValidHexType) 
                return false;

            if (hexagon.Building == null)
                return true;
            
            return hexagon.Building.Type == BuildingType && hexagon.Building.CanBeUpgraded;
        }

        public override void Place(HexagonData hexagon)
        {
            if (hexagon.Building == null)
                ClientEvents.NightShop.OnBuildingPlacementCommand?.Invoke(hexagon.Coordinates, BuildingType);
            else
                ClientEvents.NightShop.OnBuildingUpgradeCommand?.Invoke(hexagon.Coordinates);
        }
    }
}