using System;
using System.Collections.Generic;
using Unit;
using UnityEngine;

namespace HexSystem
{
    [RequireComponent(typeof(ClientHexagon))]
    public class ServerHexagon : MonoBehaviour
    {
        public UnitGroup StationaryUnitGroup { get; private set; }
        public List<UnitGroup> UnitGroups { get; private set; } = new();
        public ClientHexagon ClientHexagon { get; private set; }
        public ulong ControllingPlayerId { get; set; }
        
        public AxialCoordinates Coordinates => ClientHexagon.Coordinates;

        private void Awake()
        {
            ClientHexagon = GetComponent<ClientHexagon>();
        }

        public void ChangeUnitGroupOnHexToStationary(UnitGroup unitGroup)
        {
            Debug.Assert(UnitGroups.Contains(unitGroup), "Tried to make a UnitGroup stationary that was not added to the hex before.");
            Debug.Assert(StationaryUnitGroup == null, "Tried to make a UnitGroup on a hex, that already has a stationary UnitGroup.");

            StationaryUnitGroup = unitGroup;
        }

        public void RemoveStationaryUnitGroup()
        {
            StationaryUnitGroup = null;
        }
    }
}