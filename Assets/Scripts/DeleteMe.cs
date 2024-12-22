using System;
using Buildings;
using ExtensionMethods;
using HexSystem;
using Sirenix.OdinInspector;
using UnityEngine;

public class DeleteMe : MonoBehaviour
{
    [SerializeField] GridData gridData;
    [SerializeField] private MapBuilder mapBuilder;
    [SerializeField] private ToppingCollection toppingCollection;


    [Button]
    public void SetHexTopping(Vector2Int hexCoordinates, BuildingType buildingType, int level)
    {
        var axials = hexCoordinates.ToAxialCoordinates();
        var hexData = gridData.GetHexagonDataOnCoordinate(axials);
        //hexData.Building.Type = buildingType; // do this on Server
        var prefab = toppingCollection.GetToppingPrefab(hexData.HexType, buildingType, level);
        mapBuilder.Grid.Get(axials).SetTopping(prefab); // do this on Client
    }
}