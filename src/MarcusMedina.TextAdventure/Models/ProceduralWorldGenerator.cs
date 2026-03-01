// <copyright file="ProceduralWorldGenerator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Enums;
using MarcusMedina.TextAdventure.Helpers;
using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Configuration for procedural world generation.
/// </summary>
public record WorldGenerationOptions(
    string Name,
    int RoomCount = 10,
    float Connectivity = 0.3f,
    string Theme = "dungeon",
    int ItemDensity = 2,
    int NpcDensity = 1
);

/// <summary>
/// Generates procedural worlds with rooms, connections, items, and NPCs.
/// Supports themes: dungeon, mansion, forest.
/// </summary>
public sealed class ProceduralWorldGenerator
{
    private static readonly Random Rng = Random.Shared;

    /// <summary>
    /// Generates a complete procedural world.
    /// </summary>
    public List<ILocation> GenerateWorld(WorldGenerationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var rooms = new List<ILocation>();

        // Generate rooms based on theme
        for (int i = 0; i < options.RoomCount; i++)
        {
            var template = RoomTemplates.GetRandomTemplate(options.Theme);
            if (template is not null)
            {
                var room = new Location(
                    $"{options.Theme}_room_{i}",
                    template.Name,
                    template.Description
                );
                rooms.Add(room);
            }
        }

        if (rooms.Count == 0)
            return [];

        // Connect rooms using minimum spanning tree + random extra connections
        ConnectRooms(rooms, options.Connectivity);

        return rooms;
    }

    private void ConnectRooms(List<ILocation> rooms, float connectivity)
    {
        if (rooms.Count < 2)
            return;

        var visited = new HashSet<ILocation> { rooms[0] };
        var queue = new Queue<ILocation>();
        queue.Enqueue(rooms[0]);

        // Create minimum spanning tree first
        while (queue.Count > 0 && visited.Count < rooms.Count)
        {
            var current = queue.Dequeue();
            var currentLocation = current as Location;

            if (currentLocation is null)
                continue;

            // Find unvisited rooms and connect
            var unvisited = rooms.Where(r => !visited.Contains(r)).ToList();
            if (unvisited.Count > 0)
            {
                var next = unvisited[Rng.Next(unvisited.Count)];
                var nextLocation = next as Location;

                if (nextLocation is null)
                    continue;

                var direction = GetRandomDirection();
                var opposite = GetOppositeDirection(direction);

                currentLocation.AddExit(direction, next);
                if (direction != Direction.Up && direction != Direction.Down)
                {
                    nextLocation.AddExit(opposite, current);
                }

                visited.Add(next);
                queue.Enqueue(next);
            }
        }

        // Add extra connections based on connectivity
        var extraConnections = (int)((rooms.Count - 1) * connectivity);
        for (int i = 0; i < extraConnections; i++)
        {
            var from = rooms[Rng.Next(rooms.Count)] as Location;
            var to = rooms[Rng.Next(rooms.Count)];

            if (from is null || from == to)
                continue;

            var direction = GetRandomDirection();
            if (!from.Exits.ContainsKey(direction))
            {
                from.AddExit(direction, to);
            }
        }
    }

    private static Direction GetRandomDirection()
    {
        var directions = new[]
        {
            Direction.North, Direction.South, Direction.East, Direction.West,
            Direction.NorthEast, Direction.NorthWest, Direction.SouthEast, Direction.SouthWest
        };
        return directions[Rng.Next(directions.Length)];
    }

    private static Direction GetOppositeDirection(Direction direction) =>
        DirectionHelper.GetOpposite(direction);
}
