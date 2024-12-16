using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using HexSystem;
using Unity.Netcode;
using Utils;
using static MapDataGenerator;

public class MapBuilder : NetworkBehaviour
{
    public HexagonGrid Grid;

    [Header("Settings")]
    [SerializeField] private Hexagon defaultHexagon;
    [SerializeField] private float spacing;

    public const float TileWidth = 1;
    
    private float _horizontalSpacing;
    private float _verticalSpacing;
    private Vector3 _qOffset;
    private Vector3 _rOffset;
    private bool _initialized;

    #region Server

    public void BuildMapForAll(int[] mapData, int ringAmount)
    {
        BuildMapClientRpc(mapData, ringAmount);
    }

    #endregion

    #region Client

    [ClientRpc]
    private void BuildMapClientRpc(int[] mapData, int nRings)
    {
        if(!_initialized)
            Initialize();
        
        Grid = new HexagonGrid();
        var dataIndex = 0;
        Debug.Log("Building Map");
        
        var hexDataByType = HexTypeDataProvider.Instance.GetAllData();
        
        foreach (var coordinates in HexagonGrid.GetHexRingsAroundCoordinates(AxialCoordinates.Zero, nRings))
        {
            var hexPosition = _qOffset * coordinates.Q + _rOffset * coordinates.R;

            var hexData = hexDataByType[(HexType)mapData[dataIndex++]];
                    
            var newHex = Instantiate(hexData.HexagonPrefab, hexPosition, QuaternionUtils.GetRandomHexRotation(), transform);
            newHex.Initialize(coordinates);
            newHex.SetTopping(hexData.DefaultToppingPrefab);
            Grid.Add(newHex);
        }
    }

    private void Initialize()
    {
        _horizontalSpacing = TileWidth + spacing;

        var tileHeight = 2 * TileWidth / Mathf.Sqrt(3) + spacing;
        _verticalSpacing = 0.75f * tileHeight + spacing;

        _qOffset = new Vector3(_horizontalSpacing, 0, 0);
        _rOffset = new Vector3(0.5f * _horizontalSpacing, 0, _verticalSpacing);
    }
    
    #endregion
}
