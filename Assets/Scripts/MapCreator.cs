using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using HexSystem;
using Unit;
using Unity.Netcode;
using UnityEngine.Serialization;

public class MapCreator : NetworkBehaviour
{
    [Header("References")] 
    [SerializeField] private Hexagon grassTilePrefab;
    [SerializeField] private Hexagon mountainTilePrefab;
    
    public HexagonGrid Grid;

    [Header("Settings")]
    [SerializeField] private float spacing;
    [Range(0f, 1f)]
    [SerializeField] private float mountainChance;

    public const float TileWidth = 1;
    
    private float _horizontalSpacing;
    private float _verticalSpacing;

    private Vector3 _qOffset;
    private Vector3 _rOffset;
    
    private void Awake()
    {
        _horizontalSpacing = TileWidth + spacing;

        var tileHeight = 2 * TileWidth / Mathf.Sqrt(3) + spacing;
        _verticalSpacing = 0.75f * tileHeight + spacing;

        _qOffset = new Vector3(_horizontalSpacing, 0, 0);
        _rOffset = new Vector3(0.5f * _horizontalSpacing, 0, _verticalSpacing);

        Grid = new HexagonGrid();
    }

    public int[] GenerateRandomMapData(int ringAmount)
    {
        var hexValues = new List<int>();
        for (var ring = 0; ring <= ringAmount; ring++)
        {
            for (var q = -ring; q <= ring; q++)
            {
                for (var r = -ring; r <= ring; r++)
                {
                    var s = -q - r;
                    
                    if (Math.Abs(q) > ring || Math.Abs(r) > ring || Math.Abs(s) > ring) continue;
                    if (Math.Abs(q) != ring && Math.Abs(r) != ring && Math.Abs(s) != ring) continue;
                    
                    hexValues.Add(Random.Range(0, 1f) <= mountainChance ? (int)HexType.Mountain : (int)HexType.Grass);
                }
            }
        }
        
        return hexValues.ToArray();
    }

    public void BuildMap(int[] mapData, int ringAmount)
    {
        BuildMapClientRpc(mapData, ringAmount);
    }

    [ClientRpc]
    private void BuildMapClientRpc(int[] mapData, int ringAmount)
    {
        var rotation180 = Quaternion.Euler(0, 180, 0);
        var dataIterator = 0;
        
        for (var ring = 0; ring <= ringAmount; ring++)
        {
            for (var q = -ring; q <= ring; q++)
            {
                for (var r = -ring; r <= ring; r++)
                {
                    var s = -q - r;
                    
                    if (Math.Abs(q) > ring || Math.Abs(r) > ring || Math.Abs(s) > ring) continue;
                    if (Math.Abs(q) != ring && Math.Abs(r) != ring && Math.Abs(s) != ring) continue;

                    var coord = new AxialCoordinates(q, r);
                    var hexPosition = _qOffset * q + _rOffset * r;
                    
                    var hexPrefab = mapData[dataIterator++] == (int)HexType.Mountain ? mountainTilePrefab : grassTilePrefab;
                    var randomHexRotation = Random.Range(0, 2) == 0 ? rotation180 : Quaternion.identity;
                    
                    var newHex = Instantiate(hexPrefab, hexPosition, randomHexRotation, transform);
                    newHex.Initialize(coord);
                    Grid.Add(newHex);
                }
            }
        }
    }

    private enum HexType
    {
        Grass = 0,
        Mountain = 1
    }
}
