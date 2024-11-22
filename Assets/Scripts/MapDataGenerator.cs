using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;
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
        var map = new Dictionary<AxialCoordinates, HexType>();

        ProceduralHexGridUtils.FillMapWithHexes(map, nRings);
        GenerateMountains(map);
        GenerateForests(map);
        
        return map.ToIntArray(nRings);
    }

    private void GenerateMountains(Dictionary<AxialCoordinates, HexType> map)
    {
        var mountainCoords = ProceduralHexGridUtils.SelectHexagons(
            map, 
            _config.mountainProbability,
            _config.mountainProbabilityEnforcement,
            _config.mountainAreaCheckingSize);

        foreach (var mountainCoord in mountainCoords) map[mountainCoord] = HexType.Mountain;
    }

    private void GenerateForests(Dictionary<AxialCoordinates, HexType> map)
    {
        var srcForestCoords = ProceduralHexGridUtils.SelectHexagons(
            map, 
            _config.forestProbability,
            _config.forestProbabilityEnforcement,
            _config.forestAreaCheckingSize
        );
        
        foreach (var srcForestCords in srcForestCoords)
        {
            var forestCoords = ProceduralHexGridUtils.SelectSurrounding(
                map, 
                srcForestCords,
                _config.diminishingProbabilityPerDistance,
                new List<HexType> { HexType.Mountain });

            foreach (var forestCoord in forestCoords) map[forestCoord] = HexType.Forest;
        }
    }
}