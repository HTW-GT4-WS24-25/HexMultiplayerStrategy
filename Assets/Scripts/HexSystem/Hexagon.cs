using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        [SerializeField] private HexBorderLine hexBorderLine;
        public bool isTraversable;
        public List<UnitGroup> unitGroups = new();
        
        public AxialCoordinate Coordinates { get; private set; }
        public UnitGroup StationaryUnitGroup { get; private set; }
        public Color DominanceColor { get; private set; } = Color.black; //TODO replace with Player Reference

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

        public void UpdateDominance(Color dominanceColor)
        {
            DominanceColor = dominanceColor;
            hexBorderLine.HighlightBorderWithColor(dominanceColor);
        }
    }
}