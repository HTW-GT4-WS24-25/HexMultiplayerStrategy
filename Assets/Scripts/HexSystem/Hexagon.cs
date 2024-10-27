using System.Collections.Generic;
using Unit;
using UnityEngine;
using UnityEngine.Serialization;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        public bool isTraversable;
        public List<UnitGroup> unitGroups = new();
        
        public AxialCoordinate Coordinates { get; private set; }
        public UnitGroup StationaryUnitGroup { get; private set; }

        public void Initialize(AxialCoordinate coordinate)
        {
            Coordinates = coordinate;
        }

        public void ChangeUnitGroupOnHexToStationary(UnitGroup unitGroup)
        {
            Debug.Assert(unitGroups.Contains(unitGroup), "Tried to make a UnitGroup stationary that was not added to the hex before.");
            Debug.Assert(StationaryUnitGroup == null, "Tried to make a UnitGroup on a hex, that already has a stationary UnitGroup.");

            StationaryUnitGroup = unitGroup;
        }

        public void RemoveStationaryUnitGroup()
        {
            StationaryUnitGroup = null;
        }
    }
}