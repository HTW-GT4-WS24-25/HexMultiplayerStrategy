using HexSystem;
using UnityEngine;

namespace ExtensionMethods
{
    public static class VectorExtensions
    {
        public static Vector3 Rotate(this Vector3 vector, float degree, Vector3 axis)
        {
            return Quaternion.AngleAxis(degree, axis) * vector;
        }

        public static AxialCoordinates ToAxialCoordinates(this Vector2Int vector)
        {
            return new AxialCoordinates(vector.x, vector.y);
        }
    }
}