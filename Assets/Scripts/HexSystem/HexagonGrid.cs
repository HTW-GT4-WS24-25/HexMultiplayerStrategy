using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public class HexagonGrid
{
    private readonly Dictionary<AxialCoordinate, Hexagon> _grid = new();
    
    public int HexCount => _grid.Count;
    
    private static List<Hexagon> ExtractPathFromHexToPredecessorMatching(
        Hexagon srcHex,
        Hexagon destHex,
        Dictionary<Hexagon, Hexagon> hexToPredecessor)
    {
        Debug.Assert(hexToPredecessor != null);
        Debug.Assert(hexToPredecessor.ContainsValue(srcHex));
        Debug.Assert(hexToPredecessor.ContainsKey(destHex) );
        
        var path = new List<Hexagon>();
        var hex = destHex;

        while (hex != srcHex)
        {
            var predecessor = hexToPredecessor[hex];
            path.Insert(0, hex);
            hex = predecessor;
        }

        return path;
    }

    public void Add(Hexagon hex) => _grid.Add(hex.Coordinates, hex);
    
    public Hexagon? Get(int q, int r) => Get(new AxialCoordinate(q, r));
    
    public Hexagon? Get(AxialCoordinate coordinate) => _grid.GetValueOrDefault(coordinate);

    public Hexagon GetNeighborOf(AxialCoordinate coord, Direction dir)
    {
        Debug.Assert(_grid.ContainsKey(coord), "Grid doesn't contain given coordinate.");
        
        return _grid[new AxialCoordinate(coord.Q + dir.GetQOffset(), coord.R + dir.GetROffset())];
    }

    public HashSet<Hexagon> GetAllNeighborsOf(AxialCoordinate coord)
    {
        Debug.Assert(_grid.ContainsKey(coord), "Grid doesn't contain given coordinate.");
        
        var neighbors = new HashSet<Hexagon>();

        foreach (var neighborCoords in coord.GetNeighbors())
        {
            if (_grid.TryGetValue(neighborCoords, out var value)) neighbors.Add(value);
        }
        
        return neighbors;
    }

    public List<Hexagon> GetAllHexagonsInRange(int range, AxialCoordinate coord)
    {
        Debug.Assert(range > 0, "Range cannot be negative or zero.");
        Debug.Assert(_grid.ContainsKey(coord), "Grid doesn't contain given coordinate.");
        
        var hexesInRange = new List<Hexagon>();
        
        for (var q = coord.Q - range; q <= coord.Q + range; q++)
        {
            for (var r = coord.R - range; r <= coord.R + range; r++)
            {
                for (var s = coord.S - range; s <= coord.S + range; s++)
                {
                    if (q + r + s != 0) continue;
                    if (q == coord.Q && r == coord.R) continue;
                    if (_grid.TryGetValue(coord, out var value)) hexesInRange.Add(value);
                }
            }
        }
        
        return hexesInRange;
    }
    
    public List<Hexagon> GetReachableHexagonsInRange(int range, AxialCoordinate srcCoord)
    {
        Debug.Assert(range > 0, "Range cannot be negative or zero.");
        Debug.Assert(_grid.ContainsKey(srcCoord), "Grid doesn't contain given coordinate.");
        
        var reachableHexes = new List<Hexagon> { _grid[srcCoord] };
        var numExploredHexes = 0;

        for (var ring = 1; ring <= range; ring++)
        {
            var numDiscoveredHexes = reachableHexes.Count;
            
            for (var i = numExploredHexes; i < numDiscoveredHexes; i++)
            {
                foreach (var neighbor in GetAllNeighborsOf(reachableHexes[i].Coordinates)
                             .Where(neighbor => neighbor.IsTraversable && !reachableHexes.Contains(neighbor)))
                {
                    reachableHexes.Add(neighbor);
                }

                numExploredHexes++;
            }
        }

        reachableHexes.RemoveAt(0);

        return reachableHexes;
    }
    
    public List<Hexagon> GetPathBetween(AxialCoordinate srcCoord, AxialCoordinate destCoord)
    {
        Debug.Assert(_grid.ContainsKey(srcCoord), "Grid doesn't contain given source coordinate.");
        Debug.Assert(_grid.ContainsKey(destCoord), "Grid doesn't contain given destination coordinate.");
        
        var srcHex = _grid[srcCoord];
        var destHex = _grid[destCoord];
        var hexQueue = new Queue<Hexagon>();
        var hexToPredecessor = new Dictionary<Hexagon, Hexagon>();
        
        hexQueue.Enqueue(srcHex);
        hexToPredecessor[srcHex] = null;
            
        while (hexQueue.Count > 0)
        {
            var currentHex = hexQueue.Dequeue();

            if (currentHex == destHex) break;
            
            foreach (var neighbor in GetAllNeighborsOf(currentHex.Coordinates)
                         .Where(neighbor => neighbor.IsTraversable && !hexToPredecessor.ContainsKey(neighbor)))
            {
                Console.WriteLine(neighbor.Coordinates);
                hexQueue.Enqueue(neighbor);
                hexToPredecessor[neighbor] = currentHex;
            }
        }
        
        return ExtractPathFromHexToPredecessorMatching(srcHex, destHex, hexToPredecessor);
    }
}