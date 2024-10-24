using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace HexSystem
{
    public readonly struct AxialCoordinate : IEquatable<AxialCoordinate>
    {
        public static bool operator ==(AxialCoordinate left, AxialCoordinate right) => left.Equals(right);
        public static bool operator !=(AxialCoordinate left, AxialCoordinate right) => !(left == right);

        public int Q { get; }
        public int R { get; }
        public int S => -Q - R;

        public int Length => (Math.Abs(Q) + Math.Abs(R) + Math.Abs(S)) / 2;

        /// <summary>
        /// Is computed everytime instead of stored to minimize memory usage. Since AxialCoordinates are structs,
        /// this computation doesn't generate much overhead besides instantiating and garbage collecting the list.
        /// </summary>
        public List<AxialCoordinate> GetNeighbors() => new()
        {
            new(Q, R - 1),
            new(Q + 1, R - 1),
            new(Q + 1, R),
            new(Q, R + 1),
            new(Q - 1, R + 1),
            new(Q - 1, R)
        };

        public AxialCoordinate(int q, int r)
        {
            Q = q;
            R = r;

            Debug.Assert(Q + R + S == 0);
        }

        public AxialCoordinate this[int i] => GetNeighbors()[i];
        public AxialCoordinate this[Direction dir] => GetNeighbors()[(int)dir];

        public override bool Equals(object obj) => obj is AxialCoordinate coordinate && Equals(coordinate);
        public override int GetHashCode() => HashCode.Combine(Q, R);
        public override string ToString() => $"({Q}, {R})";
        public bool Equals(AxialCoordinate other) => Q == other.Q && R == other.R;

        public Direction GetDirectionToNeighbor(AxialCoordinate coord)
        {
            Debug.Assert(GetNeighbors().Contains(coord));

            return coord.GetSubtractedBy(this) switch
            {
                { Q: 0, R: -1 } => Direction.Up,
                { Q: 1, R: -1 } => Direction.RightUp,
                { Q: 1, R: 0 } => Direction.RightDown,
                { Q: 0, R: 1 } => Direction.Down,
                { Q: -1, R: 1 } => Direction.LeftDown,
                { Q: -1, R: 0 } => Direction.LeftUp,
                _ => throw new ArgumentOutOfRangeException(nameof(coord), coord, "Invalid neighbor coordinate.")
            };
        }

        public int GetDistanceTo(AxialCoordinate other) => GetSubtractedBy(other).Length;

        public AxialCoordinate GetAddedBy(int addend) => new AxialCoordinate(Q + addend, R + addend);
        public AxialCoordinate GetAddedBy(AxialCoordinate other) => new AxialCoordinate(Q + other.Q, R + other.R);

        public AxialCoordinate GetSubtractedBy(int minuend) => new AxialCoordinate(Q - minuend, R - minuend);
        public AxialCoordinate GetSubtractedBy(AxialCoordinate other) => new AxialCoordinate(Q - other.Q, R - other.R);

        public AxialCoordinate GetScaledBy(int scalar) => new AxialCoordinate(Q * scalar, R * scalar);
        public AxialCoordinate GetScaledBy(AxialCoordinate other) => new AxialCoordinate(Q * other.Q, R * other.R);
    }
}