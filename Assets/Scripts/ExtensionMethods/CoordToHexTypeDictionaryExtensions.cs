using System.Collections.Generic;
using System.Linq;
using Core.HexSystem;
using Core.HexSystem.Hex;

namespace ExtensionMethods
{
    public static class CoordToHexTypeDictionaryExtensions
    {
        public static int[] ToIntArray(this Dictionary<AxialCoordinates, HexType> map)
        {
            var nRings = map.Keys.Max(coord => coord.GetDistanceTo(AxialCoordinates.Zero));
            return ToIntArray(map, nRings);
        }
        
        public static int[] ToIntArray(this Dictionary<AxialCoordinates, HexType> map, int nRings)
        {
            return HexagonGrid.GetHexRingsAroundCoordinates(AxialCoordinates.Zero, nRings)
                .Select(coord => (int)map[coord]).ToArray();
        }
    }
}