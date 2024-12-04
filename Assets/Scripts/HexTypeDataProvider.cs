using System;

public static class HexTypeDataProvider
{
    public static bool IsTraversable(HexType type)
    {
        return type switch
        {
            HexType.Forest or HexType.Grass => true,
            HexType.Mountain => false,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unhandled value.")
        };
    }
        
    public static float GetMovementSpeedFactor(HexType type)
    {
        return type switch
        {
            HexType.Forest => 0.5f,
            HexType.Grass => 1f,
            HexType.Mountain => throw new ArgumentException(
                "You shouldn't ever try to retrieve a movement speed factor when working with this hex type."),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unhandled value.")
        };
    }
}