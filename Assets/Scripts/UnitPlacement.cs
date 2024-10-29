using System;
using HexSystem;
using Unit;
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

    public bool TryAddUnitsToHex(Hexagon hex)
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
            PlaceUnitGroupOnHex(hex);
        }

        return true;
    }

    private void AddUnitsToUnitGroup(UnitGroup unitGroup)
    {
        unitGroup.ChangeUnitCount(_unitAmount);
    }

    private void PlaceUnitGroupOnHex(Hexagon hex)
    {
        var newUnitGroup = Instantiate(unitGroupPrefab, hex.transform.position, Quaternion.identity);
        newUnitGroup.Initialize(hex, _unitAmount);
        
        hex.ChangeUnitGroupOnHexToStationary(newUnitGroup);
    }
}