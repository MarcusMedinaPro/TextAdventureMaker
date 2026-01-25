using MarcusMedina.TextAdventure.Enums;

namespace MarcusMedina.TextAdventure.Helpers;

public static class DirectionHelper
{
    public static Direction GetOpposite(Direction direction) => direction switch
    {
        Direction.North => Direction.South,
        Direction.South => Direction.North,
        Direction.East => Direction.West,
        Direction.West => Direction.East,
        Direction.Up => Direction.Down,
        Direction.Down => Direction.Up,
        Direction.NorthEast => Direction.SouthWest,
        Direction.NorthWest => Direction.SouthEast,
        Direction.SouthEast => Direction.NorthWest,
        Direction.SouthWest => Direction.NorthEast,
        Direction.In => Direction.Out,
        Direction.Out => Direction.In,
        _ => throw new ArgumentOutOfRangeException(nameof(direction))
    };
}
