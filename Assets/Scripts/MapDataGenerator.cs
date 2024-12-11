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
    
    public int[] Generate(int nRings, List<AxialCoordinates> hexagonsToAvoid = null)
    {
        var map = new Dictionary<AxialCoordinates, ToppingType>();

        ProceduralHexGridUtils.FillMapWithHexes(map, nRings);
        GenerateMountains(map, hexagonsToAvoid);
        GenerateForests(map, hexagonsToAvoid);
        
        return map.ToIntArray(nRings);
    }

    private void GenerateMountains(
        Dictionary<AxialCoordinates, ToppingType> map, 
        List<AxialCoordinates> hexagonsToAvoid = null)
    {
        var mountainCoords = ProceduralHexGridUtils.SelectHexagons(
            map, 
            _config.mountainProbability,
            _config.mountainProbabilityEnforcement,
            _config.mountainAreaCheckingSize,
            hexagonsToAvoid: hexagonsToAvoid);

        foreach (var mountainCoord in mountainCoords) map[mountainCoord] = ToppingType.Mountain;
    }

    private void GenerateForests(
        Dictionary<AxialCoordinates, ToppingType> map,
        List<AxialCoordinates> hexagonsToAvoid = null)
    {
        var srcForestCoords = ProceduralHexGridUtils.SelectHexagons(
            map, 
            _config.forestProbability,
            _config.forestProbabilityEnforcement,
            _config.forestAreaCheckingSize,
            hexagonsToAvoid: hexagonsToAvoid);
        
        foreach (var srcForestCords in srcForestCoords)
        {
            var forestCoords = ProceduralHexGridUtils.SelectSurrounding(
                map, 
                srcForestCords,
                _config.diminishingProbabilityPerDistance,
                new List<ToppingType> { ToppingType.Mountain },
                hexagonsToAvoid: hexagonsToAvoid);

            foreach (var forestCoord in forestCoords) map[forestCoord] = ToppingType.Forest;
        }
    }
}