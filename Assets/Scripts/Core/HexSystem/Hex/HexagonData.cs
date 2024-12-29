using System.Collections.Generic;
using Core.Buildings;

namespace Core.HexSystem.Hex
{
    public class HexagonData
    {
        public readonly HexType HexType;
        public readonly AxialCoordinates Coordinates;
        
        public Building Building { get; set; }
        public List<ulong> UnitsOnHex { get; } = new();
        public ulong? StationaryUnitGroup { get; set; }
        public ulong? ControllerPlayerId { get; set; }

        public HexagonData(AxialCoordinates coordinates, HexType type)
        {
            Coordinates = coordinates;
            HexType = type;
        }
    }
}