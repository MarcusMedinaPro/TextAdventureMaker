// <copyright file="DirectionHelper.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace MarcusMedina.TextAdventure.Helpers;

using MarcusMedina.TextAdventure.Enums;

public static class DirectionHelper
{
    private static readonly Dictionary<string, Direction> Aliases = new(StringComparer.OrdinalIgnoreCase)
    {
        ["n"] = Direction.North, ["s"] = Direction.South,
        ["e"] = Direction.East, ["w"] = Direction.West,
        ["ne"] = Direction.NorthEast, ["nw"] = Direction.NorthWest,
        ["se"] = Direction.SouthEast, ["sw"] = Direction.SouthWest,
        ["u"] = Direction.Up, ["d"] = Direction.Down,
        ["in"] = Direction.In, ["out"] = Direction.Out
    };

    /// <summary>
    /// Attempts to parse a direction from text, supporting both full names and abbreviations.
    /// </summary>
    public static bool TryParse(string? input, out Direction direction)
    {
        direction = default;
        if (string.IsNullOrWhiteSpace(input))
            return false;
        var trimmed = input.Trim();
        if (Enum.TryParse(trimmed, ignoreCase: true, out direction))
            return true;
        return Aliases.TryGetValue(trimmed, out direction);
    }

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