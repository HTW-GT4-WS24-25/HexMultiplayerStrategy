using System.Collections.Generic;
using HexSystem;
using UnityEngine;

public class MapDataGenerator
{
    private readonly MapGenerationConfig _config;
    
    public MapDataGenerator(MapGenerationConfig mapGenerationConfig)
    {
        _config = mapGenerationConfig;
    }
    
    public int[] Generate(int nRings)
    {
        var hexValues = new List<int>();

        foreach (var _ in HexagonGrid.GetHexRingsAroundCoordinates(AxialCoordinates.Zero, nRings))
        {
            var shouldBeMountain = Random.Range(0, 1f) <= _config.mountainChance;
            hexValues.Add(shouldBeMountain ? (int)HexType.Mountain : (int)HexType.Grass);
        }
        
        return hexValues.ToArray();
    }
    
    public enum HexType
    {
        Grass = 0,
        Mountain = 1
    }
}