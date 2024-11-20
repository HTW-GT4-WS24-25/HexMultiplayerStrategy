using UnityEngine;
using Random = UnityEngine.Random;
using HexSystem;
using Unity.Netcode;
using static MapDataGenerator;

public class MapBuilder : NetworkBehaviour
{
    [Header("Hexagon Prefabs")] 
    [SerializeField] private Hexagon forestTilePrefab;
    [SerializeField] private Hexagon grassTilePrefab;
    [SerializeField] private Hexagon mountainTilePrefab;
    
    public HexagonGrid Grid;

    [Header("Settings")]
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
        var rotation180 = Quaternion.Euler(0, 180, 0);
        var dataIndex = 0;
        Debug.Log("Building Map");
        
        foreach (var coordinates in HexagonGrid.GetHexRingsAroundCoordinates(AxialCoordinates.Zero, nRings))
        {
            var hexPosition = _qOffset * coordinates.Q + _rOffset * coordinates.R;
                    
            var hexPrefab = mapData[dataIndex++] == (int)HexType.Mountain ? mountainTilePrefab : grassTilePrefab;
            var randomHexRotation = Random.Range(0, 2) == 0 ? rotation180 : Quaternion.identity;
                    
            var newHex = Instantiate(hexPrefab, hexPosition, randomHexRotation, transform);
            newHex.Initialize(coordinates);
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
