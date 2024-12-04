using System.Collections.Generic;

namespace HexSystem
{
    public class HexagonData
    {
        public HexType HexType { get; set; }
        public AxialCoordinates Coordinates { get; set; }
        public List<ulong> UnitsOnHex { get; } = new();
        public ulong? StationaryUnitGroup { get; set; }
        public bool IsWarGround { get; set; }
        public ulong? ControllerPlayerId { get; set; }

        public HexagonData(AxialCoordinates coordinates)
        {
            Coordinates = coordinates;
        }
    }
}