// <copyright file="MapLayoutCalculator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Calculates layout positions for rooms using graph-based BFS algorithm.
/// Positions rooms spatially based on their exit connections.
/// </summary>
public sealed class MapLayoutCalculator
{
    /// <summary>
    /// Calculates spatial positions for all locations using breadth-first search.
    /// </summary>
    public Dictionary<ILocation, Point> Calculate(IEnumerable<ILocation> locations)
    {
        var locationList = locations.ToList();
        if (locationList.Count == 0)
            return [];

        var positions = new Dictionary<ILocation, Point>();
        var visited = new HashSet<ILocation>();
        var queue = new Queue<(ILocation location, Point position)>();

        var startLocation = locationList[0];
        queue.Enqueue((startLocation, new Point(0, 0)));

        while (queue.Count > 0)
        {
            var (currentLocation, currentPos) = queue.Dequeue();

            if (visited.Contains(currentLocation))
                continue;

            visited.Add(currentLocation);
            positions[currentLocation] = currentPos;

            // Add adjacent locations to queue
            foreach (var (direction, exit) in currentLocation.Exits)
            {
                if (!visited.Contains(exit.Target))
                {
                    var nextPos = GetAdjacentPosition(currentPos, direction);
                    queue.Enqueue((exit.Target, nextPos));
                }
            }
        }

        // Normalise positions to fit within reasonable bounds
        return NormalisePositions(positions);
    }

    private static Point GetAdjacentPosition(Point position, Direction direction) =>
        direction switch
        {
            Direction.North => position with { Y = position.Y - 1 },
            Direction.South => position with { Y = position.Y + 1 },
            Direction.East => position with { X = position.X + 1 },
            Direction.West => position with { X = position.X - 1 },
            Direction.Up => position with { Y = position.Y - 2 },
            Direction.Down => position with { Y = position.Y + 2 },
            Direction.NorthEast => position with { X = position.X + 1, Y = position.Y - 1 },
            Direction.NorthWest => position with { X = position.X - 1, Y = position.Y - 1 },
            Direction.SouthEast => position with { X = position.X + 1, Y = position.Y + 1 },
            Direction.SouthWest => position with { X = position.X - 1, Y = position.Y + 1 },
            _ => position
        };

    private static Dictionary<ILocation, Point> NormalisePositions(Dictionary<ILocation, Point> positions)
    {
        if (positions.Count == 0)
            return positions;

        var minX = positions.Values.Min(p => p.X);
        var minY = positions.Values.Min(p => p.Y);
        var maxX = positions.Values.Max(p => p.X);
        var maxY = positions.Values.Max(p => p.Y);

        // Offset to non-negative coordinates
        var normalised = new Dictionary<ILocation, Point>();
        foreach (var (loc, pos) in positions)
        {
            var offsetX = pos.X - minX;
            var offsetY = pos.Y - minY;

            // Limit to reasonable size (80x24 is typical terminal)
            var scale = 1;
            var width = maxX - minX + 1;
            var height = maxY - minY + 1;

            if (width > 40)
                scale = width / 40 + 1;

            var scaledX = offsetX / scale;
            var scaledY = offsetY / scale;

            normalised[loc] = new Point(scaledX, scaledY);
        }

        return normalised;
    }
}
