using System;
using System.Collections.Generic;
using System.Linq;
using HexSystem;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProceduralHexGridUtils
{
    public static void FillMapWithHexes(
        Dictionary<AxialCoordinates, HexType> map, 
        int nRings, 
        HexType hexType = HexType.Grass)
    {
        foreach (var coord in HexagonGrid.GetHexRingsAroundCoordinates(AxialCoordinates.Zero, nRings))
        {
            map[coord] = hexType;
        }
    }
    
    public static List<AxialCoordinates> SelectHexagons(
        Dictionary<AxialCoordinates, HexType> map,
        float probability,
        float enforcement = 2f,
        int areaSize = 2,
        List<HexType> hexTypesToIgnore = null,
        List<AxialCoordinates> hexagonsToIgnore = null)
    {
        Debug.Assert(map is not null);
        Debug.Assert(probability is >= 0 and <= 1);
        Debug.Assert(enforcement is >= 0 and <= 10);
        Debug.Assert(areaSize >= 1);

        var visited = new HashSet<AxialCoordinates>();
        var selected = new HashSet<AxialCoordinates>();

        foreach (var coord in map.Keys
                     .Where(coord => hexTypesToIgnore is null || !hexTypesToIgnore.Contains(map[coord]))
                     .Where(coord => hexagonsToIgnore is null || !hexagonsToIgnore.Contains(coord)))
        {
            visited.Add(coord);

            var selectionPercentageInArea = GetSelectionPercentageInArea(coord, areaSize, visited, selected);
            var adjustedProbability = GetSelectionProbability(probability, selectionPercentageInArea, enforcement);
            var randomChoice = Random.Range(0f, 1f);
            
            if (randomChoice <= adjustedProbability) selected.Add(coord);
        }

        return selected.ToList();
    }

    public static List<AxialCoordinates> SelectSurrounding(
        Dictionary<AxialCoordinates, HexType> map,
        AxialCoordinates srcCoord,
        float diminishingProbabilityPerDistance,
        List<HexType> hexTypesToAvoid = null,
        List<AxialCoordinates> hexagonsToAvoid = null)
    {
        Debug.Assert(map is not null);
        Debug.Assert(diminishingProbabilityPerDistance is >= 0 and <= 1);

        HashSet<AxialCoordinates> visited = new();
        HashSet<AxialCoordinates> selected = new();
        Queue<AxialCoordinates> coordQueue = new();
        
        selected.Add(srcCoord);
        coordQueue.Enqueue(srcCoord);

        while (coordQueue.Count > 0)
        {
            foreach (var neighbor in HexagonGrid.GetHexRingsAroundCoordinates(coordQueue.Dequeue(), 1)
                         .Where(neighbor => map.Keys.Contains(neighbor))
                         .Where(neighbor => hexTypesToAvoid is null || !hexTypesToAvoid.Contains(map[neighbor]))
                         .Where(neighbor => hexagonsToAvoid is null || !hexagonsToAvoid.Contains(neighbor)))
            {
                if (!visited.Add(neighbor)) continue;
                if (selected.Contains(neighbor)) continue;
                
                var improbability = Math.Min(1f, neighbor.GetDistanceTo(srcCoord) * diminishingProbabilityPerDistance);
                var randomChoice = Random.Range(0, 1f);

                if (randomChoice <= improbability) continue;
                
                selected.Add(neighbor);
                coordQueue.Enqueue(neighbor);
            }
        }

        return selected.ToList();
    }
    
    private static float GetSelectionPercentageInArea(
        AxialCoordinates coord,
        int areaSize, 
        HashSet<AxialCoordinates> visited,
        HashSet<AxialCoordinates> selected)
    {
        var hexesInArea = HexagonGrid.GetHexRingsAroundCoordinates(coord, areaSize).ToHashSet();
        var numVisitedHexesInArea = hexesInArea.Intersect(visited).Count();
        var numSelectedHexesInArea = hexesInArea.Intersect(selected).Count();
            
        var selectionPercentageInArea = numVisitedHexesInArea > 0
            ? (float)numSelectedHexesInArea / numVisitedHexesInArea
            : 0f;

        return selectionPercentageInArea;
    }
    
    private static float GetSelectionProbability(float targetPercentage, float actualPercentage, float enforcement)
    {
        Debug.Assert(targetPercentage is >= 0 and <= 1);
        Debug.Assert(actualPercentage is >= 0 and <= 1);
        Debug.Assert(enforcement is >= 0 and <= 10);
        
        var deviation = targetPercentage - actualPercentage;
        var adjustedProbability = targetPercentage + deviation * enforcement;
        
        return Math.Clamp(adjustedProbability, 0f, 1f);
    }
}