// <copyright file="MapGenerator.cs" company="Marcus Ackre Medina">
// Copyright (c) Marcus Ackre Medina. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using MarcusMedina.TextAdventure.Interfaces;

namespace MarcusMedina.TextAdventure.Models;

/// <summary>
/// Generates ASCII maps and procedural world data from game state.
/// </summary>
public sealed class MapGenerator : IMapGenerator
{
    private readonly MapLayoutCalculator _layoutCalculator = new();
    private readonly AsciiMapRenderer _renderer = new();

    /// <summary>
    /// Generates an ASCII representation of the world.
    /// </summary>
    public string GenerateAsciiMap(IGameState state, MapOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(state);
        options ??= new MapOptions();

        var mapData = GenerateMapData(state);
        return _renderer.Render(mapData, state, options);
    }

    /// <summary>
    /// Generates structured map data from current game state.
    /// </summary>
    public MapData GenerateMapData(IGameState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        // Need to get locations from a concrete GameState, not IGameState
        // For now, just use current location to generate minimal map
        var startLocation = state.CurrentLocation;
        var visitedLocations = GetVisitedLocations(state);

        // Calculate layout positions for all locations
        var positions = _layoutCalculator.Calculate(visitedLocations);

        // Build connections from exits
        var connections = BuildConnections(positions.Keys);

        return new MapData(positions, connections);
    }

    private static List<ILocation> GetVisitedLocations(IGameState state)
    {
        var visited = new HashSet<ILocation> { state.CurrentLocation };
        var queue = new Queue<ILocation>();
        queue.Enqueue(state.CurrentLocation);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            foreach (var exit in current.Exits.Values)
            {
                if (visited.Add(exit.Target))
                {
                    queue.Enqueue(exit.Target);
                }
            }
        }

        return visited.ToList();
    }

    private static List<MapConnection> BuildConnections(IEnumerable<ILocation> locations)
    {
        var connections = new List<MapConnection>();
        var visitedPairs = new HashSet<string>();

        foreach (var location in locations)
        {
            foreach (var (direction, exit) in location.Exits)
            {
                var pairKey = GetPairKey(location, exit.Target);
                if (visitedPairs.Add(pairKey))
                {
                    connections.Add(new MapConnection(location, exit.Target, direction));
                }
            }
        }

        return connections;
    }

    private static string GetPairKey(ILocation a, ILocation b) =>
        $"{a.Id}↔{b.Id}".ToLowerInvariant();
}
