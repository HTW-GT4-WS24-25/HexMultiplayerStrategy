using System;
using UnityEngine;

public class Hexagon : MonoBehaviour
{ 
    public AxialCoordinate Coordinates { get; private set; }
    public bool IsTraversable;

    public void Initialize(AxialCoordinate coordinate)
    {
        Coordinates = coordinate;
    }
}