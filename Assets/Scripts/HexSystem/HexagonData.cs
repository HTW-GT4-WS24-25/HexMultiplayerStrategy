using System.Collections.Generic;

namespace HexSystem
{
    public class HexagonData
    {
        public readonly ToppingType ToppingType;
        public readonly AxialCoordinates Coordinates;
        
        public List<ulong> UnitsOnHex { get; } = new();
        public ulong? StationaryUnitGroup { get; set; }
        public ulong? ControllerPlayerId { get; set; }

        public HexagonData(AxialCoordinates coordinates, ToppingType type)
        {
            Coordinates = coordinates;
            ToppingType = type;
        }
    }
}