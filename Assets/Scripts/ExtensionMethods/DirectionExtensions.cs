using System;
using System.Drawing;

public static class DirectionExtensions
{
    public static Direction GetInverse(this Direction dir)
    {
        return dir switch
        {
            Direction.Up => Direction.Down,
            Direction.Down => Direction.Up,
            Direction.LeftUp => Direction.RightDown,
            Direction.LeftDown => Direction.RightUp,
            Direction.RightUp => Direction.LeftDown,
            Direction.RightDown => Direction.LeftUp,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, "Unknown direction.")
        };
    }

    public static int GetQOffset(this Direction dir)
    {
        return dir switch
        {
            Direction.Up or Direction.Down => 0,
            Direction.LeftUp or Direction.LeftDown => -1,
            Direction.RightUp or Direction.RightDown => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, "Unknown direction.")
        };
    }
    
    public static int GetROffset(this Direction dir)
    {
        return dir switch
        {
            Direction.LeftUp or Direction.RightDown => 0,
            Direction.Up or Direction.RightUp => -1,
            Direction.Down or Direction.LeftDown => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(dir), dir, "Unknown direction.")
        };
    }
}