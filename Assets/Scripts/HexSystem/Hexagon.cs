using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HexSystem
{
    public class Hexagon : MonoBehaviour
    {
        public AxialCoordinate Coordinates { get; private set; }
        public bool IsTraversable;
        public List<Unit> units = new();

        public void Initialize(AxialCoordinate coordinate)
        {
            Coordinates = coordinate;
        }
    }
}