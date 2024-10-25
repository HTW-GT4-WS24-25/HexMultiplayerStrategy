using System.Collections.Generic;
using UnityEngine;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        public bool isTraversable;
        public List<Unit.Unit> units = new();
        
        public AxialCoordinate Coordinates { get; private set; }

        public void Initialize(AxialCoordinate coordinate)
        {
            Coordinates = coordinate;
        }
    }
}