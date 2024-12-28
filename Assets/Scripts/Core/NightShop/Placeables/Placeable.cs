using Core.HexSystem.Hex;
using UnityEngine;

namespace Core.NightShop.Placeables
{
    public abstract class Placeable : ScriptableObject
    {
        public abstract void Initialize(ulong playerId);
        public abstract bool IsHexValidForPlacement(HexagonData hexagon);
        public abstract void Place(HexagonData hexagon);
    }
}