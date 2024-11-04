using System;
using HexSystem;
using Networking.Host;
using Player;
using Unit;
using Unity.Netcode;
using UnityEngine;

public class UnitPlacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private UnitGroup unitGroupPrefab;

    private HexagonGrid _hexGrid;
    private int _unitAmount;

    public void SetPlacementAmount(int amount)
    {
        _unitAmount = amount;
    }

    public bool TryAddUnitsToHex(Hexagon hex, PlayerDataStorage.PlayerData playerData)
    {
        if (hex == null)
            return false;
        
        // TODO: Check if hex is owned by player and not at war

        if (hex.StationaryUnitGroup != null)
        {
            AddUnitsToUnitGroup(hex.StationaryUnitGroup);
        }
        else
        {
            PlaceUnitGroupOnHex(hex, playerData.ClientId);
        }

        return true;
    }

    private void AddUnitsToUnitGroup(UnitGroup unitGroup)
    {
        unitGroup.AddUnits(_unitAmount);
    }

    private void PlaceUnitGroupOnHex(Hexagon hex, ulong playerId)
    {
        var newUnitGroup = Instantiate(unitGroupPrefab, hex.transform.position, Quaternion.identity);
        var newUnitGroupNetworkObject = newUnitGroup.GetComponent<NetworkObject>();
        newUnitGroupNetworkObject.Spawn();
        
        newUnitGroup.Initialize(hex, _unitAmount, playerId);
        
        hex.ChangeUnitGroupOnHexToStationary(newUnitGroup);
    }
}