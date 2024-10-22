// using HexagonalGrid;
//
// const int rings = 3;
// const int range = 2;
// var srcCoord = new AxialCoordinate(-2, 2);
// var destCoord = new AxialCoordinate(2, -2);
// var grid = new HexagonGrid(rings);
//
// grid.Get(-3, 2)!.IsTraversable = false;
// grid.Get(-2, 1)!.IsTraversable = false;
// grid.Get(-1, 1)!.IsTraversable = false;
//
// grid.Get(0, -1)!.IsTraversable = false;
// grid.Get(1, -1)!.IsTraversable = false;
// grid.Get(2, -1)!.IsTraversable = false;
// grid.Get(3, -1)!.IsTraversable = false;
//
// Console.WriteLine($"Rings: {rings}");
// Console.WriteLine($"Range: {range}");
// Console.WriteLine($"Coordinate: {srcCoord}");
// Console.WriteLine("---");
// Console.WriteLine($"Count hexes: {grid.HexCount}");
// Console.WriteLine($"Count hexes in range: {grid.GetAllHexagonsInRange(range, srcCoord).Count}");
// Console.WriteLine($"Count hexes in reach: {grid.GetReachableHexagonsInRange(range, srcCoord).Count}");
// Console.WriteLine($"Optimal path:\n{grid.GetPathBetween(srcCoord, destCoord).Aggregate("", (current, dir) => current + $"\t{dir}\n")}");