using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;

namespace HexSystem
{
    public struct AxialCoordinates : IEquatable<AxialCoordinates>, INetworkSerializable
    {
        public static bool operator ==(AxialCoordinates left, AxialCoordinates right) => left.Equals(right);
        public static bool operator !=(AxialCoordinates left, AxialCoordinates right) => !(left == right);

        public int Q;
        public int R;
        public int S => -Q - R;

        public int Length => (Math.Abs(Q) + Math.Abs(R) + Math.Abs(S)) / 2;

        /// <summary>
        /// Is computed everytime instead of stored to minimize memory usage. Since AxialCoordinates are structs,
        /// this computation doesn't generate much overhead besides instantiating and garbage collecting the list.
        /// </summary>
        public List<AxialCoordinates> GetNeighbors() => new()
        {
            new(Q, R - 1),
            new(Q + 1, R - 1),
            new(Q + 1, R),
            new(Q, R + 1),
            new(Q - 1, R + 1),
            new(Q - 1, R)
        };

        public AxialCoordinates(int q, int r)
        {
            Q = q;
            R = r;

            Debug.Assert(Q + R + S == 0);
        }
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Q);
            serializer.SerializeValue(ref R);
        }

        public AxialCoordinates this[int i] => GetNeighbors()[i];
        public AxialCoordinates this[Direction dir] => GetNeighbors()[(int)dir];

        public override bool Equals(object obj) => obj is AxialCoordinates coordinate && Equals(coordinate);
        public override int GetHashCode() => HashCode.Combine(Q, R);
        public override string ToString() => $"({Q}, {R})";
        public bool Equals(AxialCoordinates other) => Q == other.Q && R == other.R;

        public Direction GetDirectionToNeighbor(AxialCoordinates coord)
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

        public int GetDistanceTo(AxialCoordinates other) => GetSubtractedBy(other).Length;

        public AxialCoordinates GetAddedBy(int addend) => new AxialCoordinates(Q + addend, R + addend);
        public AxialCoordinates GetAddedBy(AxialCoordinates other) => new AxialCoordinates(Q + other.Q, R + other.R);

        public AxialCoordinates GetSubtractedBy(int minuend) => new AxialCoordinates(Q - minuend, R - minuend);
        public AxialCoordinates GetSubtractedBy(AxialCoordinates other) => new AxialCoordinates(Q - other.Q, R - other.R);

        public AxialCoordinates GetScaledBy(int scalar) => new AxialCoordinates(Q * scalar, R * scalar);
        public AxialCoordinates GetScaledBy(AxialCoordinates other) => new AxialCoordinates(Q * other.Q, R * other.R);
    }
}