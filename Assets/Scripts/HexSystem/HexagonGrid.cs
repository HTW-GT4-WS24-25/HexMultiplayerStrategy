using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExtensionMethods;

namespace HexSystem
{
    public class HexagonGrid
    {
        private readonly Dictionary<AxialCoordinates, ClientHexagon> _grid = new();

        public int HexCount => _grid.Count;

        private static List<ClientHexagon> ExtractPathFromHexToPredecessorMatching(
            ClientHexagon srcHex,
            ClientHexagon destHex,
            Dictionary<ClientHexagon, ClientHexagon> hexToPredecessor)
        {
            Debug.Assert(hexToPredecessor != null);
            Debug.Assert(hexToPredecessor.ContainsValue(srcHex));
            Debug.Assert(hexToPredecessor.ContainsKey(destHex));

            var path = new List<ClientHexagon>();
            var hex = destHex;

            while (hex != srcHex)
            {
                var predecessor = hexToPredecessor[hex];
                path.Insert(0, hex);
                hex = predecessor;
            }

            return path;
        }

        public void Add(ClientHexagon hex) => _grid.Add(hex.Coordinates, hex);

        public ClientHexagon Get(int q, int r) => Get(new AxialCoordinates(q, r));

        public ClientHexagon Get(AxialCoordinates coordinates) => _grid.GetValueOrDefault(coordinates);

        public void Clear()
        {
            _grid.Clear();
        }

        public ClientHexagon GetNeighborOf(AxialCoordinates coord, Direction dir)
        {
            Debug.Assert(_grid.ContainsKey(coord), "Grid doesn't contain given coordinate.");

            return _grid[new AxialCoordinates(coord.Q + dir.GetQOffset(), coord.R + dir.GetROffset())];
        }

        public HashSet<ClientHexagon> GetAllNeighborsOf(AxialCoordinates coord)
        {
            Debug.Assert(_grid.ContainsKey(coord), "Grid doesn't contain given coordinate.");

            var neighbors = new HashSet<ClientHexagon>();

            foreach (var neighborCoords in coord.GetNeighbors())
            {
                if (_grid.TryGetValue(neighborCoords, out var value)) neighbors.Add(value);
            }

            return neighbors;
        }

        public List<ClientHexagon> GetAllHexagonsInRange(int range, AxialCoordinates coord)
        {
            Debug.Assert(range > 0, "Range cannot be negative or zero.");
            Debug.Assert(_grid.ContainsKey(coord), "Grid doesn't contain given coordinate.");

            var hexesInRange = new List<ClientHexagon>();

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

        public List<ClientHexagon> GetReachableHexagonsInRange(int range, AxialCoordinates srcCoord)
        {
            Debug.Assert(range > 0, "Range cannot be negative or zero.");
            Debug.Assert(_grid.ContainsKey(srcCoord), "Grid doesn't contain given coordinate.");

            var reachableHexes = new List<ClientHexagon> { _grid[srcCoord] };
            var numExploredHexes = 0;

            for (var ring = 1; ring <= range; ring++)
            {
                var numDiscoveredHexes = reachableHexes.Count;

                for (var i = numExploredHexes; i < numDiscoveredHexes; i++)
                {
                    foreach (var neighbor in GetAllNeighborsOf(reachableHexes[i].Coordinates)
                                 .Where(neighbor => neighbor.isTraversable && !reachableHexes.Contains(neighbor)))
                    {
                        reachableHexes.Add(neighbor);
                    }

                    numExploredHexes++;
                }
            }

            reachableHexes.RemoveAt(0);

            return reachableHexes;
        }

        public List<ClientHexagon> GetPathBetween(AxialCoordinates srcCoord, AxialCoordinates destCoord)
        {
            Debug.Assert(_grid.ContainsKey(srcCoord), "Grid doesn't contain given source coordinate.");
            Debug.Assert(_grid.ContainsKey(destCoord), "Grid doesn't contain given destination coordinate.");

            var srcHex = _grid[srcCoord];
            var destHex = _grid[destCoord];
            var hexQueue = new Queue<ClientHexagon>();
            var hexToPredecessor = new Dictionary<ClientHexagon, ClientHexagon>();

            hexQueue.Enqueue(srcHex);
            hexToPredecessor[srcHex] = null;

            while (hexQueue.Count > 0)
            {
                var currentHex = hexQueue.Dequeue();

                if (currentHex == destHex) break;

                foreach (var neighbor in GetAllNeighborsOf(currentHex.Coordinates)
                             .Where(neighbor => neighbor.isTraversable && !hexToPredecessor.ContainsKey(neighbor)))
                {
                    Console.WriteLine(neighbor.Coordinates);
                    hexQueue.Enqueue(neighbor);
                    hexToPredecessor[neighbor] = currentHex;
                }
            }

            return ExtractPathFromHexToPredecessorMatching(srcHex, destHex, hexToPredecessor);
        }

        public static IEnumerable<AxialCoordinates> GetHexRingsAroundCoordinates(AxialCoordinates coordinates,
            int nRings)
        {
            for (var q = -nRings; q <= nRings; q++)
            {
                for (var r = -nRings; r <= nRings; r++)
                {
                    var s = -q - r;
                
                    if (Math.Abs(q) > nRings || Math.Abs(r) > nRings || Math.Abs(s) > nRings) continue;
                    
                    yield return new AxialCoordinates(coordinates.Q + q, coordinates.R + r);
                }
            }
        }
    }
}