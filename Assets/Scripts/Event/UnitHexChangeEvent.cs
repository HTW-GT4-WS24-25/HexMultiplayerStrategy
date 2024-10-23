using HexSystem;
using UnityEngine;

namespace GameEvent
{
    [CreateAssetMenu(fileName = "New UnitHexChangeEvent", menuName = "Events/UnitHexChangeEvent")]
    public class UnitHexChangeEvent : GameEvent<(Unit,AxialCoordinate)> { }

}
